using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
public class XRHandPoseGestureTracker : MonoBehaviour
{
    [Header("Hand Tracking Events")]
    [Tooltip("The hand tracking events component to subscribe to receive updated joint data.")]
    public XRHandTrackingEvents handTrackingEvents;

    [Header("Built-in Gesture Asset")]
    [Tooltip("The XRHandShape, XRHandPose, or XRHandMovementGesture asset to detect static or movement gestures.")]
    public ScriptableObject handGestureAsset;

    [Header("Custom Dynamic Gesture Settings")]
    [Tooltip("Number of frames to keep for dynamic gesture detection.")]
    public int maxTrackedPositions = 20;

    [Tooltip("Minimum horizontal movement (meters) for detecting Wave gesture.")]
    public float waveMovementThreshold = 0.1f;

    [Tooltip("Minimum forward movement (meters) for detecting Beckon gesture.")]
    public float beckonZMovementThreshold = 0.05f;

    [Header("Events")]
    public UnityEvent onStaticGesturePerformed;
    public UnityEvent onStaticGestureEnded;
    public UnityEvent onWaveGestureDetected;
    public UnityEvent onBeckonGestureDetected;

    // Internal state
    private XRHandShape handShape;
    private XRHandPose handPose;
    //private XRHandMovementGesture handMovementGesture;

    private bool wasStaticGestureDetected = false;
    private string currentDynamicGesture = "";

    private Queue<Vector3> indexTipPositions = new Queue<Vector3>();

    private float lastGestureCheckTime = 0f;
    public float gestureDetectionInterval = 0.1f; // seconds

    private void OnEnable()
    {
        if (handTrackingEvents == null)
        {
            Debug.LogError("HandTrackingEvents is not assigned!");
            enabled = false;
            return;
        }

        handTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);

        // Detect type of gesture asset assigned
        handShape = handGestureAsset as XRHandShape;
        handPose = handGestureAsset as XRHandPose;
        //handMovementGesture = handGestureAsset as XRHandMovementGesture;

        // If handPose uses relative orientation with a target transform, assign it here if needed
        if (handPose != null && handPose.relativeOrientation != null)
        {
            // Optional: assign targetTransform if you want orientation relative to something
            // handPose.relativeOrientation.targetTransform = someTransform;
        }
    }

    private void OnDisable()
    {
        if (handTrackingEvents != null)
            handTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);
    }

    private void OnJointsUpdated(XRHandJointsUpdatedEventArgs eventArgs)
    {
        if (!isActiveAndEnabled)
            return;

        // Limit gesture detection frequency
        if (Time.timeSinceLevelLoad < lastGestureCheckTime + gestureDetectionInterval)
            return;

        lastGestureCheckTime = Time.timeSinceLevelLoad;

        // 1) Update index tip positions queue for dynamic gestures
        XRHand hand = eventArgs.hand;
        XRHandJoint indexTip = hand.GetJoint(XRHandJointID.IndexTip);

        if (indexTip.TryGetPose(out Pose tipPose))
        {
            Vector3 tipPos = tipPose.position;
            indexTipPositions.Enqueue(tipPos);
            if (indexTipPositions.Count > maxTrackedPositions)
                indexTipPositions.Dequeue();
        }
        DetectWaveGesture();
        DetectBeckonGesture();


        // 2) Check built-in gesture asset conditions (static or movement)
        bool detected = false;
        if (handShape != null)
        {
            detected = handShape.CheckConditions(eventArgs);
        }
        else if (handPose != null)
        {
            detected = handPose.CheckConditions(eventArgs);
        }
        //else if (handMovementGesture != null)
        //{
        //    detected = handMovementGesture.CheckConditions(eventArgs);
        //}

        if (!wasStaticGestureDetected && detected)
        {
            onStaticGesturePerformed?.Invoke();
        }
        else if (wasStaticGestureDetected && !detected)
        {
            onStaticGestureEnded?.Invoke();
        }

        wasStaticGestureDetected = detected;
    }

    private void DetectWaveGesture()
    {
        if (indexTipPositions.Count < 2)
            return;

        Vector3 first = indexTipPositions.Peek();
        Vector3 last = new List<Vector3>(indexTipPositions)[indexTipPositions.Count - 1];
        float xDiff = Mathf.Abs(last.x - first.x);

        if (xDiff > waveMovementThreshold)
        {
            if (currentDynamicGesture != "Wave")
            {
                currentDynamicGesture = "Wave";
                Debug.Log("Detected: Wave Gesture");
                onWaveGestureDetected?.Invoke();
            }
        }
        else if (currentDynamicGesture == "Wave")
        {
            // Gesture ended
            currentDynamicGesture = "";
        }
    }

    private void DetectBeckonGesture()
    {
        if (indexTipPositions.Count < 2)
            return;

        Vector3 first = indexTipPositions.Peek();
        Vector3 last = new List<Vector3>(indexTipPositions)[indexTipPositions.Count - 1];
        float zDiff = first.z - last.z;

        if (zDiff > beckonZMovementThreshold)
        {
            if (currentDynamicGesture != "Beckon")
            {
                currentDynamicGesture = "Beckon";
                Debug.Log("Detected: Beckon Gesture");
                onBeckonGestureDetected?.Invoke();
            }
        }
        else if (currentDynamicGesture == "Beckon")
        {
            // Gesture ended
            currentDynamicGesture = "";
        }
    }
}
