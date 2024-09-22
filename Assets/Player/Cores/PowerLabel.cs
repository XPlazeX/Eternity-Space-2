using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class PowerLabel : MonoBehaviour
{
    private Text _label;
    private string _mwText;

    private void Start() {
        _mwText = SceneLocalizator.GetLocalizedString("Game", 12, 0);

        PlayerCore.LoadMegawatts();
        PlayerCore.EnergyChanged += OnPowerChanged;

        OnPowerChanged(0f);
    }

    private void OnDisable() {
        PlayerCore.EnergyChanged -= OnPowerChanged;
    }

    private void OnPowerChanged(float mw)
    {
        if (_label == null)
            _label = GetComponent<Text>();

        _label.text = $"{SceneStatics.PreferFloatView(PlayerCore.Megawatts)}/{SceneStatics.PreferFloatView(PlayerCore.MaxMegawatts, 1)} {_mwText}";
    }
}
