using System.Collections;
using UnityEngine;

public class Reinforcementor : MonoBehaviour
{
    [SerializeField] private int _startBonusTime;
    [SerializeField] private int _reloadTime;
    [SerializeField] private int _recallTime;
    [Space()]
    [SerializeField] private bool _randomAccess;
    [SerializeField] private Reinforcement[] _reinforcements;
    [SerializeField] private int _capReinforcementEnemySpawnCount;

    private Spawner _spawner;
    private int _enemySpawned;

    private void Start() 
    {
        _spawner = SceneStatics.SceneCore.GetComponent<Spawner>();
        StartCoroutine(ReinforcementsCycle());

        VictoryHandler.LevelVictored += Stop;
    }

    private void OnDisable() {
        VictoryHandler.LevelVictored -= Stop;
    }

    private void Stop()
    {
        StopAllCoroutines();
        _spawner.SetReinforcementDelay(999);
    }

    private void UnregisterEnemy()
    {
        _enemySpawned -= 1;
    }

    private IEnumerator ReinforcementsCycle()
    {
        int timer = _startBonusTime + _reloadTime;
        int cycler = 0;
        bool succes = true;

        _spawner.SetReinforcementDelay(timer);
        _spawner.ShowReinforcementUI();

        while (true)
        {
            yield return new WaitForSeconds(1f);

            timer -= 1;
            _spawner.SetReinforcementDelay(timer);

            if (timer < 0)
            {
                Reinforcement reinforcement;

                if (_randomAccess)
                {
                    reinforcement = _reinforcements[Random.Range(0, _reinforcements.Length)];
                } else 
                {
                    reinforcement = _reinforcements[cycler];
                    cycler ++;
                    if (cycler == _reinforcements.Length)
                        cycler = 0;
                }

                for (int i = 0; i < reinforcement.enemies.Length; i++)
                {
                    if (_enemySpawned >= _capReinforcementEnemySpawnCount)
                    {
                        succes = false;
                        break;
                    }

                    DamageBody db = Spawner.SpawnDamageBody(reinforcement.enemies[i]);

                    _enemySpawned ++;
                    db.Deathed += UnregisterEnemy;
                    succes = true;

                    yield return new WaitForSeconds(reinforcement.spawnReload);
                }

                timer = succes ? _reloadTime : _recallTime;
                _spawner.SetReinforcementDelay(timer);
            }
        }
    }

    [System.Serializable]
    struct Reinforcement
    {
        public DamageBody[] enemies;
        public float spawnReload;
    }
}
