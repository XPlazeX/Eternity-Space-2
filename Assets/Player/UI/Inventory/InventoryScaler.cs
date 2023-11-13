using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScaler : MonoBehaviour
{
    const float bordWidth = 100f;

    [SerializeField] private RectTransform _contentRect;
    [SerializeField] private RectTransform _leftRect;
    [SerializeField] private RectTransform _middleRect;
    [SerializeField] private RectTransform _bord1;
    [SerializeField] private RectTransform _bord2;

    private Vector2 _canvasSize;

    private void Start() {
        Initialize();
    }

    public void Initialize()
    {
        _canvasSize = GameObject.FindWithTag("Canvas").GetComponent<RectTransform>().sizeDelta;

        _leftRect.sizeDelta = _canvasSize;
        _middleRect.sizeDelta = _canvasSize;

        _contentRect.sizeDelta = new Vector2(_canvasSize.x * 2f + bordWidth, _canvasSize.y);
        _contentRect.anchoredPosition = new Vector2(-(_canvasSize.x * 1.5f + bordWidth), _canvasSize.y * 0.5f);

        _leftRect.anchoredPosition = new Vector2(_canvasSize.x * 0.5f, 0f);
        _middleRect.anchoredPosition = new Vector2(_canvasSize.x * 1.5f + bordWidth, 0f);

        _bord1.anchoredPosition = new Vector2(_canvasSize.x + (bordWidth * 0.5f), 0f);
        _bord2.anchoredPosition = new Vector2((_canvasSize.x * 2f) + (bordWidth * 1.5f), 0f); 
    }

    public void AddRightRect(RectTransform rightRect)
    {
        _contentRect.sizeDelta = new Vector2(_canvasSize.x * 3f + bordWidth * 2f, _canvasSize.y);
        _contentRect.anchoredPosition = new Vector2(-(_canvasSize.x * 2.5f + bordWidth * 2f), _canvasSize.y * 0.5f);

        rightRect.transform.SetParent(_contentRect.transform);
        rightRect.transform.localScale = Vector3.one;

        rightRect.sizeDelta = _canvasSize;
        rightRect.anchoredPosition = new Vector2(_canvasSize.x * 2.5f + bordWidth * 2f, 0f);
    }
}
