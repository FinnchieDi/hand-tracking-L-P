using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(TeleportationAnchor))]
public class TeleportAnchorGlowEffect : MonoBehaviour
{
    [Header("Glow Object")]
    public GameObject glowVisual; // Enable/disable this based on hover

    private TeleportationAnchor teleportAnchor;

    void Awake()
    {
        teleportAnchor = GetComponent<TeleportationAnchor>();

        teleportAnchor.hoverEntered.AddListener(OnHoverEntered);
        teleportAnchor.hoverExited.AddListener(OnHoverExited);

        if (glowVisual != null)
            glowVisual.SetActive(false); // Hide glow by default
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (glowVisual != null)
            glowVisual.SetActive(true);
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        if (glowVisual != null)
            glowVisual.SetActive(false);
    }

    void OnDestroy()
    {
        teleportAnchor.hoverEntered.RemoveListener(OnHoverEntered);
        teleportAnchor.hoverExited.RemoveListener(OnHoverExited);
    }

    void Update()
    {
        if (glowVisual.activeSelf)
        {
            float pulse = Mathf.PingPong(Time.time, 1f);
            Material mat = glowVisual.GetComponent<Renderer>().material;
            mat.SetColor("_EmissionColor", Color.cyan * pulse * 2f);
        }
    }
}

