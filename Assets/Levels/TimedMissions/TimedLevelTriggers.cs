using UnityEngine;

[RequireComponent(typeof(TimeMission))]
public class TimedLevelTriggers : MonoBehaviour
{
    [SerializeField] private int _targetLevel = 0;
    [SerializeField] private TimeTrigger[] _timeTriggers;
    [SerializeField] private ControllableText _controllableText;
    [SerializeField] private string _controllableTextFilename;
    [SerializeField] private Vector3 _textPosition;
    [SerializeField] private bool _autoExitToLobby;
    [SerializeField] private bool _introByExit;

    private ControllableText _sampleText;
    private int _currentPhrase = 0;
    private TimeMission _timeMission;

    private void Start() 
    {
        if (GameSessionInfoHandler.CurrentLevel != _targetLevel)
            return;

        _timeMission = GetComponent<TimeMission>();
        _sampleText = Instantiate(_controllableText, _textPosition, Quaternion.identity);
        _sampleText.Initialize(_controllableTextFilename, 0);

        _sampleText.SetText(_currentPhrase);

        if (_introByExit)
            VictoryHandler.CustomSceneOnExit = "Intro";

        _timeMission.TimedMissionTick += OnTimedMissionTick;
        VictoryHandler.MissionVictored += OnMissionVictored;
    }

    private void OnDisable() 
    {
        if (GameSessionInfoHandler.CurrentLevel != _targetLevel)
            return;

        _timeMission.TimedMissionTick -= OnTimedMissionTick;
        VictoryHandler.MissionVictored += OnMissionVictored;
    }

    public void OnMissionVictored()
    {
        if (_autoExitToLobby)
            VictoryHandler.VictorySession();
    }

    public void OnTimedMissionTick(float t)
    {
        float elapsedTime = _timeMission.MaxTime - t;

        if (_currentPhrase >= _timeTriggers.Length)
            return;

        if (elapsedTime > _timeTriggers[_currentPhrase].elapsedTime)
        {
            Trigger();
        }
    }

    public void Trigger()
    {
        if (!string.IsNullOrEmpty(_timeTriggers[_currentPhrase].dialogueFilename))
            GameObject.FindWithTag("BetweenScenes").GetComponent<DialogueOpener>().TriggerDialogue(_timeTriggers[_currentPhrase].dialogueFilename);

        _currentPhrase ++;
        _sampleText.SetText(_currentPhrase);
    }

    [System.Serializable]
    private struct TimeTrigger
    {
        public float elapsedTime;
        public string dialogueFilename;
    }
}
