using UnityEngine;
using UnityEngine.UI;

public class DevModeToggler : MonoBehaviour
{
    public const string dev_pp = "Dev";

    [SerializeField] private Text _targetGraphic;
    [SerializeField] private Color _disabledColor;
    [SerializeField] private Color _enabledColor;

    private void Start() {
        CheckState();
    }

    public void ToggleMod()
    {
        bool enabled = PlayerPrefs.GetInt(dev_pp, 0) == 1;

        if (enabled)
        {
            PlayerPrefs.SetInt(dev_pp, 0);
            GlobalSaveHandler.ClearSave();
            GameSessionInfoHandler.ClearGameSession();
        } else
        {
            PlayerPrefs.SetInt(dev_pp, 1);
        }

        CheckState();
    }

    private void CheckState()
    {
        bool enabled = PlayerPrefs.GetInt(dev_pp, 0) == 1;

        if (enabled)
        {
            _targetGraphic.color = _enabledColor;
            _targetGraphic.text = $"{SceneLocalizator.GetLocalizedString("Intro", 1, 0)}{SceneLocalizator.GetLocalizedString("Intro", 1, 1)}";
        } else
        {
            _targetGraphic.color = _disabledColor;
            _targetGraphic.text = $"{SceneLocalizator.GetLocalizedString("Intro", 1, 0)}{SceneLocalizator.GetLocalizedString("Intro", 1, 2)}";
        }

    }
}
