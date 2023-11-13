using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserDevice : Device
{
    [SerializeField] private int[] _barrelIndexes;
    [SerializeField] private LayerMask _mask;
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _laserLifetime;

    public override void Fire()
    {
        for (int i = 0; i < _barrelIndexes.Length; i++)
        {
            SpawnLaser(_barrels[_barrelIndexes[i]]);
        }
    }

    private void SpawnLaser(Transform barrel)
    {
        LaserObject laserObject = (LaserObject)CharacterBulletDatabase.GetAttackObject(_bulletIndex);

        laserObject.CreateLaser(barrel, _attackDistance, _mask, _laserLifetime);
    }
}
