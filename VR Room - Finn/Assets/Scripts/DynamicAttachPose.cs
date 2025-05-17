using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DynamicAttachPose : MonoBehaviour
{
    public Transform controllerAttachPoint;
    public Transform handAttachPoint;

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        var interactorGO = args.interactorObject.transform.gameObject;

        if (interactorGO.name.Contains("Hand") || interactorGO.GetComponent<XRDirectInteractor>() != null)
        {
            grabInteractable.attachTransform = handAttachPoint;
        }
        else
        {
            grabInteractable.attachTransform = controllerAttachPoint;
        }
    }
}

