using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleFillingUI : MonoBehaviour
{
    [SerializeField] private bool fadeAtFull;
    [SerializeField] private float fadeTime = 0.33f;
    [SerializeField] private CanvasGroup fillCG;
    [Space()]
    [SerializeField] private bool useFill;
    [SerializeField] private Image fillImage;
    [SerializeField] private Color fillChargedColor;
    [SerializeField] private Gradient fillChargingGradient;
    [Space()]
    [SerializeField] private bool useLabel;
    [SerializeField] private TMP_Text label;
    [SerializeField] private NormalizedValueFormat normalizingFormat;
    [SerializeField] private string labelPrefix;
    [SerializeField] private string labelSuffix;
    [SerializeField] private bool useLabelColoring;
    [SerializeField] private Color labelChargedColor;
    [SerializeField] private Gradient labelChargingGradient;
    [Space()]
    [SerializeField] private bool useReadyIndicator;
    [SerializeField] private GameObject readyIndicator;
    [SerializeField][Range(0, 1f)] private float readyIndicatorThreshold = 1f;

    private float _fadeTimer;
    private bool _fadeFlag;

    public void UpdateState(float f, float dt, float raw = -1f)
    {
        f = Mathf.Clamp01(f);

        if (fadeAtFull)
        {
            _fadeFlag = f >= 1f;

            if (!_fadeFlag)
            {
                fillCG.alpha = 1f;
                _fadeTimer = 0f;
            } else
            {
                fillCG.alpha = Mathf.Lerp(0, 1f, 1f - (_fadeTimer / fadeTime));
                _fadeTimer = Mathf.Clamp(_fadeTimer + dt, 0, fadeTime);
            }
        }

        if (useFill)
        {
            fillImage.fillAmount = f;
            fillImage.color = f >= 1f ? fillChargedColor : fillChargingGradient.Evaluate(f);
        }

        if (useLabel)
        {
            if (normalizingFormat == NormalizedValueFormat.RawInt)
            {
                label.text = labelPrefix + NormalizedString.Convert(raw, normalizingFormat) + labelSuffix;
            } else
                label.text = labelPrefix + NormalizedString.Convert(f, normalizingFormat) + labelSuffix;

            if (useLabelColoring)
            {
                label.color = f >= 1f ? labelChargedColor : labelChargingGradient.Evaluate(f);
            }
        }

        if (useReadyIndicator)
        {
            readyIndicator.SetActive(f >= readyIndicatorThreshold);
        }
    }
}
