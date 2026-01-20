using UnityEngine;

public class PlayerHeightOffset : MonoBehaviour
{
    [Tooltip("Taille voulue du joueur en mètres (ex: 1.80)")]
    public float targetHeight = 1.80f;

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (OVRManager.instance != null)
        {
            float realHeight = OVRManager.instance.eyeHeight;
            float offset = targetHeight - realHeight;

            Vector3 pos = transform.localPosition;
            pos.y = offset;
            transform.localPosition = pos;
        }
#endif
    }
}