using UnityEngine;

public class DisraptorDevice : Device
{
    [SerializeField] private float cooldown = 5f;
    [SerializeField] private int bulletsPerBarrel = 4;

    private float _cooldown;

    public override float GetChargeNormalized()
    {
        return Mathf.Clamp01(1f - (_cooldown / cooldown));
    }

    public override float GetChargeRaw()
    {
        return _cooldown;
    }

    protected override void Update()
    {
        _cooldown -= ESTime.worldDeltaTime;

        base.Update();
        if (!MainWeaponHandler.CanUseWeapons) return;

        if (Active && PlayerInput.SecondaryFireDown && _cooldown <= 0f)
        {
            StartRelease();
        }
    }

    public override void StartRelease()
    {
        _cooldown = cooldown;

        for (int i = 0; i < _barrels.Length; i++)
        {
            for (int l = 0; l < bulletsPerBarrel; l++)
            {
                SpawnBullet(_barrels[i].position, Player.Orientation.eulerAngles.z);
            }
            MuzzleFlash(_barrels[i].position);
        }

        PlaySound();

        base.StartRelease();
    }
}
