using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PPSettingPanel : MonoBehaviour
{
    [SerializeField] private string _settingName;
    [SerializeField] private float _defaultValue;
    [SerializeField] private bool _toggler = false;
    [Header("Toggler = false")]
    [SerializeField] private LinkSettingButton[] _buttons;
    [Header("Toggler = true")]
    [SerializeField] private Image _button;
    [SerializeField] private Sprite _enabledSprite;
    [SerializeField] private Sprite _disabledSprite;

    private void Start() {
        ReloadButtons();
    }

    private void ReloadButtons()
    {
        float currentValue = PlayerPrefs.GetFloat(_settingName, _defaultValue);

        if (_toggler)
        {
            if (currentValue == 1)
            {
                _button.sprite = _enabledSprite;
            }
            else
            {
                _button.sprite = _disabledSprite;
            }
        }

        for (int i = 0; i < _buttons.Length; i++)
        {
            if (currentValue == _buttons[i].SettingValue)
            {
                _buttons[i].LinkedImage.sprite = _enabledSprite;
            }
            else
            {
                _buttons[i].LinkedImage.sprite = _disabledSprite;
            }
        }
    }

    public void SetSetting(int id)
    {
        PlayerPrefs.SetFloat(_settingName, _buttons[id].SettingValue);
        PlayerPrefs.Save();

        ReloadButtons();
        //GameObject.FindWithTag("SceneCore").GetComponent<MainSettings>().SetLogState(0);
    }

    public void Toggle()
    {
        float currentValue = PlayerPrefs.GetFloat(_settingName, _defaultValue);

        if (currentValue == 1)
            PlayerPrefs.SetFloat(_settingName, 0);
        else
            PlayerPrefs.SetFloat(_settingName, 1);

        ReloadButtons();
        //GameObject.FindWithTag("SceneCore").GetComponent<MainSettings>().SetLogState(0);
    }

    [System.Serializable]
    private class LinkSettingButton
    {
        [SerializeField] private Image _image;
        [SerializeField] private float _setValue;

        public Image LinkedImage => _image;
        public float SettingValue => _setValue;
    }
}
