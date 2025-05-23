using UnityEngine;
using UnityEngine.XR.Hands.Gestures;

namespace UnityEngine.XR.Hands.Samples.GestureSample
{
    [CreateAssetMenu(fileName = "Wave Movement Object", menuName = "XRHands/Gestures/Wave Movement")]
    public class WaveMovement : XRDynamicHandGesture
    {
        // Threshold for how far the wrist must move horizontally to count as a wave
        [SerializeField] float waveThreshold = 0.005f; // Lowered threshold for sensitivity
        [SerializeField] float resetTime = 1f;
        [SerializeField] float smoothingFactor = 0.5f;   // Increased smoothing for more responsive delta
        [SerializeField] int framesRequired = 1;
        [SerializeField] float jitterEpsilon = 0.001f;

        float lastWristX = 0f;
        bool movedRight = false;
        bool movedLeft = false;
        float lastUpdateTime = 0f;
        int consecutiveRightFrames = 0;
        int consecutiveLeftFrames = 0;
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

            // On first valid update, initialize lastWristX and lastUpdateTime
            if (lastUpdateTime == 0f)
            {
                lastWristX = currentX;
                lastUpdateTime = Time.time;
                return false; // Skip movement detection for the first frame
            }

            // Apply smoothing to reduce jitter
            float smoothedX = Mathf.Lerp(lastWristX, currentX, smoothingFactor);
            float deltaX = smoothedX - lastWristX;

            //Debug.Log($"currentX: {currentX}, lastWristX: {lastWristX}, smoothedX: {smoothedX}, deltaX: {deltaX}");

            // Filter out tiny fluctuations that are likely just noise
            if (Mathf.Abs(deltaX) < jitterEpsilon)
            {
                deltaX = 0f;
            }

            lastWristX = smoothedX; // Update for next frame

            // Reset if too much time has passed since last update
            if (Time.time - lastUpdateTime > resetTime)
            {
                movedRight = false;
                movedLeft = false;
                consecutiveRightFrames = 0;
                consecutiveLeftFrames = 0;
            }

            // Check for right movement: require several consecutive frames over threshold
            if (deltaX > waveThreshold)
            {
                consecutiveRightFrames++;
            }
            else
            {
                consecutiveRightFrames = 0;
            }

            if (consecutiveRightFrames >= framesRequired)
            {
                movedRight = true;
                Debug.Log("Right Movement");
            }

            // Check for left movement only after right movement has been detected
            if (movedRight && deltaX < -waveThreshold)
            {
                consecutiveLeftFrames++;
            }
            else
            {
                consecutiveLeftFrames = 0;
            }

            if (consecutiveLeftFrames >= framesRequired)
            {
                movedLeft = true;
                Debug.Log("Left Movement");
            }

            lastUpdateTime = Time.time;

            // Wave is detected if we have valid right then left movement in sequence
            bool waveDetected = movedRight && movedLeft;
            if (waveDetected)
            {
                Debug.Log("Wave detected!");
            }
            return waveDetected;
        }
    }

}
