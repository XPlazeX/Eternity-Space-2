using UnityEngine;

public class TimeMission : SpawningMission
{
    public delegate void timeMissionEvent(float time);

    public event timeMissionEvent TimedMissionStarted;
    public event timeMissionEvent TimedMissionTick;

    [SerializeField] private float _firstLevelTime;
    [SerializeField] private float _finalLevelTime;

    private Spawner _spawner;
    private float _timer;
    private int _lastCeiledTime;
    private float _targetTime;

    public override void StartPlay()
    {
        _spawner = SceneStatics.SceneCore.GetComponent<Spawner>();

        _targetTime = Mathf.CeilToInt(Mathf.Lerp(_firstLevelTime, _finalLevelTime, GameSessionInfoHandler.LevelProgress));

        _timer = _targetTime;
        _lastCeiledTime = Mathf.CeilToInt(_targetTime) - 1;

        _spawner.HideLabel();

        base.StartPlay();

        TimedMissionStarted?.Invoke(_targetTime);
        Tick();
    }

    private void Update() 
    {
        if (_timer < 0)
            return;

        _timer -= Time.deltaTime;

        if (_timer < _lastCeiledTime)
        {
            Tick();
            _lastCeiledTime --;
        }
    }

    public virtual void Tick()
    {
        TimedMissionTick?.Invoke(Mathf.CeilToInt(_timer));

        _spawner.PrintProgressUI($"{Mathf.CeilToInt(_timer)}s");

        if (_timer < 0f)
            TriggerVictory();
    }
}
