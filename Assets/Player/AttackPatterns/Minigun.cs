using UnityEngine;
using System.Collections;

public class Minigun : AttackPattern
{
    [SerializeField] private float _topFirerate;
    [SerializeField] private float _firerateGrowth;
    [SerializeField] private int _bulletsPerFire = 1;
    [SerializeField] private int[] _barrelPool;

    private float _startFirerate;
    private bool _spinning = false;
    private WeaponRoot _weaponRoot;

    private void Start() {
        _startFirerate = FireReload;

        FindPlayer();
        
        TimeHandler.TimeSlow += EndSpin;
        TimeHandler.TimeResume += StartSpin;
    }

    private void OnDisable() {
        EndSpin();
        TimeHandler.TimeSlow -= EndSpin;
        TimeHandler.TimeResume -= StartSpin;
    }

    private void FindPlayer()
    {
        _weaponRoot = Player.PlayerObject.GetComponent<WeaponRoot>();
    }

    public override void Fire()
    {
        base.Fire();
        
        for (int i = 0; i < _bulletsPerFire; i++)
        {
            SpawnBullet(_barrels[_barrelPool[Random.Range(0, _barrelPool.Length)]].position , 0f);   
        }
    }

    private IEnumerator Spinup()
    {
        while (_spinning)
        {
            if (FireReload > _topFirerate)
            {
                FireReload += _firerateGrowth * Time.deltaTime;
                //_weaponRoot.FireReload = FireReload;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void StartSpin()
    {
        _spinning = true;
        StartCoroutine(Spinup());
    }

    private void EndSpin()
    {
        _spinning = false;
        FireReload = _startFirerate;
        //_weaponRoot.FireReload = FireReload;
    }
}
