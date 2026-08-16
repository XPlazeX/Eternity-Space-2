using UnityEngine;

public class FlowLaserGun : AttackPattern
{
    [SerializeField] private float _distance;
    [SerializeField] private LayerMask _mask;
    [SerializeField] private bool blockWhileSecondary = true;

    private LaserObject _activeLaser;

    private void OnEnable() {
        SledgeDirector.PlayerCatchStarted += StopFiring;
    }

    void OnDisable()
    {
        SledgeDirector.PlayerCatchStarted -= StopFiring;
    }

    protected override void Update()
    {
        LocalLock = blockWhileSecondary && PlayerInput.SecondaryFirePressed;

        base.Update();
    }

    public override void StartFiring()
    {
        if (_activeLaser != null)
        {
            StopFiring();
        }

        base.StartFiring();
        _activeLaser = (LaserObject)SpawnBullet();

        _activeLaser.LaserOn(_barrels[0], _distance, _mask);
        base.Fire();

        MuzzleFlash(_barrels[0].position);
    }

    public override void StopFiring()
    {
        if (_activeLaser == null)
        {
            return;
        }

        base.StopFiring();

        _activeLaser.LaserOff();
        _activeLaser = null;
    }
}
