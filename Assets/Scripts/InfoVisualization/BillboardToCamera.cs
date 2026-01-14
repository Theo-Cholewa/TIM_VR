using UnityEngine;

public class BillboardToCamera : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool lockY = true;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (targetCamera == null) return;

        Vector3 dir = transform.position - targetCamera.transform.position;

        if (lockY)
        {
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) return;
        }

        transform.rotation = Quaternion.LookRotation(dir);
    }
}
