using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.InputSystem;

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

    [Header("Debug")]
    [Tooltip("Appuie sur S pour jouer automatiquement la première vidéo trouvée.")]
    [SerializeField] private bool debugPlayFirstVideoOnS = true;

    public event Action<Texture2D> OnImageSelected;
    public event Action<VideoClip> OnVideoSelected;

    private readonly List<Entry> entries = new();
    private VideoClip[] cachedVideos = Array.Empty<VideoClip>();

    private class Entry
    {
        public string label;
        public bool isVideo;
        public Texture thumbnail;
        public Texture2D image;
        public VideoClip video;
    }

    void Start()
    {
        PopulateNow();
    }

    void Update()
    {
        if (!debugPlayFirstVideoOnS) return;

        if (Keyboard.current != null && Keyboard.current.sKey.wasPressedThisFrame)
        {
            PlayFirstVideoDebug();
        }
    }

    private void PlayFirstVideoDebug()
    {
        // Recharge au cas où, mais utilise aussi le cache
        if (cachedVideos == null || cachedVideos.Length == 0)
            cachedVideos = Resources.LoadAll<VideoClip>(videosPath);

        if (cachedVideos == null || cachedVideos.Length == 0)
        {
            Debug.LogWarning($"[MediaSelectionMenu] Debug S: no videos found in Resources/{videosPath}");
            return;
        }

        var first = cachedVideos
            .OrderBy(v => v.name) // "première" déterministe
            .FirstOrDefault();

        if (first == null)
        {
            Debug.LogWarning($"[MediaSelectionMenu] Debug S: first video is null (unexpected)");
            return;
        }

        Debug.Log($"[MediaSelectionMenu] Debug S: playing first video = {first.name}");
        OnVideoSelected?.Invoke(first);
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
            if (entries.Any(e => !e.isVideo && e.label == spr.name)) continue;

            entries.Add(new Entry
            {
                label = spr.name,
                isVideo = false,
                thumbnail = spr.texture,
                image = spr.texture
            });
        }

        // --- VIDEOS + SIGNATURES ---
        cachedVideos = Resources.LoadAll<VideoClip>(videosPath);

        var vidThumbTex = Resources.LoadAll<Texture2D>(videosPath);
        var vidThumbSpr = Resources.LoadAll<Sprite>(videosPath);

        var thumbByName = new Dictionary<string, Texture>();

        foreach (var t in vidThumbTex)
            thumbByName[t.name] = t;

        foreach (var s in vidThumbSpr)
            if (s != null && s.texture != null)
                thumbByName[s.name] = s.texture;

        Debug.Log($"[MediaSelectionMenu] Videos={cachedVideos.Length} | VideoThumbTex={vidThumbTex.Length} | VideoThumbSprites={vidThumbSpr.Length} (Resources/{videosPath})");

        foreach (var clip in cachedVideos)
        {
            thumbByName.TryGetValue(clip.name, out var thumb);

            entries.Add(new Entry
            {
                label = clip.name,
                isVideo = true,
                thumbnail = thumb,
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
            var go = Instantiate(itemPrefab);
            go.transform.SetParent(content, false);

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
                Debug.Log($"[MediaSelectionMenu] CLICK: {(e.isVideo ? "VIDEO" : "IMAGE")} {e.label}");

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
