using UnityEngine;

public class MultiBarrelDevice : Device
{
    [SerializeField] private int[] _barrelIndexes;

    public override void Fire()
    {
        for (int i = 0; i < _barrelIndexes.Length; i++)
        {
            SpawnBullet(_barrels[_barrelIndexes[i]].position, 0);
        }
    }
}
