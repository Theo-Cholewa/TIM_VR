using UnityEngine;

public class TwoHandScaleMeta : MonoBehaviour
{
    [Header("Assign in Inspector (from your Meta rig)")]
    public Transform leftPoint;   // Left hand/controller transform
    public Transform rightPoint;  // Right hand/controller transform

    [Header("Activation")]
    [Tooltip("If true, scaling runs. You enable it when object is grabbed with 2 hands.")]
    public bool scalingEnabled = false;

    [Header("Scale Limits")]
    public float minUniformScale = 0.1f;
    public float maxUniformScale = 5f;

    private float initialDistance;
    private Vector3 initialScale;

    public void BeginTwoHandScale()
    {
        if (leftPoint == null || rightPoint == null) return;

        initialDistance = Vector3.Distance(leftPoint.position, rightPoint.position);
        initialScale = transform.localScale;
        scalingEnabled = true;
    }

    public void EndTwoHandScale()
    {
        scalingEnabled = false;
    }

    void Update()
    {
        if (!scalingEnabled) return;
        if (leftPoint == null || rightPoint == null) return;
        if (initialDistance <= 0.0001f) return;

        float currentDistance = Vector3.Distance(leftPoint.position, rightPoint.position);
        float factor = currentDistance / initialDistance;

        // Uniform scale (avoid stretching)
        float target = Mathf.Clamp(initialScale.x * factor, minUniformScale, maxUniformScale);
        transform.localScale = new Vector3(target, target, target);
    }
}