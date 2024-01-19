using UnityEngine;

public class LaserGun : AttackPattern
{
    [SerializeField] private float _distance;
    [SerializeField] private LayerMask _mask;
    
    public override void Fire()
    {
        LaserObject laser = (LaserObject)SpawnBullet();

        laser.CreateLaser(_barrels[0], _distance, _mask, _firerate);
        base.Fire();
    }


}
