using DamageSystem;
using UnityEngine;

public class RegeneratorDevice : Device
{
    [SerializeField] private float _distance;
    [SerializeField] private LayerMask _mask;
    [Header("Rebuilder")]
    [SerializeField] protected AttackObject _rebuildingLaser;
    [SerializeField] protected AudioClip _rebuildSoundWork;
    [SerializeField][Range(0, 1f)] private float _rebuildVolume = 1f;
    [SerializeField][Range(0, 2f)] private float _rebuildStartPitch = 1f;
    [SerializeField][Range(0, 3f)] private float _rebuildPitchSpread = 0f;
    [Space()]
    [SerializeField] private _ExplosionBullet rebuildMuzzleExplosion;
    [SerializeField] private float _rebuilderDistance;
    [SerializeField] private LayerMask _rebuilderMask;

    private LaserObject _activeLaser;
    private LaserObject _activeRebuilder;

    private void OnEnable() {
        SledgeDirector.PlayerCatchStarted += EndRelease;
        SledgeDirector.PlayerCatchStarted += EndRebuilder;
    }

    void OnDisable()
    {
        SledgeDirector.PlayerCatchStarted -= EndRelease;
        SledgeDirector.PlayerCatchStarted -= EndRebuilder;
    }

    public override float GetChargeNormalized()
    {
        return (_activeLaser == null && _activeRebuilder == null) ? 0f : 1f;
    }

    public override float GetChargeRaw()
    {
        return (_activeLaser == null && _activeRebuilder == null) ? 0f : 1f;
    }
    protected override void Update()
    {
        base.Update();
        if (!MainWeaponHandler.CanUseWeapons) return;

        if (Active && PlayerInput.SecondaryFireDown)
        {
            if (PlayerInput.MainFirePressed)
            {
                StartRebuilder();
            } else
            {
                StartRelease();
            }
            
        }
        else if (Active && PlayerInput.SecondaryFireUp)
        {
            if (_activeLaser != null)
            {
                EndRelease();
            } else if (_activeRebuilder != null)
            {
                EndRebuilder();
            }
        } else if (Active && _activeLaser != null && PlayerInput.MainFirePressed)
        {
            EndRelease();
            StartRebuilder();
        }
        else if (Active && _activeRebuilder != null && PlayerInput.MainFireUp)
        {
            EndRebuilder();
            StartRelease();
        }
    }

    public override void StartRelease()
    {
        if (_activeLaser != null)
        {
            EndRelease();
        }

        base.StartRelease();
        _activeLaser = (LaserObject)SpawnBullet();

        _activeLaser.LaserOn(_barrels[0], _distance, _mask);

        MuzzleFlash(_barrels[0].position);
    }

    public override void EndRelease()
    {
        if (_activeLaser == null)
        {
            return;
        }

        base.EndRelease();

        _activeLaser.LaserOff();
        _activeLaser = null;
    }

    public void StartRebuilder()
    {
        if (_activeRebuilder != null)
        {
            EndRebuilder();
        }

        _activeRebuilder = (LaserObject)SpawnRebuildLaser();

        _activeRebuilder.LaserOn(_barrels[0], _rebuilderDistance, _rebuilderMask);

        RebuildMuzzleFlash(_barrels[0].position);
    }

    public void EndRebuilder()
    {
        if (_activeRebuilder == null)
        {
            return;
        }

        _activeRebuilder.LaserOff();
        _activeRebuilder = null;
    }

    protected void PlayRebuildSound() => SoundPlayer.PlaySound(_soundWork, _rebuildVolume, Random.Range(_rebuildStartPitch - _rebuildPitchSpread, _rebuildStartPitch + _rebuildPitchSpread));

    protected LaserObject SpawnRebuildLaser()
    {
        LaserObject bulletSample = (LaserObject)Pool.Spawn(_rebuildingLaser);

        return bulletSample;
    }

    protected void RebuildMuzzleFlash(Vector3 position)
    {
        rebuildMuzzleExplosion.SpawnExplosion(position);
    }
}
