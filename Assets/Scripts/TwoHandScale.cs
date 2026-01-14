using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TwoHandScale : MonoBehaviour
{
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    private float initialDistance;
    private Vector3 initialScale;

    private UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor firstInteractor;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor secondInteractor;

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        grabInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (firstInteractor == null)
        {
            firstInteractor = args.interactorObject;
        }
        else if (secondInteractor == null)
        {
            secondInteractor = args.interactorObject;

            initialDistance = Vector3.Distance(
                firstInteractor.transform.position,
                secondInteractor.transform.position
            );

            initialScale = transform.localScale;
        }
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        if (args.interactorObject == firstInteractor)
            firstInteractor = null;

        if (args.interactorObject == secondInteractor)
            secondInteractor = null;
    }

    void Update()
    {
        if (firstInteractor != null && secondInteractor != null)
        {
            float currentDistance = Vector3.Distance(
                firstInteractor.transform.position,
                secondInteractor.transform.position
            );

            float scaleFactor = currentDistance / initialDistance;
            transform.localScale = initialScale * scaleFactor;
        }
    }
}