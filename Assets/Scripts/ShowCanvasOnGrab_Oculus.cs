using System.Collections;
using System.Reflection;
using UnityEngine;

public class ShowCanvasOnGrab_Oculus : MonoBehaviour
{
    [SerializeField] private Component grabbable;     // Oculus.Interaction.Grabbable
    [SerializeField] private GameObject canvasObject;
    [SerializeField] private bool debug = false;

    PropertyInfo selectingPointsProp;

    void Awake()
    {
        if (canvasObject != null)
            canvasObject.SetActive(false);

        if (grabbable != null)
        {
            selectingPointsProp = grabbable.GetType().GetProperty(
                "SelectingPoints",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );
        }
    }

    void Update()
    {
        if (grabbable == null || canvasObject == null || selectingPointsProp == null)
            return;

        object val = selectingPointsProp.GetValue(grabbable);
        int count = CountEnumerable(val);

        bool isGrabbed = count > 0;
    
        if (canvasObject.activeSelf != isGrabbed)
            canvasObject.SetActive(isGrabbed);
            Debug.Log($"Canvas activeSelf={canvasObject.activeSelf} activeInHierarchy={canvasObject.activeInHierarchy} name={canvasObject.name}");

        if (debug)
            Debug.Log($"[ShowCanvasOnGrab_Oculus] SelectingPoints={count} grabbed={isGrabbed}");
    }

    private int CountEnumerable(object obj)
    {
        if (obj == null) return 0;
        if (obj is ICollection col) return col.Count;

        // fallback IEnumerable
        if (obj is System.Collections.IEnumerable en)
        {
            int c = 0;
            foreach (var _ in en) c++;
            return c;
        }
        return 0;
    }
}