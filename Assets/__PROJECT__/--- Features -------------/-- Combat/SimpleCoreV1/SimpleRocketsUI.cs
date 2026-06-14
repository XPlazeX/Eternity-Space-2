using UnityEngine;
using UnityEngine.UI;

public class SimpleRocketsUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup visualGroup;
    [SerializeField] private float dimAlpha = 0.3f;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject readyIndicator;

    private SimpleRocketsDevice _rocketsDevice;

    private void Start() 
    {
        _rocketsDevice = FindAnyObjectByType<SimpleRocketsDevice>();
    }

    private void Update()
    {
        if (_rocketsDevice == null)
        {
            _rocketsDevice = FindAnyObjectByType<SimpleRocketsDevice>();

            if (_rocketsDevice == null)
                return;
        }

        float charge = _rocketsDevice.GetChargeNormalized();
        bool ready = charge >= 1f;

        if (fillImage != null)
            fillImage.fillAmount = charge;

        if (readyIndicator != null)
            readyIndicator.SetActive(ready);

        if (visualGroup != null)
            visualGroup.alpha = ready ? 1f : dimAlpha;
    }
}
