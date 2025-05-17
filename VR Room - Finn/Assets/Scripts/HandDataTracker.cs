using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;

public class HandDataTracker : MonoBehaviour
{
    public XRNode handNode; // Assign LeftHand or RightHand in the Inspector

    private XRHandSubsystem handSubsystem;
    private XRHand trackedHand;

    void Start()
    {
        // Retrieve the active XRHandSubsystem
        List<XRHandSubsystem> handSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetInstances(handSubsystems);

        if (handSubsystems.Count > 0)
        {
            handSubsystem = handSubsystems[0];
            Debug.Log("XRHandSubsystem successfully retrieved.");
        }
        else
        {
            Debug.LogWarning("No XRHandSubsystem found. Ensure XR Hands and OpenXR plugins are installed and configured.");
        }
    }

    void Update()
    {
        if (handSubsystem == null)
            return;

        // Access the appropriate hand based on the XRNode
        trackedHand = handNode == XRNode.LeftHand ? handSubsystem.leftHand : handSubsystem.rightHand;

        if (!trackedHand.isTracked)
            return;

        // Example: Access the Index Tip joint
        XRHandJoint indexTipJoint = trackedHand.GetJoint(XRHandJointID.IndexTip);

        if (indexTipJoint.TryGetPose(out Pose pose))
        {
            // Use pose.position and pose.rotation as needed
            Debug.Log($"{handNode} Index Tip Position: {pose.position}");
        }
    }
}
