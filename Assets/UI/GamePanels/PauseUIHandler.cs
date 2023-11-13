using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _timeBufferImage;
    [SerializeField] private Text _countdownLabel;

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

    private IEnumerator Countdown(int time)
    {
        _timeBufferImage.SetActive(true);

        float timer = time;

        while (timer > 0)
        {
            _countdownLabel.text = Mathf.CeilToInt(timer).ToString();
            timer -= Time.unscaledDeltaTime;

            yield return null;
        }

        _timeBufferImage.SetActive(false);
        TimeHandler.Resume();
    }
}
