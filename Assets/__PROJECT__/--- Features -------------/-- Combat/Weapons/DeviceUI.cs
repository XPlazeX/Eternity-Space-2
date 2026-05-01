using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeviceUI : MonoBehaviour
{
    [SerializeField] private Image _backFill;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Animator _deviceUIAnimator;
    [SerializeField] private Color _backColor;
    [SerializeField] private Color _notChargedColor;
    [SerializeField] private Color _chargedColor;

    private bool _charged = false;

    private void Start() {
        _fillImage.color = _notChargedColor;
        Fill(0);
    }

    public void Fill(float part)
    {
        if (part >= 1 && !_charged)
            ToggleCharge(true);

        else if (part < 1 && _charged)
            ToggleCharge(false);

        _fillImage.fillAmount = part;
    }

    private void ToggleCharge(bool tog)
    {
        if (tog)
        {
            _deviceUIAnimator.SetTrigger("DeviceCharged");
            _fillImage.color = _chargedColor;
        }
        else 
        {
            _deviceUIAnimator.SetTrigger("UseDevice");
            _fillImage.color = _notChargedColor;
        }

        _charged = tog;
    }

    public void Hide() => _backFill.gameObject.SetActive(false);
}
