using System.Collections;
using UnityEngine;

public class AttackModule : MonoBehaviour, IAttackModule
{
    [SerializeField] private float _waitTime;
    [SerializeField] private float _attackReload;
    [SerializeField] private EnemyAttackObject[] _attackObjects;
    [SerializeField] private bool _autoStart = true;

    private float _aggro = 1f;
    private float _localAggro = 1f;

    private void OnEnable() 
    {
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
            for (int i = 0; i < _attackObjects.Length; i++)
            {
                for (int j = 0; j < ((float)_attackObjects[i].Cycles * (_aggro * _localAggro) < 1f ? 1f : Mathf.Floor((float)_attackObjects[i].Cycles * (_aggro * _localAggro))); j++)
                {
                    _attackObjects[i].Fire();

                    yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_attackObjects[i].TimeBetweenCycles / (_aggro * _localAggro)));
                }

                yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_attackObjects[i].TimeCooling / (_aggro * _localAggro)));
            }

            yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_attackReload / (_aggro * _localAggro)));
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
    [SerializeField] private int _bulletCode;
    [SerializeField] private Transform[] _barrels;
    [SerializeField] private bool _randomBarrel = false;
    [Space()]
    [SerializeField] private int _bulletsPerFire = 1;
    [SerializeField] private float _fixedAngleStep;
    [SerializeField] private float _randomAngleStep;
    [SerializeField][Range(0, 1f)] private float _randomizingBulletSpeed = 0f;
    [Space()]
    [SerializeField] private int _soundOnFireID = default_attack_sound_id;

    public int Cycles => _cycles;
    public float TimeBetweenCycles => _timeBetweenCycles;
    public float TimeCooling => _timeCooling;

    public void Fire()
    {
        if (_randomBarrel)
            SpawnBullets(_barrels[Random.Range(0, _barrels.Length)]);
        
        else
            for (int i = 0; i < _barrels.Length; i++)
            {
                SpawnBullets(_barrels[i]);
            }

        FightSoundHelper.PlaySound(_soundOnFireID, _barrels[0].position);
    }

    private void SpawnBullets(Transform barrel)
    {
        float startAngle = barrel.eulerAngles.z;

        if (_fixedAngleStep != 0)
            startAngle += -_fixedAngleStep * (0.5f * (_bulletsPerFire - 1));

        for (int i = 0; i < _bulletsPerFire; i++)
        {
            Bullet bullet = GenericBulletDatabase.GetBullet(_bulletCode);
            bullet.transform.position = barrel.position;
            bullet.transform.rotation = Quaternion.Euler(0, 0, startAngle + Random.Range(-_randomAngleStep, _randomAngleStep));

            if (_randomizingBulletSpeed > 0)
                bullet.MultiplySpeedParams(Random.Range(1f - _randomizingBulletSpeed, 1f + _randomizingBulletSpeed));

            startAngle += _fixedAngleStep;

        }   
    }
}
