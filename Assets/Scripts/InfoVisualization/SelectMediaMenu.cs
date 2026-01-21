using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class SelectMediaMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private Transform content;
    [SerializeField] private GameObject itemButtonPrefab;
    [SerializeField] private Button closeButton;

    [Header("Menu placement")]
    [SerializeField] private Transform xrCamera;
    [SerializeField] private float distanceFromCamera = 1.2f;

    [Header("Thumbnails")]
    [Tooltip("Optionnel: texture affichée si une vidéo n'a pas de png signature.")]
    [SerializeField] private Texture2D videoThumbnailFallback;
    [Tooltip("Nom exact de l'enfant contenant le RawImage (d'après ton prefab : 'Thumbnail').")]
    [SerializeField] private string thumbnailChildName = "Thumbnail";

    [Header("Target")]
    [SerializeField] private AltarMediaDisplay altarDisplay;

    private bool built = false;
    private bool isOpen = false;

    private class Entry
    {
        public string label;
        public Texture2D image;
        public VideoClip video;
        public Texture2D thumbnail; // <-- NEW : thumbnail final (image ou signature video)
        public bool IsVideo => video != null;
    }

    private List<Entry> entries = new();

    void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        if (rootCanvas != null)
            rootCanvas.gameObject.SetActive(false);

        isOpen = false;
    }

    public void Show()
    {
        if (isOpen) return;
        isOpen = true;

        BuildOnce();
        //PlaceInFrontOfCamera();

        if (rootCanvas != null)
            rootCanvas.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (!isOpen) return;
        isOpen = false;

        if (rootCanvas != null)
            rootCanvas.gameObject.SetActive(false);
    }
/*
    private void PlaceInFrontOfCamera()
    {
        if (xrCamera == null || rootCanvas == null) return;

        Vector3 forward = new Vector3(xrCamera.forward.x, 0f, xrCamera.forward.z);
        if (forward.sqrMagnitude < 0.0001f)
            forward = xrCamera.forward;

        forward.Normalize();

        rootCanvas.transform.position = xrCamera.position + forward * distanceFromCamera;
        rootCanvas.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }*/

    private void BuildOnce()
    {
        if (built) return;
        built = true;

        // 1) Images
        var imgs = Resources.LoadAll<Texture2D>("AltarMedia/Images");

        // 2) Videos + leurs signatures PNG dans le MEME dossier Resources/AltarMedia/Videos
        var vids = Resources.LoadAll<VideoClip>("AltarMedia/Videos");
        var videoThumbs = Resources.LoadAll<Texture2D>("AltarMedia/Videos");

        // Dictionnaire : nom -> Texture2D (signature)
        var thumbByName = new Dictionary<string, Texture2D>();
        foreach (var t in videoThumbs)
        {
            // si doublons, le dernier gagne (rare)
            thumbByName[t.name] = t;
        }

        // Build entries
        var imageEntries = imgs.Select(t => new Entry
        {
            label = t.name,
            image = t,
            thumbnail = t
        });

        var videoEntries = vids.Select(v =>
        {
            // On cherche la signature png du même nom
            thumbByName.TryGetValue(v.name, out var sig);

            return new Entry
            {
                label = v.name,
                video = v,
                thumbnail = sig != null ? sig : videoThumbnailFallback
            };
        });

        entries = imageEntries
            .Concat(videoEntries)
            .OrderBy(e => e.IsVideo ? 1 : 0)
            .ThenBy(e => e.label)
            .ToList();

        // Nettoie Content
        foreach (Transform child in content)
            Destroy(child.gameObject);

        // Instancie un bouton par entry
        foreach (var e in entries)
        {
            var go = Instantiate(itemButtonPrefab, content);

            var btn = go.GetComponent<Button>();
            if (btn == null)
            {
                Debug.LogError("Media item prefab must have a Button component at root.");
                Destroy(go);
                continue;
            }

            // Texte
            var txt = go.GetComponentInChildren<TextMeshProUGUI>(true);
            if (txt != null)
                txt.text = e.IsVideo ? $"🎬 {e.label}" : $"🖼️ {e.label}";

            // Thumbnail : enfant "Thumbnail" avec RawImage
            RawImage rawThumb = null;
            var thumbTr = go.transform.Find(thumbnailChildName);
            if (thumbTr != null)
                rawThumb = thumbTr.GetComponent<RawImage>();

            if (rawThumb == null)
                rawThumb = go.GetComponentInChildren<RawImage>(true);

            if (rawThumb != null)
            {
                rawThumb.texture = e.thumbnail;

                // si pas de signature ET pas de fallback -> on cache la vignette
                rawThumb.enabled = (e.thumbnail != null);

                // important : évite que le RawImage "mange" le clic du Button
                rawThumb.raycastTarget = false;
            }

            btn.onClick.AddListener(() =>
            {
                if (altarDisplay == null) return;

                if (e.IsVideo) altarDisplay.ShowVideo(e.video);
                else altarDisplay.ShowImage(e.image);

                Hide();
            });
        }

        // Bonus debug utile : liste les vidéos sans signature
        var missing = entries.Where(x => x.IsVideo && x.thumbnail == null).Select(x => x.label).ToList();
        if (missing.Count > 0)
            Debug.LogWarning($"Video signature missing for: {string.Join(", ", missing)}");
    }
}
