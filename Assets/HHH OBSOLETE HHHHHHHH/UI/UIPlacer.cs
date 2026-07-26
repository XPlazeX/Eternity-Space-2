using UnityEngine;
using UnityEngine.UI;

public class UIPlacer : MonoBehaviour
{
    [SerializeField] private RectTransform _passiveCanvas;
    [SerializeField] private RectTransform _interactiveCanvas;
    [SerializeField] private int _interactiveSiblingIndex;

    public void PlacePassiveUI(RectTransform rectTransform, Vector2 position)
    {
        RectTransform tmp = Instantiate(rectTransform);
        tmp.SetParent(_passiveCanvas);
        tmp.transform.localScale = Vector3.one;

        tmp.anchoredPosition = position;
    }

    public RectTransform PlaceInteractiveUI(RectTransform rectTransform, Vector2 position, bool panel = false)
    {
        RectTransform tmp = Instantiate(rectTransform);
        tmp.SetParent(_interactiveCanvas);
        tmp.SetSiblingIndex(_interactiveSiblingIndex);
        tmp.transform.localScale = Vector3.one;

        tmp.anchoredPosition = position;
        if (panel)
        {
            tmp.offsetMin = new Vector2(-5f, -5f);
            tmp.offsetMax = new Vector2(5f, 5f);
        }

        return tmp;
    }

}
