using UnityEngine;

public class ScalableObject : MonoBehaviour
{
    public float minScale = 0.3f;
    public float maxScale = 2.5f;

    private Vector3 _baseScale;

    private void Awake()
    {
        _baseScale = transform.localScale;
    }

    public void SetScale01(float t01)
    {
        float s = Mathf.Lerp(minScale, maxScale, t01);
        transform.localScale = _baseScale * s;
    }
}