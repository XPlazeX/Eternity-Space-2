using UnityEngine;

public class ChaosGun : Minigun
{
    public override void Fire()
    {
        base.Fire();
        
        for (int i = 0; i < Random.Range(1, 5); i++)
        {
            SpawnBullet(_barrels[Random.Range(0, _barrels.Length)].position, 0);
        }
    }
}
