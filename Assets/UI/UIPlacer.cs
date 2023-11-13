using UnityEngine;
using UnityEngine.UI;

public class UIPlacer : MonoBehaviour
{
    [SerializeField] private RectTransform _canvas;

    public void SpawnUI(RectTransform rectTransform, Vector2 position)
    {
        RectTransform tmp = Instantiate(rectTransform);
        tmp.SetParent(_canvas);
        tmp.transform.localScale = Vector3.one;

        tmp.anchoredPosition = position;
    }

}
