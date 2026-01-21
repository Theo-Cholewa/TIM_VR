using UnityEngine;
using UnityEngine.Video;

public class FitScreenToVideo : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Transform screenTransform; // ton mesh "Affichage"
    [SerializeField] private bool keepHeight = true;
    [SerializeField] private float referenceSize = 1f; // hauteur ou largeur de référence

    private void Awake()
    {
        if (videoPlayer == null) videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.Prepare();
    }

    private void OnPrepared(VideoPlayer vp)
    {
        float videoW = vp.width;
        float videoH = vp.height;
        if (videoW <= 0 || videoH <= 0 || screenTransform == null) return;

        float aspect = videoW / videoH; // ex: 0.5625 pour 9:16

        Vector3 s = screenTransform.localScale;

        if (keepHeight)
        {
            s.y = referenceSize;        // on fixe la hauteur
            s.x = referenceSize * aspect; // largeur ajustée
        }
        else
        {
            s.x = referenceSize;        // on fixe la largeur
            s.y = referenceSize / aspect; // hauteur ajustée
        }

        screenTransform.localScale = s;
    }
}
