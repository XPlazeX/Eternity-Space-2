using UnityEngine;
using UnityEngine.Events;

public class WaveSpawningMission : SpawningMission
{
    [Header("Wave spawning")]
    [SerializeField][Tooltip("Включительно")][Range(1, 100)] private int _firstLevelWaveCount = 1;
    [SerializeField][Tooltip("Включительно")][Range(1, 100)] private int _finalLevelWaveCount = 1;
    [SerializeField] private int _startLevelWeight = 5;
    [SerializeField] private int _finalLevelWeight = 15;
    [SerializeField] private float _missionWeightFinalMultiplier = 1.5f;
    [SerializeField][Range(0, 10)] private int _enemyBuffer = 0;
    [SerializeField] private float _reloadSpawnTime = 0.5f;
    [Space()]
    [SerializeField] private LevelEnemyDatabase _levelEnemyDatabase;
    [Space()]
    [Header("События вызываются в начале каждой волны и передают номер волны")]
    public UnityEvent OnNewWaveStarted;

    public override void StartPlay()
    {
        EnemyDBSpawner edbSpawner = SceneStatics.SceneCore.GetComponent<EnemyDBSpawner>();

        float currentMissionWeightMultiplier = Mathf.Lerp(1f, _missionWeightFinalMultiplier, GameSessionInfoHandler.LevelProgress);
        print($"Множитель веса текущей миссии: {currentMissionWeightMultiplier}. Прогресс миссии: {GameSessionInfoHandler.LevelProgress}");

        edbSpawner.SetParams(_levelEnemyDatabase, 
        Mathf.CeilToInt(Mathf.Lerp(_firstLevelWaveCount, _finalLevelWaveCount, GameSessionInfoHandler.LevelProgress)),
        Mathf.CeilToInt(currentMissionWeightMultiplier * _startLevelWeight), Mathf.CeilToInt(currentMissionWeightMultiplier * _finalLevelWeight), 
        _enemyBuffer, _reloadSpawnTime);

        SceneTransition.SceneOpened += StartOnSceneLoaded;

        edbSpawner.SpawnerEnded += TriggerVictory;

        base.StartPlay();
    }

    protected override void TriggerVictory()
    {
        base.TriggerVictory();
        GameSessionInfoHandler.RemoveDataCollection(EnemyDBSpawner.weightSelectorID_data_collection);
    }

    private void OnDisable() {
        SceneTransition.SceneOpened -= StartOnSceneLoaded;
    }

    private void StartOnSceneLoaded()
    {
        SceneStatics.SceneCore.GetComponent<EnemyDBSpawner>().StartSpawning();
        SceneStatics.SceneCore.GetComponent<EnemyDBSpawner>().WaveStarted += OnSpawnerWaveStarted;
    }

    private void OnSpawnerWaveStarted(int waveID)
    {
        OnNewWaveStarted.Invoke();
    }
}
