using UnityEngine;
using UnityEngine.Video;

public class MediaDisplayReceiver : MonoBehaviour
{
    [Header("Link")]
    [SerializeField] private MediaSelectionMenu selectionMenu;

    [Header("Target display")]
    [SerializeField] private MeshRenderer targetRenderer;

    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;

    private MaterialPropertyBlock mpb;

    private static readonly int BaseMap = Shader.PropertyToID("_BaseMap"); // URP Lit
    private static readonly int MainTex = Shader.PropertyToID("_MainTex"); // Standard / autres

    void Awake()
    {
        mpb = new MaterialPropertyBlock();

        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = false;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;

            // IMPORTANT : laisse targetTexture null, le script la crée si besoin
        }
    }

    void OnEnable()
    {
        if (selectionMenu == null) return;
        selectionMenu.OnImageSelected += HandleImage;
        selectionMenu.OnVideoSelected += HandleVideo;
    }

    void OnDisable()
    {
        if (selectionMenu == null) return;
        selectionMenu.OnImageSelected -= HandleImage;
        selectionMenu.OnVideoSelected -= HandleVideo;
    }

    private int GetTexPropertyId()
    {
        if (targetRenderer == null || targetRenderer.sharedMaterial == null)
            return BaseMap;

        // si le shader a _BaseMap (URP), on utilise ça
        if (targetRenderer.sharedMaterial.HasProperty(BaseMap))
            return BaseMap;

        // sinon fallback _MainTex
        return MainTex;
    }

    private void SetTextureOnScreen(Texture tex)
    {
        if (targetRenderer == null) return;

        int prop = GetTexPropertyId();
        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture(prop, tex);
        targetRenderer.SetPropertyBlock(mpb);
    }

    private void HandleImage(Texture2D tex)
    {
        if (tex == null) return;

        if (videoPlayer != null && videoPlayer.isPlaying)
            videoPlayer.Stop();

        SetTextureOnScreen(tex);
    }

    private void HandleVideo(VideoClip clip)
    {
        if (clip == null || videoPlayer == null) return;

        if (videoPlayer.targetTexture == null)
        {
            var rt = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGB32);
            rt.Create();
            videoPlayer.targetTexture = rt;
        }

        SetTextureOnScreen(videoPlayer.targetTexture);

        videoPlayer.clip = clip;
        videoPlayer.Play();
    }
}
