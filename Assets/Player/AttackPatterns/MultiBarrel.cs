using System.Collections;
using UnityEngine;

public class MultiBarrel : AttackPattern
{
    [SerializeField] private int[] _barrelIndexes;

    public override void Fire()
    {
        base.Fire();
        
        for (int i = 0; i < _barrelIndexes.Length; i++)
        {
            SpawnBullet(_barrels[_barrelIndexes[i]].position, _barrels[_barrelIndexes[i]].eulerAngles.z);
        }
    }
}
