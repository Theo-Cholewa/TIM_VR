using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AltarImageMenu : MonoBehaviour
{
    [Header("Resources path (without Assets/Resources)")]
    [SerializeField] private string resourcesPath = "AltarImages";

    [Header("UI")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Transform contentParent;
    [SerializeField] private Button buttonPrefab;

    private List<Sprite> sprites = new();
    private AltarImageDisplay currentTarget;

    private void Awake()
    {
        panelRoot.SetActive(false);
        LoadSprites();
        BuildButtons();
    }

    private void LoadSprites()
    {
        sprites.Clear();
        sprites.AddRange(Resources.LoadAll<Sprite>(resourcesPath));
    }

    private void BuildButtons()
    {
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        foreach (var s in sprites)
        {
            var btn = Instantiate(buttonPrefab, contentParent);
            var img = btn.GetComponentInChildren<Image>();
            if (img != null) img.sprite = s;

            btn.onClick.AddListener(() =>
            {
                currentTarget?.SetSprite(s);
                Close();
            });
        }
    }

    public void Open(AltarImageDisplay target)
    {
        currentTarget = target;
        panelRoot.SetActive(true);
    }

    public void Close()
    {
        panelRoot.SetActive(false);
        currentTarget = null;
    }
}
