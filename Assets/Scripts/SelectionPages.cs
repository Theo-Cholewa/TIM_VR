using UnityEngine;

public class SelectionPages : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] private GameObject pageHome;
    [SerializeField] private GameObject pageLibrary;

    void Start()
    {
        ShowHome();
    }

    public void ShowHome()
    {
        pageHome.SetActive(true);
        pageLibrary.SetActive(false);
    }

    public void ShowLibrary()
    {
        pageHome.SetActive(false);
        pageLibrary.SetActive(true);
    }
}
