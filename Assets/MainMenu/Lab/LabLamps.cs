using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LabLamps : MonoBehaviour
{
    [SerializeField] private RectTransform _parentRect;
    [SerializeField] private RectTransform _lampRect;
    [SerializeField] private float _yPosition;
    [SerializeField] private Sprite _disabledSprite;
    [SerializeField] private Sprite _enabledSprite;

    private List<RectTransform> LampPool = new List<RectTransform>();

    public void PlaceLamps(LabCategory labCategory)
    {
        for (int i = 0; i < LampPool.Count; i++)
        {
            Destroy(LampPool[i].gameObject);
        }
        LampPool.Clear();

        int count = labCategory.Blueprints.Length;
        float parentWidth = (_parentRect.rect.width) - 50f;

        float stepX = (parentWidth / count);
        float startXPosition = -stepX * ((float)count - 1f) / 2f;

        for (int i = 0; i < count; i++)
        {
            RectTransform lamp = Instantiate(_lampRect);

            lamp.transform.SetParent(_parentRect);
            lamp.transform.localScale = Vector3.one;

            lamp.anchoredPosition = new Vector2(startXPosition + (stepX * i), _yPosition);

            if (Unlocks.HasUnlock(labCategory.Blueprints[i].handingID))
                lamp.GetComponent<Image>().sprite = _enabledSprite;

            else
                lamp.GetComponent<Image>().sprite = _disabledSprite;

            LampPool.Add(lamp);
        }
    }

}
