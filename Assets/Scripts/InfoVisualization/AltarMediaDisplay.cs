using UnityEngine;
using UnityEngine.Video;

public class AltarMediaDisplay : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private MeshRenderer targetRenderer;

    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;

    private MaterialPropertyBlock mpb;
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    void Awake()
    {
        mpb = new MaterialPropertyBlock();

        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = true;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        }
    }

    public void ShowImage(Texture2D tex)
    {
        if (tex == null || targetRenderer == null) return;

        if (videoPlayer != null && videoPlayer.isPlaying)
            videoPlayer.Stop();

        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture(MainTex, tex);
        targetRenderer.SetPropertyBlock(mpb);
    }

    public void ShowVideo(VideoClip clip)
    {
        if (clip == null || targetRenderer == null || videoPlayer == null) return;

        if (videoPlayer.targetTexture == null)
        {
            var rt = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGB32);
            rt.Create();
            videoPlayer.targetTexture = rt;
        }

        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture(MainTex, videoPlayer.targetTexture);
        targetRenderer.SetPropertyBlock(mpb);

        videoPlayer.clip = clip;
        videoPlayer.Play();
    }

    public void StopVideo()
    {
        if (videoPlayer != null)
            videoPlayer.Stop();
    }

    public void Clear()
    {
        StopVideo();
        if (targetRenderer == null) return;

        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture(MainTex, null);
        targetRenderer.SetPropertyBlock(mpb);
    }
}
