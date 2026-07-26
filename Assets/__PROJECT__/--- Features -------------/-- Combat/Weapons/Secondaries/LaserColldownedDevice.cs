using UnityEngine;

public class LaserColldownedDevice : Device
{
    [SerializeField] private float cooldown = 3f;
    [SerializeField] private float laserLifetime;
    [SerializeField] private float _distance;
    [SerializeField] private LayerMask _mask;

    private float _cooldown;

    public override float GetChargeNormalized()
    {
        return Mathf.Clamp01(1f - (_cooldown / cooldown));
    }

    protected override void Update()
    {
        _cooldown -= ESTime.worldDeltaTime;

        base.Update();

        if (Active && PlayerInput.SecondaryFireDown && _cooldown <= 0f)
        {
            StartRelease();
            _cooldown = cooldown;
        }
    }

    public override void StartRelease()
    {
        LaserObject laser = (LaserObject)SpawnBullet();

        laser.CreateLaser(_barrels[0], _distance, _mask, laserLifetime);
        base.StartRelease();
        MuzzleFlash(_barrels[0].position);
    }
}
