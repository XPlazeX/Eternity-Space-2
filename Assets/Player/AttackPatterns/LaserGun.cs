using UnityEngine;

public class LaserGun : AttackPattern
{
    [SerializeField] private float _distance;
    [SerializeField] private LayerMask _mask;
    [SerializeField] private float laserLifetime;
    
    public override void Fire()
    {
        LaserObject laser = (LaserObject)SpawnBullet();

        laser.CreateLaser(_barrels[0], _distance, _mask, laserLifetime);
        base.Fire();

        MuzzleFlash(_barrels[0].position);
    }


}
