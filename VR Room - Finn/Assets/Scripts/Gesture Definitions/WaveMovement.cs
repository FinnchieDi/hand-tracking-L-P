using UnityEngine;
using UnityEngine.XR.Hands.Gestures;

namespace UnityEngine.XR.Hands.Samples.GestureSample
{
    [CreateAssetMenu(fileName = "Wave Movement Object", menuName = "XRHands/Gestures/Wave Movement")]
    public class WaveMovement : XRDynamicHandGesture
    {
        // Threshold for how far the wrist must move horizontally to count as a wave
        [SerializeField] float waveThreshold = 0.1f;

        // Time window to reset the wave detection (seconds)
        [SerializeField] float resetTime = 1f;

        float lastWristX = 0f;
        bool movedRight = false;
        bool movedLeft = false;
        float lastUpdateTime = 0f;

        public override bool CheckMovement(XRHandJointsUpdatedEventArgs eventArgs)
        {
            Debug.Log("CheckMovement called");
            // Get wrist joint position
            var wristJoint = eventArgs.hand.GetJoint(XRHandJointID.Wrist);

            if (!wristJoint.TryGetPose(out Pose wristPose))
            {
                Debug.Log("Wrist joint pose not valid");
                return false;
            }

            float currentX = wristPose.position.x;
            float deltaX = currentX - lastWristX;

            // Reset if too much time has passed
            if (Time.time - lastUpdateTime > resetTime)
            {
                movedRight = false;
                movedLeft = false;
            }

            // Detect movement right
            if (deltaX > waveThreshold)
                movedRight = true;

            // Detect movement left after moving right
            if (movedRight && deltaX < -waveThreshold)
                movedLeft = true;

            lastWristX = currentX;
            lastUpdateTime = Time.time;

            // Wave detected if moved right then left in sequence within resetTime
            return movedRight && movedLeft;
        }
    }
    
}
