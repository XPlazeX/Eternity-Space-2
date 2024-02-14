using System.Collections;
using UnityEngine;

public class SpawnAttackModule : MonoBehaviour, IAttackModule
{
    [SerializeField] private float _waitTime;
    [SerializeField] private float _attackReload;
    [SerializeField] private SpawnEnemyAttackObject[] _attackObjects;
    [SerializeField] private int _maxSpawnerEnemies = 1;
    [SerializeField] private bool _autoStart = true;

    private EnemyDBSpawner _edbSpawner;
    private float _aggro = 1f;
    private float _localAggro = 1f;

    private void OnEnable() {
        _edbSpawner = SceneStatics.SceneCore.GetComponent<EnemyDBSpawner>();

        if (_autoStart)
            StartCoroutine(Firing());
    }

    private void Start() {
        _aggro = ShipStats.GetValue("EnemyAggresionMultiplier");
    }

    public void LocalMultiplyAggro(float multiplier)
    {
        _localAggro *= multiplier;
    }

    public void HandFire()
    {
        StartCoroutine(Firing());
    }

    private IEnumerator Firing()
    {
        yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_waitTime / (_aggro * _localAggro)));

        while (true)
        {
            if (_edbSpawner.ActiveEnemies < _maxSpawnerEnemies)
            {
                for (int i = 0; i < _attackObjects.Length; i++)
                {
                    for (int j = 0; j < Mathf.Floor((float)_attackObjects[i].Cycles * (_aggro * _localAggro)); j++)
                    {
                        _attackObjects[i].Fire();

                        yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_attackObjects[i].TimeBetweenCycles / (_aggro * _localAggro)));
                    }

                    yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_attackObjects[i].TimeCooling / (_aggro * _localAggro)));
                }
            }

            yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_attackReload / (_aggro * _localAggro)));
        }
    }
}

// HHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH

[System.Serializable]
public class SpawnEnemyAttackObject
{
    [SerializeField] private int _cycles = 1;
    [SerializeField] private float _timeBetweenCycles;
    [SerializeField] private float _timeCooling = 0;
    [Space()]
    [SerializeField] private DamageBody _enemy;
    [SerializeField] private Transform[] _barrels;
    [SerializeField] private bool _randomBarrel = false;
    [Space()]
    [SerializeField] private int _enemyPerFire = 1;

    public int Cycles => _cycles;
    public float TimeBetweenCycles => _timeBetweenCycles;
    public float TimeCooling => _timeCooling;

    public void Fire()
    {
        if (_randomBarrel)
            Spawn(_barrels[Random.Range(0, _barrels.Length)]);
        
        else
            for (int i = 0; i < _barrels.Length; i++)
            {
                Spawn(_barrels[i]);
            }
    }

    private void Spawn(Transform barrel)
    {
        for (int i = 0; i < _enemyPerFire; i++)
        {
            Spawner.InitializeHPBar(Spawner.SpawnDamageBody(_enemy, barrel.position));
        }
    }
}