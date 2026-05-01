using UnityEngine;

public class BurstGun : AttackPattern
{
    [SerializeField] private int _burstCount;
    [SerializeField] private float _inBurstReload;
    [SerializeField] private int _bulletsPerFire = 1;
    [SerializeField] private int[] _barrelPool;

    private float _burstTimer;
    private int _burstOrder;
    private bool _bursting;

    protected override void Update()
    {
        base.Update();

        if (_bursting)
        {
            _burstTimer -= Time.deltaTime;

            if (_burstTimer <= 0f)
            {
                for (int i = 0; i < _bulletsPerFire; i++)
                {
                    int id = Random.Range(0, _barrelPool.Length);
                    SpawnBullet(_barrels[_barrelPool[id]].position, _barrels[_barrelPool[id]].eulerAngles.z);   
                }

                _burstTimer = _inBurstReload;

                _burstOrder ++;

                base.Fire();

                if (_burstOrder >= _burstCount)
                {
                    _burstOrder = 0;
                    _bursting = false;
                }
            }
        }
    }

    public override void Fire()
    {
        _bursting = true;
    }
}
