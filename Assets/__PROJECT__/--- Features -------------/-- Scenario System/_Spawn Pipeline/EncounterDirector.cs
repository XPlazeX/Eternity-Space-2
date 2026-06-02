using System.Collections.Generic;
using UnityEngine;

public class EncounterDirector : MonoBehaviour
{
    public event System.Action<Encounter> EncounterStarted;
    public event System.Action<Encounter> EncounterStopped;
    public event System.Action<EnemyHandle> AnyBossSpawned;
    public event System.Action<EnemyHandle> AnyBossDied;
    public event System.Action<EnemyHandle> AnyEnemySpawned;
    public event System.Action<EnemyHandle> AnyEnemyDied;

    [SerializeField] private EnemySpawner enemySpawner;

    private Dictionary<string, EncounterParser> _activeEncounters = new();
    private Dictionary<string, EncounterParser> _completedEncounters = new(); //  не выполняющиеся => враги стандартно не появятся

    public bool Processing {get; private set;}
    public bool Paused {get; private set;}

    private List<EnemyHandle> _externalEnemiesAlive = new();
    private List<EnemyHandle> _externalBossesAlive = new();
    private int _defeatedExternalEnemies = 0;
    private int _defeatedExternalBosses = 0;

    /// <summary>
    /// Включает в себя боссов
    /// </summary>
    public int AliveEnemies {get{
        int e = 0;
        foreach (var kvp in _activeEncounters) {e += kvp.Value.EnemiesAlive;}
        e += _externalEnemiesAlive.Count;
        return e;}}

    public int AliveBosses {get{
        int e = 0;
        foreach (var kvp in _activeEncounters) {e += kvp.Value.BossesAlive;}
        e += _externalBossesAlive.Count;
        return e;}}

    public float TotalWeight {get{
        float w = 0f;
        foreach (var kvp in _activeEncounters) {w += kvp.Value.TotalWeight;}
        for (int i = 0; i < _externalEnemiesAlive.Count; i++) {w += _externalEnemiesAlive[i].weight;}
        return w;}}

    public int DefeatedEnemies {get{
        int e = 0;
        foreach (var kvp in _activeEncounters) {e += kvp.Value.DefeatedEnemies;}
        foreach (var kvp in _completedEncounters) {e += kvp.Value.DefeatedEnemies;}
        e += _defeatedExternalEnemies; 
        return e;}}

    public int DefeatedBosses {get{
        int e = 0;
        foreach (var kvp in _activeEncounters) {e += kvp.Value.DefeatedBosses;}
        foreach (var kvp in _completedEncounters) {e += kvp.Value.DefeatedBosses;}
        e += _defeatedExternalBosses;
        return e;}}

    public bool AnyEnemyAlive => TotalWeight > 0 || AliveEnemies > 0 || AliveBosses > 0;
    public int ActiveEncountersCount => _activeEncounters.Count;

    void Update()
    {
        if (!Processing) return;
        TickAll(Time.deltaTime);
    }

    public void StartEncounter(Encounter encounter)
    {
        if (encounter == null || _activeEncounters.ContainsKey(encounter.RuntimeID))
        {
            return;
        }

        EncounterParser parser = new EncounterParser(encounter, enemySpawner);

        _activeEncounters[encounter.RuntimeID] = parser;
        parser.EncounterCleared += OnEncounterCleared;
        parser.EnemySpawned += OnEncounterEnemySpawned;
        parser.EnemyDied += OnEncounterEnemyDied;
        parser.BossSpawned += OnEncounterBossSpawned;
        parser.BossDied += OnEncounterBossDied;

        EncounterStarted?.Invoke(encounter);

        if (!Processing) 
        {
            Processing = true;
        }
    }

    public void ForcedStopEncounter(string runtimeID)
    {
        if (!_activeEncounters.ContainsKey(runtimeID))
        {
            return;
        }

        StopEncounter(runtimeID);
    }

    public void ForcedStopEncounter(Encounter encounter)
    {
        if (encounter == null) return;

        ForcedStopEncounter(encounter.RuntimeID);
    }

    private void OnEncounterCleared(Encounter encounterData)
    {
        StopEncounter(encounterData.RuntimeID);
    }

    private void StopEncounter(string runtimeID)
    {
        if (!_activeEncounters.ContainsKey(runtimeID))
        {
            return;
        }

        EncounterParser stoppedParser = _activeEncounters[runtimeID];

        stoppedParser.EncounterCleared -= OnEncounterCleared;
        stoppedParser.EnemySpawned -= OnEncounterEnemySpawned;
        stoppedParser.EnemyDied -= OnEncounterEnemyDied;
        stoppedParser.BossSpawned -= OnEncounterBossSpawned;
        stoppedParser.BossDied -= OnEncounterBossDied;

        _activeEncounters.Remove(runtimeID);

        _completedEncounters[runtimeID] = stoppedParser;
        EncounterStopped?.Invoke(stoppedParser.EncounterData);

        if (Processing && _activeEncounters.Count == 0)
        {
            Processing = false;
        }
    }

    private void TickAll(float dt)
    {
        if (Paused) return;

        List<EncounterParser> encounterParsers = new List<EncounterParser>(_activeEncounters.Values);
        for (int i = 0; i < encounterParsers.Count; i++)
        {
            encounterParsers[i].Tick(dt);
        }
    }

    public void ExternalEnemySpawn(ExternalSpawnRequest externalSpawnRequest)
    {
        for (int s = 0; s < externalSpawnRequest.spawnRequests.Count; s++)
        {
            List<EnemyHandle> handles = enemySpawner.Spawn(externalSpawnRequest.spawnRequests[s]);

            for (int i = 0; i < handles.Count; i++)
            {
                handles[i].DiedCallback += OnExternalEnemyDie;
                if (handles[i].isBoss)
                {
                    handles[i].DiedCallback += OnExternalBossDie;
                    _externalBossesAlive.Add(handles[i]);
                    AnyBossSpawned?.Invoke(handles[i]);
                }

                _externalEnemiesAlive.Add(handles[i]);
                AnyEnemySpawned?.Invoke(handles[i]);
            }
        }
    }

    private void OnEncounterEnemySpawned(EnemyHandle handle)
    {
        AnyEnemySpawned?.Invoke(handle);
    }
    private void OnEncounterBossSpawned(EnemyHandle handle)
    {
        AnyBossSpawned?.Invoke(handle);
    }
    private void OnEncounterEnemyDied(EnemyHandle handle)
    {
        AnyEnemyDied?.Invoke(handle);
    }
    private void OnEncounterBossDied(EnemyHandle handle)
    {
        AnyBossDied?.Invoke(handle);
    }

    private void OnExternalEnemyDie(EnemyHandle handle)
    {
        _externalEnemiesAlive.Remove(handle);
        _defeatedExternalEnemies ++;
        AnyEnemyDied?.Invoke(handle);
    }

    private void OnExternalBossDie(EnemyHandle handle)
    {
        _externalBossesAlive.Remove(handle);
        _defeatedExternalBosses ++;
        AnyBossDied?.Invoke(handle);
    }

    public Dictionary<string, EncounterSnapshot> GetActiveEncounterSnapshots() // ScenarioRunner просто вставит это в context
    {
        Dictionary<string, EncounterSnapshot> snapshots = new();

        foreach (var kvp in _activeEncounters)
        {
            snapshots[kvp.Key] = kvp.Value.GenerateSnapshot();
        }

        return snapshots;
    }

    public bool TryGetEncounterSnapshot(string encounterID, out EncounterSnapshot snapshot) // если кто-то просто помнит о encounter
    {
        snapshot = new EncounterSnapshot();

        if (_activeEncounters.ContainsKey(encounterID))
        {
            snapshot = _activeEncounters[encounterID].GenerateSnapshot();
            return true;
        }

        else if (_completedEncounters.ContainsKey(encounterID))
        {
            snapshot = _completedEncounters[encounterID].GenerateSnapshot();
            return true;
        }

        return false;
    }

    public EncounterParser GetActiveParser(string encounterID) // если кому-то понадобится следить за конкретным парсером, на события подписаться
    {
        if (!_activeEncounters.ContainsKey(encounterID)) return null;
        return _activeEncounters[encounterID];
    }
}
