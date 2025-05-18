using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Hands.Gestures;

namespace UnityEngine.XR.Hands.Samples.GestureSample
{
    public class DynamicHandGesture : MonoBehaviour
    {
        [SerializeField]
        XRHandTrackingEvents m_HandTrackingEvents;

        [SerializeField]
        ScriptableObject m_HandShapeOrPose;

        [SerializeField]
        ScriptableObject m_DynamicMovementAction; // NEW: Movement check

        [SerializeField]
        Transform m_TargetTransform;

        [SerializeField]
        Image m_Background;

        [SerializeField]
        UnityEvent m_GesturePerformed;

        [SerializeField]
        UnityEvent m_GestureEnded;

        [SerializeField]
        float m_MinimumHoldTime = 0.2f;

        [SerializeField]
        float m_GestureDetectionInterval = 0.1f;

        [SerializeField]
        StaticHandGesture[] m_StaticGestures;

        XRHandShape m_HandShape;
        XRHandPose m_HandPose;
        XRDynamicHandGesture m_DynamicGesture; // NEW: casted object

        bool m_WasDetected;
        bool m_PerformedTriggered;
        float m_TimeOfLastConditionCheck;
        float m_HoldStartTime;
        Color m_BackgroundDefaultColor;
        Color m_BackgroundHiglightColor = new Color(0f, 0.627451f, 1f);

        void OnEnable()
        {
            if (m_HandTrackingEvents != null)
                m_HandTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);

            m_HandShape = m_HandShapeOrPose as XRHandShape;
            m_HandPose = m_HandShapeOrPose as XRHandPose;
            m_DynamicGesture = m_DynamicMovementAction as XRDynamicHandGesture;
            Debug.Log("Dynamic gesture assigned: " + (m_DynamicGesture != null));

            if (m_HandPose != null && m_HandPose.relativeOrientation != null)
                m_HandPose.relativeOrientation.targetTransform = m_TargetTransform;
        }

        void OnDisable()
        {
            if (m_HandTrackingEvents != null)
                m_HandTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);
        }

        void OnJointsUpdated(XRHandJointsUpdatedEventArgs eventArgs)
        {
            if (m_HandTrackingEvents == null)
            {
                Debug.LogWarning("m_HandTrackingEvents is not assigned.");
                return;
            }

            if (!isActiveAndEnabled || Time.timeSinceLevelLoad < m_TimeOfLastConditionCheck + m_GestureDetectionInterval)
                return;

            var shapeDetected = m_HandTrackingEvents.handIsTracked &&
                ((m_HandShape != null && m_HandShape.CheckConditions(eventArgs)) ||
                (m_HandPose != null && m_HandPose.CheckConditions(eventArgs)));
            //
            if (shapeDetected) Debug.Log("Pose Detected");

            bool movementDetected =
                m_DynamicGesture == null ||
                m_DynamicGesture.CheckMovement(eventArgs);
            //bool movementDetected = true;

            bool gestureDetected = shapeDetected && movementDetected;

            if (!m_WasDetected && gestureDetected)
            {
                m_HoldStartTime = Time.timeSinceLevelLoad;
                //
                Debug.Log("Gesture Detected");

            }
            else if (m_WasDetected && !gestureDetected)
            {
                m_PerformedTriggered = false;
                m_GestureEnded?.Invoke();
                // m_Background.color = m_BackgroundDefaultColor;
            }

            m_WasDetected = gestureDetected;

            if (!m_PerformedTriggered && gestureDetected)
            {
                float holdTime = Time.timeSinceLevelLoad - m_HoldStartTime;
                if (holdTime > m_MinimumHoldTime)
                {
                    m_PerformedTriggered = true;
                    m_GesturePerformed?.Invoke();
                    // m_Background.color = m_BackgroundHiglightColor;

                }
            }

            m_TimeOfLastConditionCheck = Time.timeSinceLevelLoad;
        }
    }
    public abstract class XRDynamicHandGesture : ScriptableObject
    {
        public abstract bool CheckMovement(XRHandJointsUpdatedEventArgs eventArgs);
    }


}


