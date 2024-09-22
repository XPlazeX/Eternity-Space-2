using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Button _controlButton;
    [SerializeField] private Image _fillingConture;
    [SerializeField] private Image _fillingIcon;
    [SerializeField] private Image _bgIcon;
    [SerializeField] private Text _mwLabel;
    [Space()]
    [SerializeField] private Color _notEnoughtColor;
    [SerializeField] private Color _enoughtColor;
    [SerializeField] private Color _maxEnergyColor;

    public Button ControlButton => _controlButton;

    private float _maxEnergy;
    private float _neededEnergy;
    private string _mwText;

    private void OnEnable() 
    {
        _mwText = SceneLocalizator.GetLocalizedString("Game", 12, 0);

        _maxEnergy = PlayerCore.MaxMegawatts;

        PlayerCore.EnergyChanged += SetMegawatts;
        SetMegawatts(PlayerCore.Megawatts);

        ToggleInteractable(false);
    }

    private void OnDisable() {
        PlayerCore.EnergyChanged -= SetMegawatts;
    }

    public void SetStats(Ability ability)
    {
        _fillingIcon.sprite = ability.Icon;
        _bgIcon.sprite = ability.Icon;

        _neededEnergy = ability.EnergyConsume;

        _fillingConture.color = SceneStatics.GetClassColor(Player.Class);

        SetMegawatts(PlayerCore.Megawatts);
    }

    public void ToggleInteractable(bool tog)
    {
        _controlButton.interactable = tog;
    }

    public void SetFillIcon(float f)
    {
        _fillingIcon.fillAmount = f;
    }

    public void SetMegawatts(float mw)
    {
        float contureFill = Mathf.Clamp01(mw / _neededEnergy);
        _fillingConture.fillAmount = contureFill;

        //sb.Append()

        _mwLabel.text = $"{SceneStatics.PreferFloatView(mw)}/{SceneStatics.PreferFloatView(_neededEnergy, 1)} {_mwText}";

        if (mw < _neededEnergy)
        {
            _mwLabel.color = _notEnoughtColor;
        } else if (mw >= _maxEnergy - 0.01f)
        {
            _mwLabel.color = _maxEnergyColor;
        } else
        {
            _mwLabel.color = _enoughtColor;
        }
    }

    public void Hide()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
    }
}
