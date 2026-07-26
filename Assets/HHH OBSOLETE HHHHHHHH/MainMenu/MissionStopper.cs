using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MissionStopper : MonoBehaviour
{
    [SerializeField] private Button _launchButton;
    [SerializeField] private TogglerButton[] _togButtons;
    [SerializeField] private Text _locationTerminal;
    [SerializeField] private Color _dangerColor;
    [SerializeField] private float _dangerTimeCycle;
    [Space()]
    [SerializeField] private Sprite _commonButtonSprite;
    [SerializeField] private Sprite _resetButtonSprite;
    [SerializeField] private Text _launchLabel;
    [SerializeField] private Color _activeLaunchColor;
    [SerializeField] private Color _resetLaunchColor;

    private Color _normalLevelColor;
    bool _firstCheck = false;
    bool _resetMode = false;

    private void Start() {
        for (int i = 0; i < _togButtons.Length; i++)
        {
            _togButtons[i].Toggled += CheckState;
        }

        _normalLevelColor = _locationTerminal.color;
    }

    private void OnEnable() {
        if (_firstCheck)
            CheckState();

        _firstCheck = true;
    }

    public void ToggleLaunchButton(bool tog) => _launchButton.interactable = tog;

    public void ResetButtonMode(bool tog)
    {
        _launchButton.GetComponent<Image>().sprite = tog ? _resetButtonSprite : _commonButtonSprite;
        _launchLabel.color = tog ? _resetLaunchColor : _activeLaunchColor;
    }

    private void CheckState()
    {
        bool dangerCheck = false;
        _resetMode = true;

        for (int i = 0; i < _togButtons.Length; i++)
        {
            if (!_togButtons[i].ON)
            {
                _resetMode = false;
            }
            else
                dangerCheck = true;
        }

        if (_resetMode)
        {
            _locationTerminal.GetComponent<CanvasGroup>().alpha = 1f;
            StopAllCoroutines();
            _locationTerminal.text = SceneLocalizator.GetLocalizedString("MissionMenu", 2, 4);
            ResetButtonMode(true);
            return;
        }

        ResetButtonMode(false);
        
        if (dangerCheck)
        {
            StartCoroutine(DangerReset());
            _locationTerminal.text = SceneLocalizator.GetLocalizedString("MissionMenu", 2, 2);
            _locationTerminal.color = _dangerColor;
            return;
        }

        StopAllCoroutines();
        _locationTerminal.GetComponent<LevelLabel>().SetLevelData();
        _locationTerminal.color = _normalLevelColor;
        _locationTerminal.GetComponent<CanvasGroup>().alpha = 1f;
    }

    public void LaunchButtonPress()
    {
        if (_resetMode)
        {
            ResetMission();
        } else 
        {
            SceneStatics.SceneCore.GetComponent<MenuController>().StartGame();
        }

        ToggleLaunchButton(false);
    }

    private void ResetMission()
    {
        _locationTerminal.GetComponent<CanvasGroup>().alpha = 1f;
        StopAllCoroutines();
        GameSessionInfoHandler.ClearGameSession();
        SceneTransition.SwitchToScene("Lobby");
        _locationTerminal.text = SceneLocalizator.GetLocalizedString("MissionMenu", 2, 3);
        _locationTerminal.color = _dangerColor;
        Camera.main.GetComponent<MainMenuCamera>().CodeRed();
    }

    private IEnumerator DangerReset()
    {
        CanvasGroup cg = _locationTerminal.GetComponent<CanvasGroup>();
        bool enabled = false;
        float timer = _dangerTimeCycle;

        while (true)
        {
            timer -= ESTime.worldDeltaTime;

            if (timer < 0)
            {
                cg.alpha = enabled ? 1f : 0f;
                enabled = !enabled;
                timer = _dangerTimeCycle;
            }

            yield return null;
        }
    }
}
