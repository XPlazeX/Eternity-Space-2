using System.Collections;
using UnityEngine;

public class Spreader : AttackPattern
{
    [SerializeField] private int _bulletCount;
    [SerializeField] private float _angleStep;

    public override void Fire()
    {
        base.Fire();
        float startAngle = -_angleStep * ((_bulletCount - 1) / 2f);
        
        for (int i = 0; i < _bulletCount; i++)
        {
            SpawnBullet(_barrels[0].position, startAngle);
            startAngle += _angleStep;
        }
    }
}