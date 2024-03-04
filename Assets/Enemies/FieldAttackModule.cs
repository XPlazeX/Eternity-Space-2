using System.Collections;
using UnityEngine;

public class FieldAttackModule : MonoBehaviour
{
    [SerializeField] private float _waitTime;
    [SerializeField] private float _attackReload;
    [SerializeField] private FieldAttackObject[] _attackObjects;

    private float _aggro = 1f;

    private void OnEnable() {
        StartCoroutine(Firing());
    }

    private void Start() {
        _aggro = ShipStats.GetValue("EnemyAggresionMultiplier");

        for (int i = 0; i < _attackObjects.Length; i++)
        {
            _attackObjects[i].Preload();
        }
    }

    private IEnumerator Firing()
    {
        yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_waitTime / _aggro));

        while (true)
        {
            for (int i = 0; i < _attackObjects.Length; i++)
            {
                for (int j = 0; j < Mathf.Floor((float)_attackObjects[i].Cycles * _aggro); j++)
                {
                    _attackObjects[i].Fire();

                    yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_attackObjects[i].TimeBetweenCycles / _aggro));
                }

                yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_attackObjects[i].TimeCooling / _aggro));
            }

            yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_attackReload / _aggro));
        }
    }

    [System.Serializable]
    public class FieldAttackObject
    {
        [SerializeField] private int _cycles = 1;
        [SerializeField] private float _timeBetweenCycles;
        [SerializeField] private float _timeCooling = 0;
        [Space()]
        [SerializeField] private int _bulletCode;
        [Space()]
        [SerializeField] private int _bulletsPerFire = 1;
        [SerializeField] private Vector3 _fixedVector2Step;
        [SerializeField] private Vector3 _randomVector2Step;

        public int Cycles => _cycles;
        public float TimeBetweenCycles => _timeBetweenCycles;
        public float TimeCooling => _timeCooling;

        public void Preload()
        {
            GenericBulletDatabase.PreloadBullet(_bulletCode);
        }

        public void Fire()
        {
            SpawnBullets(Player.PlayerTransform.position);
        }

        private void SpawnBullets(Vector3 position)
        {

            Vector3 startPos = position;

            startPos += -_fixedVector2Step * (0.5f * (_bulletsPerFire - 1));

            for (int i = 0; i < _bulletsPerFire; i++)
            {
                Bullet bullet = GenericBulletDatabase.GetBullet(_bulletCode);
                bullet.transform.position = startPos + new Vector3(Random.Range(-_randomVector2Step.x, _randomVector2Step.x), Random.Range(-_randomVector2Step.y, _randomVector2Step.y), 0f);

                startPos += _fixedVector2Step;

            }   
        }
    }
}

// HHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH


