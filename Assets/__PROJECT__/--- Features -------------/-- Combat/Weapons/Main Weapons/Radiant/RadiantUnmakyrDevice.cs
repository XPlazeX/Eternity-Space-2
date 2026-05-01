using UnityEngine;

public class RadiantUnmakyrDevice : Device
{
    [SerializeField] private float fireReload;
    [SerializeField] private Vector2Int minMaxBulletCount;
    [SerializeField] private Vector2 minMaxAngleStep;
    [SerializeField] private float radiantChargePerShotRequirement = 0.5f;

    private bool _releasing;
    private float _reloadTimer;

    public override float GetChargeNormalized()
    {
        return RadiantChargeBank.Charge01;
    }

    public void Fire()
    {
        int bulletCount = Random.Range(minMaxBulletCount.x, minMaxBulletCount.y);
        float angleStep = Random.Range(minMaxAngleStep.x, minMaxAngleStep.y);

        float startAngle = -angleStep * ((bulletCount - 1) / 2f);
        
        for (int i = 0; i < bulletCount; i++)
        {
            SpawnBullet(_barrels[0].position, startAngle + _barrels[0].eulerAngles.z); 
            startAngle += angleStep;
        }

        MuzzleFlash(_barrels[0].position);
    }

    protected override void Update()
    {
        _reloadTimer -= Time.deltaTime;

        base.Update();

        if (_releasing)
        {
            if (!Active)
            {
                EndRelease();
                return;
            }

            Releasing();
        }

        if (Active && PlayerInput.SecondaryFireDown)
        {
            StartRelease();
        }

        else if (Active && PlayerInput.SecondaryFireUp)
        {
            EndRelease();
        }
    }

    public override void StartRelease()
    {
        base.StartRelease();
        _releasing = true;
    }

    public override void Releasing()
    {
        if (!RadiantChargeBank.EnoughtCharge(radiantChargePerShotRequirement)) return;

        if (_reloadTimer <= 0f)
        {
            Fire();
            _reloadTimer = fireReload;
            RadiantChargeBank.ConsumeCharge(radiantChargePerShotRequirement);
        }
    }

    public override void EndRelease()
    {
        base.EndRelease();
        _releasing = false;
    }
}
