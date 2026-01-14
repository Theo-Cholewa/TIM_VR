using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class SelectMediaMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform content;          // ScrollView/Viewport/Content
    [SerializeField] private GameObject itemPrefab;      // prefab bouton
    [SerializeField] private Texture2D videoPlaceholder; // icône si pas de poster (optionnel)

    [Header("Target")]
    [SerializeField] private AltarMediaDisplay altarDisplay;

    private readonly List<GameObject> _spawned = new();

    private void OnEnable()
    {
        Populate();
    }

    public void Populate()
    {
        Clear();

        // 1) Images
        var images = Resources.LoadAll<Texture2D>("AltarMedia/Images");
        foreach (var tex in images)
        {
            CreateItem(
                preview: tex,
                label: tex.name,
                onClick: () => altarDisplay.ShowImage(tex)
            );
        }

        // 2) Videos
        var videos = Resources.LoadAll<VideoClip>("AltarMedia/Videos");
        foreach (var clip in videos)
        {
            // Poster = image même nom dans le même dossier
            Texture2D poster = Resources.Load<Texture2D>($"AltarMedia/Videos/{clip.name}");
            Texture2D preview = poster != null ? poster : videoPlaceholder;

            CreateItem(
                preview: preview,
                label: clip.name,
                onClick: () => altarDisplay.PlayVideo(clip)
            );
        }
    }

    private void CreateItem(Texture2D preview, string label, Action onClick)
    {
        var go = Instantiate(itemPrefab, content);
        _spawned.Add(go);

        // Button
        var btn = go.GetComponent<Button>();
        if (btn == null) btn = go.GetComponentInChildren<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => onClick());

        // RawImage preview
        var raw = go.GetComponentInChildren<RawImage>(true);
        if (raw != null)
            raw.texture = preview;

        // Text label (optional)
        var tmp = go.GetComponentInChildren<TMP_Text>(true);
        if (tmp != null)
            tmp.text = label;
    }

    private void Clear()
    {
        foreach (var go in _spawned)
            if (go != null) Destroy(go);
        _spawned.Clear();
    }
}
