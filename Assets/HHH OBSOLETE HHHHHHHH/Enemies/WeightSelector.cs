using UnityEngine;

public class WeightSelector : MonoBehaviour
{
    [SerializeField] private AnimationCurve _weightProbability;
    [SerializeField] private RectTransform _iconType;

    public void Initialize()
    {
        Transform canvas = GameObject.FindWithTag("PassiveUI").GetComponent<Transform>();
        RectTransform _iconRect = Instantiate(_iconType);

        _iconRect.transform.SetParent(canvas);
        _iconRect.anchoredPosition = Vector2.zero;
        _iconRect.transform.localScale = Vector3.one;
    }

    public float GetWeight()
    {
        float curveValue = _weightProbability.Evaluate(Random.value);

        return curveValue;
    }
}
