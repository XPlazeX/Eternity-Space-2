using UnityEngine;

public class BuildupGun : AttackPattern
{
    [SerializeField] private float energyForProjectile = 10f;
    [SerializeField] private float angleStep;

    public override void Fire()
    {
        int bulletCount = Mathf.FloorToInt(Mathf.FloorToInt(_currentEnergy) / energyForProjectile);

        if (bulletCount <= 0)
            return;

        base.Fire();

        _currentEnergy -= EnergyPerFire * (bulletCount - 1);

        float startAngle = -angleStep * ((bulletCount - 1) / 2f);
        
        for (int i = 0; i < bulletCount; i++)
        {
            SpawnBullet(_barrels[0].position, startAngle + _barrels[0].eulerAngles.z);
            startAngle += angleStep;
        }

        MuzzleFlash(_barrels[0].position);
    }
}
