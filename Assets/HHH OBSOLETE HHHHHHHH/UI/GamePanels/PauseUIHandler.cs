using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _timeBufferImage;
    [SerializeField] private Text _countdownLabel;
    [Space()]
    [SerializeField] private CanvasGroup _pauseCanvasGroup;
    [SerializeField] private GameObject _uiPlacePanel;

    public void ShowPause()
    {
        _timeBufferImage.SetActive(false);
        _pausePanel.SetActive(true);
    }

    public void Resume(int timeBuffer)
    {
        _pausePanel.SetActive(false);
        StartCoroutine(Countdown(timeBuffer));
    }

    public void ToggleUIPlaceMode(bool tog)
    {
        _pauseCanvasGroup.alpha = tog ? 0f : 1f;
        _pauseCanvasGroup.interactable = tog ? false : true;
        _uiPlacePanel.SetActive(tog);
    }

    private IEnumerator Countdown(int time)
    {
        _timeBufferImage.SetActive(true);

        float timer = time;

        while (timer > 0)
        {
            _countdownLabel.text = $"{(int)timer % 10}.{(int)(timer * 10) % 10}{(int)(timer * 100) % 10}";
            timer -= ESTime.unscaledDeltaTime;

            yield return null;
        }

        _timeBufferImage.SetActive(false);
        TimeHandler.Resume();
    }
}
