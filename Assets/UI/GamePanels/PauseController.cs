using UnityEngine;

[RequireComponent(typeof(PauseUIHandler))]
public class PauseController : MonoBehaviour
{
    private PauseUIHandler _pauseUIHandler;
    public static bool HandPause {get; private set;} = false;

    private int _timeBuffer;

    private void Start() {
        _pauseUIHandler = GetComponent<PauseUIHandler>();
        _timeBuffer = Mathf.RoundToInt(PlayerPrefs.GetFloat("EngineTime", 1));
    }

    private void Pause()
    {
        if (Time.timeScale == 0)
            return;

        TimeHandler.Pause();
        _pauseUIHandler.ShowPause();

        HandPause = true;
    }

    public void Resume()
    {
        _pauseUIHandler.Resume(_timeBuffer);

        HandPause = false;
    }
}
