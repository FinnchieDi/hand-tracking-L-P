using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using TMPro;

public class XRHandGestureTracker : MonoBehaviour
{
    public XRNode handNode = XRNode.RightHand;
    private XRHandSubsystem handSubsystem;
    private XRHand trackedHand;

    [Header("Gesture Thresholds")]
    public float waveMovementThreshold = 0.05f;
    public float waveTimeWindow = 0.5f;
    public float beckonZMovementThreshold = 0.05f;
    public float beckonTimeWindow = 0.5f;

    [Header("Gesture History Tracking")]
    private Queue<Vector3> indexTipPositions = new Queue<Vector3>();
    private Queue<float> positionTimestamps = new Queue<float>();

    [Header("Output")]
    public string currentGesture = "None";
    public TextMeshProUGUI leftHandText;
    public TextMeshProUGUI rightHandText;

    void Start()
    {
        List<XRHandSubsystem> handSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetInstances(handSubsystems);

        if (handSubsystems.Count > 0)
        {
            handSubsystem = handSubsystems[0];
            Debug.Log("XRHandSubsystem active.");
        }
        else
        {
            Debug.LogWarning("No XRHandSubsystem found.");
        }
    }

    void Update()
    {
        if (handSubsystem == null) return;

        trackedHand = (handNode == XRNode.LeftHand) ? handSubsystem.leftHand : handSubsystem.rightHand;

        if (!trackedHand.isTracked) return;

        var indexTip = trackedHand.GetJoint(XRHandJointID.IndexTip);
        if (!indexTip.TryGetPose(out Pose pose)) return;

        float currentTime = Time.time;
        indexTipPositions.Enqueue(pose.position);
        positionTimestamps.Enqueue(currentTime);

        while (positionTimestamps.Count > 0 && currentTime - positionTimestamps.Peek() > waveTimeWindow)
        {
            indexTipPositions.Dequeue();
            positionTimestamps.Dequeue();
        }

        DetectWaveGesture();
        DetectBeckonGesture();
        // Optional: Add static pose recognition here

        UpdateUIText();
    }

    void DetectWaveGesture()
    {
        if (indexTipPositions.Count < 2) return;

        Vector3 first = indexTipPositions.Peek();
        Vector3 last = indexTipPositions.ToArray()[indexTipPositions.Count - 1];

        float xDiff = Mathf.Abs(last.x - first.x);
        if (xDiff > waveMovementThreshold)
        {
            currentGesture = "Wave";
        }
    }

    void DetectBeckonGesture()
    {
        if (indexTipPositions.Count < 2) return;

        Vector3 first = indexTipPositions.Peek();
        Vector3 last = indexTipPositions.ToArray()[indexTipPositions.Count - 1];

        float zDiff = first.z - last.z;
        if (zDiff > beckonZMovementThreshold)
        {
            currentGesture = "Beckon";
        }
    }

    void UpdateUIText()
    {
        if (handNode == XRNode.LeftHand)
        {
            if (leftHandText != null)
                leftHandText.text = "Left Gesture: " + currentGesture;
            if (rightHandText != null)
                rightHandText.text = "";
        }
        else if (handNode == XRNode.RightHand)
        {
            if (rightHandText != null)
                rightHandText.text = "Right Gesture: " + currentGesture;
            if (leftHandText != null)
                leftHandText.text = "";
        }
    }
}
