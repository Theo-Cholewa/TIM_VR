using System.Runtime.InteropServices;
using UnityEngine;

public class RaycastInspector : MonoBehaviour
{
    [Header("Raycast")]
    public Camera cam;
    public float maxDistance = 8f;
    public LayerMask interactMask = ~0;

    [Header("Crosshair")]
    public RectTransform crosshair; // UI crosshair
    public UnityEngine.UI.Image crosshairImage;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.green;

    [Header("UI")]
    public InspectTooltip tooltip;
    private InspectableInfo current;

    void Reset()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (!cam) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactMask, QueryTriggerInteraction.Ignore);

        InspectableInfo info = null;
        if (hitSomething)
        {
            info = hit.collider.GetComponentInParent<InspectableInfo>();
        }

        if (crosshairImage)
        {
            crosshairImage.color = info ? hoverColor : normalColor;
        }

        if (info != null)
        {
            current = info;

            // si la tooltip a été fermée manuellement, on ne la rouvre pas tant qu'on ne change pas de cible
            tooltip.TryShowFor(info);
        }
        else
        {
            current = null;
            tooltip.HideIfNoTarget();
        }

        // Debug.DrawRay(ray.origin, ray.direction * maxDistance, hitSomething ? Color.green : Color.red);
    }
}
