using UnityEngine;

public class SimpleRocketsDevice : Device
{
    [SerializeField] private float cooldown = 15f;
    [SerializeField] private BarrelAngle[] barrelIndexesAngles;

    private float _cooldown;

    public override float GetChargeNormalized()
    {
        return Mathf.Clamp01(1f - (_cooldown / cooldown));
    }

    protected override void Update()
    {
        _cooldown -= Time.deltaTime;

        base.Update();

        if (MainWeaponHandler.UsedSimpleDevice)
        {
            if (PlayerInput.NextReleased && _cooldown <= 0f)
            {
                StartRelease();
                _cooldown = cooldown;
                return;
            }
        }

        if (Active && PlayerInput.SecondaryFireDown && _cooldown <= 0f)
        {
            StartRelease();
            _cooldown = cooldown;
        }
    }

    public override void StartRelease()
    {
        for (int i = 0; i < barrelIndexesAngles.Length; i++)
        {
            SpawnBullet(_barrels[barrelIndexesAngles[i].barrelIndex].position, barrelIndexesAngles[i].angle);
            MuzzleFlash(_barrels[barrelIndexesAngles[i].barrelIndex].position);
        }

        base.StartRelease();
        
    }

    [System.Serializable]
    private struct BarrelAngle
    {
        public int barrelIndex;
        public float angle;
    }
}
