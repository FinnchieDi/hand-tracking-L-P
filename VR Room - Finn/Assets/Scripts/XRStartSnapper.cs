using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class XRPlayerSnapper : MonoBehaviour
{
    [Header("XR Rig and Spawn Anchor")]
    public Transform xrOrigin;           // XR Origin GameObject
    public Transform targetAnchor;       // Designated spawn point / teleport anchor

    [Header("Optional Manual Reset")]
    public bool enableManualReset = true;
    public KeyCode resetKey = KeyCode.R;

    private Camera xrCamera;

    private void Start()
    {
        if (xrOrigin == null)
            Debug.LogError("XR Origin not assigned in XRPlayerSnapper.");
        
        xrCamera = Camera.main;

        StartCoroutine(SnapToAnchorDelayed());
    }

    private void Update()
    {
        if (enableManualReset && Input.GetKeyDown(resetKey))
        {
            StartCoroutine(SnapToAnchorDelayed());
        }
    }

    IEnumerator SnapToAnchorDelayed()
    {
        // Wait for XR tracking to stabilize
        yield return new WaitForSeconds(0.5f);

        if (xrCamera == null)
        {
            Debug.LogError("Main Camera not found in scene.");
            yield break;
        }

        // Get the current local headset offset within the XR Origin
        Vector3 headsetLocalOffset = xrCamera.transform.localPosition;

        //Debug.Log("XR Origin local offset: " + headsetLocalOffset);

        // Keep the same headset height relative to XR Origin
        Vector3 targetPos = targetAnchor.position - new Vector3(headsetLocalOffset.x, 0f, headsetLocalOffset.z);

        // Snap XR Origin to calculated position
        xrOrigin.position = targetPos;

        // Snap rotation (Y-axis only)
        Quaternion targetRot = Quaternion.Euler(0f, targetAnchor.eulerAngles.y, 0f);
        xrOrigin.rotation = targetRot;

        //Debug.Log("XR Origin snapped to anchor: " + targetAnchor.name);
    }
}

