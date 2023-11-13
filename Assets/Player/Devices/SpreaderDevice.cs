using UnityEngine;

public class SpreaderDevice : Device
{
    [SerializeField] private int _countBullets;
    [SerializeField] private float _angleStep;

    public override void Fire()
    {
        float startAngle = -(_angleStep * 0.5f) * (_countBullets - 1);

        for (int i = 0; i < _countBullets; i++)
        {
            SpawnBullet(_barrels[0].position, startAngle);
            startAngle += _angleStep;
        }
    }
}