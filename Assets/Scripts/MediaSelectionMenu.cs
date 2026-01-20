using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MediaSelectionMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform content;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private string thumbnailChildName = "Thumbnail";
    [SerializeField] private bool sortAlphabetically = true;

    [Header("Resources paths (relative to Resources/)")]
    [SerializeField] private string imagesPath = "AltarMedia/Images";
    [SerializeField] private string videosPath = "AltarMedia/Videos";

    public event Action<Texture2D> OnImageSelected;
    public event Action<VideoClip> OnVideoSelected;

    private readonly List<Entry> entries = new();

    private class Entry
    {
        public string label;
        public bool isVideo;
        public Texture thumbnail;   // <-- Texture (Texture2D ou autre)
        public Texture2D image;     // image finale si isVideo=false
        public VideoClip video;     // si isVideo=true
    }

    void Start()
    {
        PopulateNow();
    }

    [ContextMenu("Populate Now")]
    public void PopulateNow()
    {
        entries.Clear();

        // --- IMAGES : Texture2D + Sprite ---
        var imgsTex = Resources.LoadAll<Texture2D>(imagesPath);
        var imgsSpr = Resources.LoadAll<Sprite>(imagesPath);

        Debug.Log($"[MediaSelectionMenu] Images Texture2D={imgsTex.Length} | Sprites={imgsSpr.Length} (Resources/{imagesPath})");

        foreach (var tex in imgsTex)
        {
            entries.Add(new Entry
            {
                label = tex.name,
                isVideo = false,
                thumbnail = tex,
                image = tex
            });
        }

        foreach (var spr in imgsSpr)
        {
            if (spr == null || spr.texture == null) continue;

            // Evite doublon si déjà ajouté via Texture2D
            if (entries.Any(e => !e.isVideo && e.label == spr.name))
                continue;

            entries.Add(new Entry
            {
                label = spr.name,
                isVideo = false,
                thumbnail = spr.texture,
                image = spr.texture
            });
        }

        // --- VIDEOS + SIGNATURES (png) : Texture2D + Sprite ---
        var vids = Resources.LoadAll<VideoClip>(videosPath);

        var vidThumbTex = Resources.LoadAll<Texture2D>(videosPath);
        var vidThumbSpr = Resources.LoadAll<Sprite>(videosPath);

        var thumbByName = new Dictionary<string, Texture>();

        foreach (var t in vidThumbTex)
            thumbByName[t.name] = t;

        foreach (var s in vidThumbSpr)
            if (s != null && s.texture != null)
                thumbByName[s.name] = s.texture;

        Debug.Log($"[MediaSelectionMenu] Videos={vids.Length} | VideoThumbTex={vidThumbTex.Length} | VideoThumbSprites={vidThumbSpr.Length} (Resources/{videosPath})");

        foreach (var clip in vids)
        {
            thumbByName.TryGetValue(clip.name, out var thumb);

            entries.Add(new Entry
            {
                label = clip.name,
                isVideo = true,
                thumbnail = thumb, // null si pas trouvé
                video = clip
            });
        }

        // --- TRI ---
        IEnumerable<Entry> ordered = entries;
        if (sortAlphabetically)
        {
            ordered = entries
                .OrderBy(e => e.isVideo ? 1 : 0)
                .ThenBy(e => e.label);
        }

        // --- UI CLEAR ---
        foreach (Transform child in content)
            Destroy(child.gameObject);

        // --- UI BUILD ---
        foreach (var e in ordered)
        {
            var go = Instantiate(itemPrefab, content);

            var btn = go.GetComponent<Button>();
            if (btn == null)
            {
                Debug.LogError("Item prefab must have a Button on its root.");
                Destroy(go);
                continue;
            }

            var txt = go.GetComponentInChildren<TextMeshProUGUI>(true);
            if (txt != null)
                txt.text = e.isVideo ? $"🎬 {e.label}" : $"🖼️ {e.label}";

            // Thumbnail : enfant "Thumbnail" avec RawImage
            RawImage raw = null;
            var tr = go.transform.Find(thumbnailChildName);
            if (tr != null) raw = tr.GetComponent<RawImage>();
            if (raw == null) raw = go.GetComponentInChildren<RawImage>(true);

            if (raw != null)
            {
                raw.texture = e.thumbnail;
                raw.enabled = (e.thumbnail != null);
                raw.raycastTarget = false;
            }

            btn.onClick.AddListener(() =>
            {
                if (!e.isVideo)
                    OnImageSelected?.Invoke(e.image);
                else
                    OnVideoSelected?.Invoke(e.video);
            });
        }

        if (entries.Count == 0)
            Debug.LogWarning($"[MediaSelectionMenu] No media found. Check Resources paths.");

        var missingVideoThumbs = entries.Where(x => x.isVideo && x.thumbnail == null).Select(x => x.label).ToList();
        if (missingVideoThumbs.Count > 0)
            Debug.LogWarning($"[MediaSelectionMenu] Missing video thumbnail for: {string.Join(", ", missingVideoThumbs)}");
    }
}
