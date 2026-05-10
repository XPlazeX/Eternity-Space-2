using System.Collections;
using UnityEngine;

public class LaserAttackModule : AttackingModule
{
    [SerializeField] private float _waitTime;
    [SerializeField] private float _attackReload;
    [SerializeField] private EnemyLaserAttackObject[] _attackObjects;
    [SerializeField] private bool _autoStart = true;

    private float _aggro = 1f;
    private float _localAggro = 1f;

    private void OnEnable() {
        if (_autoStart)
            StartCoroutine(Firing(false));
    }

    private void Start() {
        _aggro = ShipStats.GetValue("EnemyAggresionMultiplier");
    }

    public override void LocalMultiplyAggro(float multiplier)
    {
        _localAggro *= multiplier;
    }

    public override void HandFire(bool volley = false)
    {
        StartCoroutine(Firing(volley));
    }

    private IEnumerator Firing(bool volley)
    {
        yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_waitTime));

        while (true)
        {
            for (int i = 0; i < _attackObjects.Length; i++)
            {
                for (int j = 0; j < _attackObjects[i].Cycles; j++)
                {
                    _attackObjects[i].Fire();

                    yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_attackObjects[i].TimeBetweenCycles));
                }

                yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_attackObjects[i].TimeCooling / (_aggro * _localAggro)));
            }

            if (volley) yield break;

            yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_attackReload));
        }
    }
}

// HHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH

[System.Serializable]
public class EnemyLaserAttackObject
{
    [SerializeField] private int _cycles = 1;
    [SerializeField] private float _timeBetweenCycles;
    [SerializeField] private float _timeCooling = 0;
    [Space()]
    [SerializeField] private LaserObject _laserSample;
    [SerializeField] private Transform[] _barrels;
    [SerializeField] private bool _randomBarrel = false;
    [Space()]
    [SerializeField] private LayerMask _mask;
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _laserLifetime;

    public int Cycles => _cycles;
    public float TimeBetweenCycles => _timeBetweenCycles;
    public float TimeCooling => _timeCooling;

    public void Fire()
    {
        if (_randomBarrel)
            SpawnLaser(_barrels[Random.Range(0, _barrels.Length)]);
        
        else
            for (int i = 0; i < _barrels.Length; i++)
            {
                SpawnLaser(_barrels[i]);
            }
    }

    private void SpawnLaser(Transform barrel)
    {
        LaserObject laserObject = Pool.Spawn(_laserSample);

        laserObject.CreateLaser(barrel, _attackDistance, _mask, _laserLifetime);
    }
}

