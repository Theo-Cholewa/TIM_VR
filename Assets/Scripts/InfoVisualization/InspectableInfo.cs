using UnityEngine;

public class InspectableInfo : MonoBehaviour
{
    private const int MaxDescriptionLength = 100;

    [Header("tooltip content")]
    public string titleOverride;
    [TextArea(3, 10)]
    public string description;

    private void OnValidate()
    {
        if (!string.IsNullOrEmpty(description) && description.Length > MaxDescriptionLength)
        {
            description = description.Substring(0, MaxDescriptionLength);
        }
    }
}
