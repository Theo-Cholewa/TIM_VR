using System.Collections;
using UnityEngine;

public class OVRHeightOffset : MonoBehaviour
{
    [SerializeField] private OVRCameraRig rig;
    [SerializeField] private float extraHeightMeters = 0.2f;

    private IEnumerator Start()
    {
        if (rig == null) rig = GetComponentInChildren<OVRCameraRig>();
        if (rig == null) yield break;

        // attendre que le tracking/OVR soit bien initialisé
        yield return null;
        yield return new WaitForEndOfFrame();

        Transform ts = rig.trackingSpace;
        Vector3 p = ts.localPosition;
        p.y += extraHeightMeters;     // += (pas =) si OVR met déjà quelque chose
        ts.localPosition = p;
    }
}
