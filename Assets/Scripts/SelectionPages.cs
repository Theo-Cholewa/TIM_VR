using System.Collections;
using UnityEngine;

public class SelectionPages : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] private GameObject pageHome;
    [SerializeField] private GameObject pageLibrary;

    [Header("Button")]
    [SerializeField] private GameObject selectButton;
    [SerializeField] private GameObject selectAsset;

    void Start() => ShowHome();

    public void ShowHome()
    {
        pageHome.SetActive(true);
        pageLibrary.SetActive(false);
        if (selectButton != null) selectButton.SetActive(true);
        if (selectAsset != null) selectAsset.SetActive(false);
    }

    public void ShowLibrary()
    {
        pageHome.SetActive(false);
        pageLibrary.SetActive(true);

        if (selectButton != null)
            StartCoroutine(HideButtonEndOfFrame());

        if (selectAsset != null) selectAsset.SetActive(true);
    }

    private IEnumerator HideButtonEndOfFrame()
    {
        // laisse Meta SDK finir le "select" et ses updates visuels
        yield return new WaitForEndOfFrame();
        selectButton.SetActive(false);
    }
}