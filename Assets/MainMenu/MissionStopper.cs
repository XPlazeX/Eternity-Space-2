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

    private Color _normalLevelColor;
    bool _firstCheck = false;

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

    private void CheckState()
    {
        bool check = true;
        bool dangerCheck = false;

        for (int i = 0; i < _togButtons.Length; i++)
        {
            if (!_togButtons[i].ON)
                check = false;
            else
                dangerCheck = true;
        }

        if (check)
        {
            _locationTerminal.GetComponent<CanvasGroup>().alpha = 1f;
            StopAllCoroutines();
            Reset();
            return;
        }
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

    private void Reset()
    {
        GameSessionInfoHandler.ClearGameSession();
        SceneTransition.SwitchToScene("Lobby");
        _locationTerminal.text = SceneLocalizator.GetLocalizedString("MissionMenu", 2, 3);
        _locationTerminal.color = _dangerColor;
    }

    private IEnumerator DangerReset()
    {
        CanvasGroup cg = _locationTerminal.GetComponent<CanvasGroup>();
        bool enabled = false;
        float timer = _dangerTimeCycle;

        while (true)
        {
            timer -= Time.deltaTime;

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
