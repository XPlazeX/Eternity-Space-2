using System.Collections;
using UnityEngine;

public class EnemyDBSpawner : SpawnerRoot
{
    private const float default_spawn_reload = 0.5f;
    public const string weightSelectorID_data_collection = "WS";

    public event spawnerAction WaveCompleted;
    public event spawnerAction EnemyKilled;

    [SerializeField] private WeightSelectorSelector[] _wsSelectors;
    [Space()]
    [SerializeField] private int _waveWeight;

    public LevelEnemyDatabase ActiveLEDB {get; private set;}
    public int ActiveEnemies {get; private set;}
    public int WaveCount {get; private set;} = 10;
    public int StartWeight {get; private set;}
    public int FinalWeight {get; private set;} 
    public int EnemyBuffer {get; private set;} 
    public float ReloadSpawnTime {get; private set;}
    public int CurrentWave => _currentWave;

    private int _currentWave;
    private WeightSelector _weightSelector = null;
    public int WeightPerWave {get; private set;} = 10; // максимальный вес волны, достигаемый на последней волне

    public void SetParams(LevelEnemyDatabase ledb, int waveCount, int startWeight, int topWeight, int enemyBuffer, float reloadTime)
    {
        ActiveLEDB = ledb;
        WaveCount = waveCount;
        StartWeight = startWeight;
        FinalWeight = topWeight;
        EnemyBuffer = enemyBuffer;
        ReloadSpawnTime = reloadTime;

        SceneStatics.SceneCore.GetComponent<Spawner>().PrintCountUI(ActiveEnemies);
        SceneStatics.SceneCore.GetComponent<Spawner>().PrintProgressUI($"{_currentWave} / {WaveCount}");

        //print($"Установлены параметры спаунера: волны {WaveCount}. Вес {StartWeight}-{FinalWeight}. Буфер {EnemyBuffer}. Перезарядка {ReloadSpawnTime}.");
    }

    public void Modify(float waveCountMultiplier, float weightMultiplier, int additiveEnemyBuffer, float reloadTimeMultiplier)
    {
        if (_spawner == null)
            _spawner = SceneStatics.SceneCore.GetComponent<Spawner>();

        WaveCount = Mathf.CeilToInt(waveCountMultiplier * WaveCount);
        StartWeight = Mathf.CeilToInt(StartWeight * weightMultiplier);
        FinalWeight = Mathf.CeilToInt(FinalWeight * weightMultiplier);
        EnemyBuffer += additiveEnemyBuffer;
        ReloadSpawnTime *= reloadTimeMultiplier;

        _spawner.PrintProgressUI($"{_currentWave} / {WaveCount}");

        //print($"мод спаунера волн: волны Х{waveCountMultiplier} = {WaveCount}. Вес {StartWeight}-{FinalWeight}. Буфер {EnemyBuffer}. Перезарядка {ReloadSpawnTime}.");
    }

    public override void StartSpawning()
    {
        if (_weightSelector != null)
        {
            print("Используется кастомный выборщик веса");
        } else if (GameSessionInfoHandler.ExistDataCollection(weightSelectorID_data_collection))
        {
            _weightSelector = Instantiate(_wsSelectors[GameSessionInfoHandler.GetDataCollection(weightSelectorID_data_collection)[0]].weightSelector);
            //print($"Загружен выборщик веса из сохранения: {GameSessionInfoHandler.GetDataCollection(weightSelectorID_data_collection)[0]}");
        } else
        {
            int selectedID = 0;

            if (GameSessionInfoHandler.LevelProgress == 0 || GameSessionInfoHandler.MaxLevel == 1)
            {
                //print("Выбран выборщик веса по умолчанию для первого уровня.");
            }
            else
            {
                for (int i = 0; i < _wsSelectors.Length; i++)
                {
                    if (GameSessionInfoHandler.CurrentLevel >= _wsSelectors[i].minimumLevel && Random.value <= _wsSelectors[i].selectChance)
                    {
                        selectedID = i;
                        break;
                    }
                }
            }

            _weightSelector = Instantiate(_wsSelectors[selectedID].weightSelector);

            GameSessionInfoHandler.AddDataCollection(weightSelectorID_data_collection, new System.Collections.Generic.List<int>(1) {selectedID});
            //print($"Сохранён и установлен выборщик веса : {selectedID}");
        }

        _weightSelector.Initialize();

        ActiveLEDB.Initialize();

        print("СПАУНЕР ВОЛН ЗАПУЩЕН");

        base.StartSpawning();
    }

    public override void Stop()
    {
        GameSessionInfoHandler.RemoveDataCollection(weightSelectorID_data_collection);
        base.Stop();
    }

    public void SetCustomWeightSelector(WeightSelector ws)
    {
        _weightSelector = ws;
        print("Установлен кастомный выборщик веса");
    }

    protected override bool CheckConditions()
    {
        _spawner.PrintProgressUI($"{_currentWave} / {WaveCount}");
        return _currentWave != WaveCount;
    }

    protected override IEnumerator SpawnBody()
    {
        int aviableWeight = Mathf.CeilToInt(Mathf.Lerp(StartWeight, FinalWeight, (float)_currentWave / WaveCount));

        _waveWeight = aviableWeight;

        while (aviableWeight > 0)
        {
            float selectedWeight = _weightSelector.GetWeight();
            int choosedWeight = 0;

            EnemyPack pack = ActiveLEDB.GetRandomEnemyPacks(selectedWeight, aviableWeight, out choosedWeight);
            aviableWeight -= choosedWeight;

            yield return StartCoroutine(SpawnEnemyPack(pack));
        }

        while (ActiveEnemies > EnemyBuffer)
        {
            yield return null;
        }

        if (_currentWave == WaveCount - 1)
        {
            while (ActiveEnemies > 0)
            {
                yield return null;
            }
        }

        _currentWave ++;
        WaveCompleted?.Invoke();
    }

    private IEnumerator SpawnEnemyPack(EnemyPack enemyPack)
    {
        if (enemyPack == null || enemyPack.Count == 0)
            yield break;

        for (int i = 0; i < enemyPack.Count; i++)
        {
            DamageBody spawnedDB = Spawner.SpawnDamageBody(enemyPack.Enemy);

            spawnedDB.Deathed += UnregisterEnemy;
            ActiveEnemies ++;
            _spawner.PrintCountUI(ActiveEnemies);

            Spawner.InitializeHPBar(spawnedDB);

            yield return new WaitForSeconds(ReloadSpawnTime);
        }
    }

    public void UnregisterEnemy()
    {
        ActiveEnemies --;
        _spawner.PrintCountUI(ActiveEnemies);

        EnemyKilled?.Invoke();
    }

    public void SetLEDB(LevelEnemyDatabase ledb)
    {
        ActiveLEDB = ledb;
        ActiveLEDB.Initialize();
        print("Setting new ledb to enemyDBSpawner");
    }

    public void InsertLEDBPacks(EnemyPack[] packs)
    {
        ActiveLEDB.AddEnemyPacks(packs);
    }

    [System.Serializable]
    private struct WeightSelectorSelector
    {
        public WeightSelector weightSelector;
        [Range(0, 1f)] public float selectChance;
        public int minimumLevel;
    }
}
