using UnityEngine;

public class AltarImageDisplay : MonoBehaviour
{
    [SerializeField] private MeshRenderer targetRenderer;

    private Material runtimeMat;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<MeshRenderer>();

        // Copie runtime du material pour éviter de modifier l'asset
        runtimeMat = new Material(targetRenderer.sharedMaterial);
        targetRenderer.material = runtimeMat;
    }

    public void SetSprite(Sprite sprite)
    {
        if (sprite == null) return;

        runtimeMat.mainTexture = sprite.texture;

        // Ajuster ratio (largeur/hauteur)
        float w = sprite.rect.width;
        float h = sprite.rect.height;
        float aspect = (h <= 0f) ? 1f : (w / h);

        // On scale le quad pour conserver le ratio
        // Ici: hauteur = 1, largeur = aspect
        targetRenderer.transform.localScale = new Vector3(aspect, 1f, 1f);
    }
}
