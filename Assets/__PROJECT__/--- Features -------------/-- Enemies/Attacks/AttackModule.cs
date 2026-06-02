using System.Collections;
using UnityEngine;

public class AttackModule : AttackingModule
{
    public delegate void attackAction();
    public event attackAction Fired;
    public event attackAction Reloaded;

    [SerializeField] private float _waitTime;
    [SerializeField] private float _attackReload;
    [SerializeField] private EnemyAttackObject[] _attackObjects;
    [SerializeField] private bool _autoStart = true;

    private float _aggro = 1f;
    private float _localAggro = 1f;

    private void OnEnable() 
    {
        if (_autoStart)
            StartCoroutine(Firing(false));
    }

    private void Start() {
        _aggro = ShipStats.GetValue("EnemyAggresionMultiplier") * Mathf.Sqrt(GameSessionInfoHandler.HardnessMultiplier);
    }

    public override void LocalMultiplyAggro(float multiplier)
    {
        _localAggro *= multiplier;
    }

    public override void HandFire(bool volley = false)
    {
        StartCoroutine(Firing(volley));
    }

    public void HandFireSeries(int seriesID, float waitTime = 0f)
    {
        StartCoroutine(FiringSeries(seriesID, waitTime));
    }

    private IEnumerator Firing(bool volley)
    {
        yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_waitTime / (_aggro * _localAggro) * (PlayerPrefs.GetFloat("GameMode") == 1f ? 0.2f : 1f)));

        while (true)
        {
            for (int i = 0; i < _attackObjects.Length; i++)
            {
                for (int j = 0; j < ((float)_attackObjects[i].Cycles * (_aggro * _localAggro) < 1f ? 1f : Mathf.Floor((float)_attackObjects[i].Cycles * (_aggro * _localAggro))); j++)
                {
                    _attackObjects[i].Fire(FixedDeltaPosition);
                    Fired?.Invoke();

                    yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_attackObjects[i].TimeBetweenCycles / (_aggro * _localAggro)));
                }

                yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_attackObjects[i].TimeCooling / (_aggro * _localAggro)));
            }

            Reloaded?.Invoke();

            if (volley)
            {
                yield break;
            }
            
            yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_attackReload / (_aggro * _localAggro)));
        }
    }

    private IEnumerator FiringSeries(int id, float waitTime = 0f)
    {
        yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(waitTime / (_aggro * _localAggro)));

        for (int j = 0; j < ((float)_attackObjects[id].Cycles * (_aggro * _localAggro) < 1f ? 1f : Mathf.Floor((float)_attackObjects[id].Cycles * (_aggro * _localAggro))); j++)
        {
            _attackObjects[id].Fire(FixedDeltaPosition);
            Fired?.Invoke();

            yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_attackObjects[id].TimeBetweenCycles / (_aggro * _localAggro)));
        }
    }
}

// HHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH

[System.Serializable]
public class EnemyAttackObject
{
    const int default_attack_sound_id = 1;

    [SerializeField] private int _cycles = 1;
    [SerializeField] private float _timeBetweenCycles;
    [SerializeField] private float _timeCooling = 0;
    [Space()]
    [SerializeField] private Bullet _bulletSample;
    [SerializeField] private _ExplosionBullet _muzzleExplosion;
    [SerializeField] private Transform[] _barrels;
    [SerializeField] private bool _randomBarrel = false;
    [Space()]
    [SerializeField] private int _bulletsPerFire = 1;
    [SerializeField] private float _fixedAngleStep;
    [SerializeField] private float _randomAngleStep;
    [SerializeField][Range(0, 1f)] private float _randomizingBulletSpeed = 0f;

    public int Cycles => _cycles;
    public float TimeBetweenCycles => _timeBetweenCycles;
    public float TimeCooling => _timeCooling;

    public void Fire(Vector3 fixedDeltaPosition = default(Vector3))
    {
        if (_randomBarrel)
            SpawnBullets(_barrels[Random.Range(0, _barrels.Length)], fixedDeltaPosition);
        
        else
            for (int i = 0; i < _barrels.Length; i++)
            {
                SpawnBullets(_barrels[i], fixedDeltaPosition);
            }
    }

    private void SpawnBullets(Transform barrel, Vector3 fixedDeltaPosition = default(Vector3))
    {
        float startAngle = barrel.eulerAngles.z;

        if (_fixedAngleStep != 0)
            startAngle += -_fixedAngleStep * (0.5f * (_bulletsPerFire - 1));

        for (int i = 0; i < _bulletsPerFire; i++)
        {
            Bullet bullet = Pool.Spawn(_bulletSample);
            bullet.transform.position = barrel.position;
            bullet.transform.rotation = Quaternion.Euler(0, 0, startAngle + Random.Range(-_randomAngleStep, _randomAngleStep));

            bullet.MultiplySpeedParams(Random.Range(1f - _randomizingBulletSpeed, 1f + _randomizingBulletSpeed), fixedDeltaPosition);

            startAngle += _fixedAngleStep;

        }

        if (_muzzleExplosion != null)
            _muzzleExplosion.SpawnExplosion(barrel.position);   
    }
}
