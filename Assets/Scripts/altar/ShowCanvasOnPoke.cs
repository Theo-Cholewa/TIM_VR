using UnityEngine;

public class ShowCanvasOnPoke : MonoBehaviour
{
    [SerializeField] private Canvas targetCanvas;

    void Awake()
    {
        if (targetCanvas != null)
            targetCanvas.gameObject.SetActive(false);
    }

    // Appelé par UnityEvent
    public void OnPokeEnter()
    {
        if (targetCanvas != null)
            targetCanvas.gameObject.SetActive(true);
    }

    public void OnPokeExit()
    {
        if (targetCanvas != null)
            targetCanvas.gameObject.SetActive(false);
    }
}