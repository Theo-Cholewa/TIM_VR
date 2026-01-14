using UnityEngine;

public class ShowCanvasOnGrab : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody targetRigidbody;
    [SerializeField] private GameObject canvasObject;

    private bool wasKinematic;

    void Awake()
    {
        if (canvasObject != null)
            canvasObject.SetActive(false);

        if (targetRigidbody != null)
            wasKinematic = targetRigidbody.isKinematic;
    }

    void Update()
    {
        if (targetRigidbody == null || canvasObject == null)
            return;

        bool isGrabbed = targetRigidbody.isKinematic;

        // Transition: not grabbed → grabbed
        if (!wasKinematic && isGrabbed)
        {
            canvasObject.SetActive(true);
        }
        // Transition: grabbed → released
        else if (wasKinematic && !isGrabbed)
        {
            canvasObject.SetActive(false);
        }

        wasKinematic = isGrabbed;
    }
}