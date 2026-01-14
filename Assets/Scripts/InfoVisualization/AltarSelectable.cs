using UnityEngine;

public class AltarSelectable : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AltarMediaDisplay imageDisplay;
    [SerializeField] private GameObject menu;

    public void Select()
    {
        Debug.Log("ALTARSELECTABLE: Select() called");

        if (menu == null || imageDisplay == null)
        {
            Debug.LogWarning($"ALTARSELECTABLE: null refs menu={menu} imageDisplay={imageDisplay}");
            return;
        }

        Debug.Log("ALTARSELECTABLE: opening menu");
        menu.SetActive(true);
        var panel = menu.transform.Find("AltarMenuPanel");
        if (panel != null) panel.gameObject.SetActive(true);
        Debug.Log($"ALTARSELECTABLE: menu object = {menu.name}, activeSelf={menu.activeSelf}, activeInHierarchy={menu.activeInHierarchy}");
        Debug.Log($"ALTARSELECTABLE: menu path = {GetPath(menu.transform)}");

        var ctrl = menu.GetComponent<SelectMediaMenu>();
        Debug.Log("ALTARSELECTABLE: menu ctrl = " + ctrl);

        if (ctrl != null)
            ctrl.Populate();

        var cg = menu.GetComponentInChildren<CanvasGroup>(true);
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
            Debug.Log("ALTARSELECTABLE: CanvasGroup forced visible");
        }
    }

    private static string GetPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null) { t = t.parent; path = t.name + "/" + path; }
        return path;
    }
}
