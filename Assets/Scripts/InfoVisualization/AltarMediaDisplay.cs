using UnityEngine;
using UnityEngine.Video;

public class AltarMediaDisplay : MonoBehaviour
{
    [Header("Quad")]
    [SerializeField] private MeshRenderer quadRenderer;
    [SerializeField] private string textureProperty = "_MainTex"; // Unlit/Standard
    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RenderTexture videoTexture;

    private Material _mat;

    private void Awake()
    {
        _mat = quadRenderer.material;
        if (videoPlayer != null) videoPlayer.Stop();
    }

    public void ShowImage(Texture tex)
    {
        if (videoPlayer != null) videoPlayer.Stop();
        _mat.SetTexture(textureProperty, tex);
    }

    public void PlayVideo(VideoClip clip, bool loop = true)
    {
        if (clip == null) return;

        videoPlayer.Stop();
        videoPlayer.isLooping = loop;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = videoTexture;
        videoPlayer.clip = clip;

        _mat.SetTexture(textureProperty, videoTexture);
        videoPlayer.Play();
    }
}
