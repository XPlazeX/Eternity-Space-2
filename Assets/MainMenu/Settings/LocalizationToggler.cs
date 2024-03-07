using UnityEngine;

public class LocalizationToggler : TogglerButton
{
    [SerializeField] private string _langCode;
    [SerializeField] private TextLocalizer[] _callingLocalizers;
    [SerializeField] private LocalizationToggler[] _otherTogglers;

    protected override void Start()
    {
        if (!PlayerPrefs.HasKey("Language"))
        {
            PlayerPrefs.SetString("Language", "RU");
        }

        if (PlayerPrefs.GetString("Language", "RU") == _langCode)
        {
            Toggle();
        }
    }

    public void CheckState()
    {
        if (!PlayerPrefs.HasKey("Language"))
        {
            PlayerPrefs.SetString("Language", "RU");
        }

        if (PlayerPrefs.GetString("Language", "RU") != _langCode && ON)
        {
            Toggle();
        }
    }

    public override void Toggle()
    {
        base.Toggle();

        if (ON)
        {
            PlayerPrefs.SetString("Language", _langCode);

            SceneLocalizator.Reload();

            for (int i = 0; i < _callingLocalizers.Length; i++)
            {
                _callingLocalizers[i].HandLocalize();
            }

            for (int i = 0; i < _otherTogglers.Length; i++)
            {
                _otherTogglers[i].CheckState();
            }

        }
    }
}
