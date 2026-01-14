using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectTooltip : MonoBehaviour
{
    [Header("References")]
    public GameObject panelRoot;
    public TMP_Text titleText;
    public TMP_Text contentText;
    public Button closeButton;

    // Empêche de se ré-ouvrir instantanément après fermeture tant qu'on vise le même objet
    private InspectableInfo lastShown;
    private bool closedManuallyForCurrentTarget;

    void Awake()
    {
        if (closeButton)
            closeButton.onClick.AddListener(Close);

        HideImmediate();
    }

    public void TryShowFor(InspectableInfo info)
    {
        if (!info) return;

        // Si on change de cible, on autorise à réafficher
        if (info != lastShown)
        {
            lastShown = info;
            closedManuallyForCurrentTarget = false;
        }

        if (closedManuallyForCurrentTarget) return;

        Show(info);
    }

    void Show(InspectableInfo info)
    {
        if (!panelRoot) return;
        panelRoot.SetActive(true);

        string title = string.IsNullOrWhiteSpace(info.titleOverride) ? info.gameObject.name : info.titleOverride;
        if (titleText) titleText.text = title;
        if (contentText) contentText.text = info.description;
    }

    public void Close()
    {
        closedManuallyForCurrentTarget = true;
        if (panelRoot) panelRoot.SetActive(false);
    }

    public void HideIfNoTarget()
    {
        // Quand on ne vise rien, on cache ET on reset l'état de fermeture
        lastShown = null;
        closedManuallyForCurrentTarget = false;
        if (panelRoot) panelRoot.SetActive(false);
    }

    void HideImmediate()
    {
        if (panelRoot) panelRoot.SetActive(false);
    }
}
