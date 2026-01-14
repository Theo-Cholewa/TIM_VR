using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ClickRaycaster : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float maxDistance = 100f;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null)
        {
            Debug.Log("CLICKRAYCASTER: no mouse");
            return;
        }

        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Debug.Log("CLICKRAYCASTER: click detected");

        if (cam == null)
        {
            Debug.LogWarning("CLICKRAYCASTER: Camera is null. Tag your camera MainCamera or assign it.");
            return;
        }

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePos);

        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            Debug.Log($"CLICKRAYCASTER: hit {hit.collider.name} (root={hit.collider.transform.root.name})");

            var altar = hit.collider.GetComponentInParent<AltarSelectable>();
            if (altar != null)
            {
                Debug.Log("CLICKRAYCASTER: ALTAR found -> Select()");
                altar.Select();
            }
            else
            {
                Debug.Log("CLICKRAYCASTER: no AltarSelectable in parents");
            }
        }
        else
        {
            Debug.Log("CLICKRAYCASTER: no hit");
        }
    }
}
