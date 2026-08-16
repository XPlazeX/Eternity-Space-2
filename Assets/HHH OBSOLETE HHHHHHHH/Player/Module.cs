using UnityEngine;
using ModuleWork;
using DamageSystem;

public class Module : MonoBehaviour
{
    [SerializeField] private string moduleID;
    [SerializeField] private Sprite _icon;
    [SerializeField] private bool _selfLoadOnStart = false;

    public Sprite Icon => _icon;
    public string ID => moduleID;
    
    private void Start() 
    {
        if (_selfLoadOnStart)
        {
            Load();
        }
    }

    public virtual void Load() {} // метод загрузки модуля перед боем
    public virtual void Asquiring() {}
    public virtual void MissionMenuLoad() {}
}

public class Gear : Module
{

}

public class AttackPattern : Gear
{
    public CommonAction Fired;
    public CommonAction Prepared;

    [SerializeField] protected AttackObject _bulletSample;
    [SerializeField] protected float prepareTime = 0.5f;
    [SerializeField] protected float _firerate = 1f;
    [SerializeField] private float _spread = 0;
    [Range(0, 1f)][SerializeField] protected float _spreadBulletSpeed = 0;
    [SerializeField] protected AudioClip _soundWork;
    [SerializeField][Range(0, 1f)] private float _volume = 1f;
    [SerializeField][Range(0, 2f)] private float _startPitch = 1f;
    [SerializeField][Range(0, 3f)] private float _pitchSpread = 0f;
    [Space()]
    [SerializeField] private _ExplosionBullet muzzleExplosion;
    [Space()]
    [SerializeField] private float _energyBank = 100f;
    [SerializeField] private float _energyPerFire = 0f;
    [SerializeField] private float _energyRegenerationSpeed = 30f;
    [SerializeField] private WeaponRoot _customWeaponRoot;

    public AttackObject BulletSample => _bulletSample;
    public int CharacterBulletIndex => _bulletIndex;

    protected Transform[] _barrels => _bindedWR.PlayerBarrels; // нужен, для ссылок на Barrels
    protected WeaponRoot _bindedWR;
    
    protected bool _prepared;
    protected int _bulletIndex;
    protected float _currentEnergy = 0f;
    private float _fireReloading = 0f;
    private float _prepareTimer = 0f;

    public float Spread => _spread + RR.Get(RuntimeStat.MainWeaponFlatSpread);
    public float CurrentPrepare => _prepareTimer;
    public float PrepareNormalized => prepareTime <= 0 ? (Active && PlayerInput.MainFirePressed ? 1f : 0f) : Mathf.Clamp01(1f - (_prepareTimer / prepareTime));
    public float CurrentEnergy => _currentEnergy;
    public float EnergyNormalized => Mathf.Clamp01(_currentEnergy / _energyBank);
    public float CurrentCooldown => _fireReloading;
    public float CooldownNormalized => Mathf.Clamp01(_fireReloading / FireReload);
    protected float EnergyPerFire => _energyPerFire;

    public float FireReload
    {
        get { return _firerate; }
        set { _firerate = value; }
    }
    public bool Firing {get; private set;} = false;

    public bool Workable => _bindedWR == null ? false : _bindedWR.CanAttack;
    public bool Active => MainWeaponHandler.ActiveWeaponID == ID;
    public bool Ready => prepareTime > 0f ? _prepareTimer <= 0 : true;
    protected bool LocalLock {get; set;} = false;

    public override void Load()
    {
        if (_customWeaponRoot != null)
        {
            _bindedWR = _customWeaponRoot;
        } else
            _bindedWR = MainWeaponHandler.MainWeaponRoot;

        _currentEnergy = _energyBank;
    }

    protected virtual void Update()
    {
        _currentEnergy = Mathf.Clamp(_currentEnergy + (_energyRegenerationSpeed * ESTime.worldDeltaTime), 0f, _energyBank);
        
        if (!Workable || !MainWeaponHandler.CanUseWeapons) return;

        _fireReloading -= ESTime.worldDeltaTime;

        if (PlayerInput.MainFirePressed && !LocalLock)
        {
            _prepareTimer -= ESTime.worldDeltaTime * RR.Get(RuntimeStat.MainWeaponPrepareTimeMultiplier);
            if (_prepareTimer <= 0f && !_prepared)
            {
                if (prepareTime > 0);
                    Prepared?.Invoke();
                _prepared = true;
            }
        } else
        {
            _prepareTimer = prepareTime;
            _prepared = false;
            if (Firing)
            {
                Firing = false;
                StopFiring();
            }
            return;
        } 
        
        if (!Ready || !Active)
            return;

        if (_fireReloading <= 0 && _currentEnergy >= _energyPerFire)
        {
            if (!Firing)
            {
                Firing = true;
                StartFiring();
            }
            Fire();
            float fr = RR.Get(RuntimeStat.MainWeaponFirerateRandomizing);
            _fireReloading = FireReload * (1f / RR.Get(RuntimeStat.MainWeaponFirerateMultiplier)) * Random.Range(1f / fr, 1f * fr);
            _currentEnergy -= _energyPerFire;
        }
    }

    public virtual void StartFiring()
    {
        
    }

    public virtual void StopFiring()
    {
        
    }

    public virtual void Fire()
    {
        Fired?.Invoke();
        SoundPlayer.PlaySound(_soundWork, _volume, Random.Range(_startPitch - _pitchSpread, _startPitch + _pitchSpread));
    }

    protected AttackObject SpawnBullet(Vector3 position, float startRotation)
    {
        AttackObject bulletSample = Pool.Spawn(_bulletSample);//CharacterBulletDatabase.GetAttackObject(_bulletIndex);

        bulletSample.transform.rotation = Quaternion.Euler(0, 0, ShipStats.GetValue("NoSpread") == 1 ? 0 : (startRotation + (Random.Range(-Spread, Spread) * ShipStats.GetValue("SpreadMultiplier"))));
        bulletSample.transform.position = position;

        if (bulletSample is Bullet)
            ((Bullet)bulletSample).MultiplySpeedParams(1f + Random.Range(-_spreadBulletSpeed, _spreadBulletSpeed), PlayerController.FixedDeltaPosition);

        return bulletSample;
    }

    protected AttackObject SpawnBullet(AttackObject customSample, Vector3 position, float startRotation)
    {
        AttackObject bulletSample = Pool.Spawn(customSample);//CharacterBulletDatabase.GetAttackObject(_bulletIndex);

        bulletSample.transform.rotation = Quaternion.Euler(0, 0, ShipStats.GetValue("NoSpread") == 1 ? 0 : (startRotation + (Random.Range(-Spread, Spread) * ShipStats.GetValue("SpreadMultiplier"))));
        bulletSample.transform.position = position;

        if (bulletSample is Bullet)
            ((Bullet)bulletSample).MultiplySpeedParams(1f + (Random.Range(-_spreadBulletSpeed, _spreadBulletSpeed)), PlayerController.FixedDeltaPosition);

        return bulletSample;
    }

    protected void MuzzleFlash(Vector3 position)
    {
        muzzleExplosion.SpawnExplosion(position);
    }

    public AttackObject SpawnBullet()
    {
        return Pool.Spawn(_bulletSample);
    }
}

public class Device : Gear 
{
    public CommonAction Charged;
    public CommonAction StartedRelease;
    public CommonAction EndedRelease;

    [SerializeField] protected AttackObject _bulletSample;
    [SerializeField] private float _spread = 0;
    [Range(0, 1f)][SerializeField] protected float _spreadBulletSpeed = 0;
    [SerializeField] protected AudioClip _soundWork;
    [SerializeField][Range(0, 1f)] private float _volume = 1f;
    [SerializeField][Range(0, 2f)] private float _startPitch = 1f;
    [SerializeField][Range(0, 3f)] private float _pitchSpread = 0f;
    [Space()]
    [SerializeField] private _ExplosionBullet muzzleExplosion;
    [Space()]
    [SerializeField] private WeaponRoot _customWeaponRoot;

    public AttackObject BulletSample => _bulletSample;
    public int CharacterBulletIndex => _bulletIndex;

    protected Transform[] _barrels => _bindedWR.PlayerBarrels; // нужен, для ссылок на Barrels
    protected WeaponRoot _bindedWR;
    public float Spread => _spread + RR.Get(RuntimeStat.MainWeaponFlatSpread);

    public bool Workable => _bindedWR == null ? false : _bindedWR.CanAttack;
    public bool Active => MainWeaponHandler.ActiveSecondaryID == ID;

    protected int _bulletIndex;
    protected int _normalDamage;
    protected float _chargeTimer;
    private bool _released = false;

    protected virtual void Update()
    {
        if (!Workable || !MainWeaponHandler.CanUseWeapons) return;

    }

    public virtual float GetChargeNormalized()
    {
        return 0f;
    }

    public virtual float GetChargeRaw()
    {
        return 0f;
    }

    public override void Load()
    {
        if (_customWeaponRoot != null)
        {
            _bindedWR = _customWeaponRoot;
        } else
            _bindedWR = Player.PlayerObject.GetComponent<WeaponRoot>();

        _bindedWR = MainWeaponHandler.MainWeaponRoot;
    }

    public virtual void StartRelease()
    {
        StartedRelease?.Invoke();
    }
    public virtual void Releasing()
    {
        
    }
    public virtual void EndRelease()
    {
        EndedRelease?.Invoke();
    }

    protected void PlaySound() => SoundPlayer.PlaySound(_soundWork, _volume, Random.Range(_startPitch - _pitchSpread, _startPitch + _pitchSpread));

    protected void SpawnBullet(Vector3 position, float startRotation)
    {
        AttackObject bulletSample = Pool.Spawn(_bulletSample);

        bulletSample.transform.rotation = Quaternion.Euler(0, 0, ShipStats.GetValue("NoSpread") == 1 ? 0 : (startRotation + (Random.Range(-Spread, Spread) * ShipStats.GetValue("SpreadMultiplier"))));
        bulletSample.transform.position = position;

        if (_spreadBulletSpeed != 0)
            ((Bullet)bulletSample).MultiplySpeedParams(1f + (Random.Range(-_spreadBulletSpeed, _spreadBulletSpeed)), PlayerController.FixedDeltaPosition);
    }

    public AttackObject SpawnBullet()
    {
        return Pool.Spawn(_bulletSample);
    }

    protected void MuzzleFlash(Vector3 position)
    {
        muzzleExplosion.SpawnExplosion(position);
    }

    public enum UseCondition
    {
        Free = 0,
        Cooldown = 1,
        Toggle = 2,
        MainFiringLoad = 3,
        SecondaryPress = 4
    }

    public enum ReleaseFlow
    {
        Discrete = 0,
        Continious = 1
    }
}

public class Core : Gear
{
    [SerializeField] protected float _maxMegawatts;
    [SerializeField] protected float _startMegawatts;
    [SerializeField] protected float _megawattsGrowth;

    public float MaxMegawatts => _maxMegawatts;
    public float StartMegawatts => _startMegawatts;
    public float Effeciency {get; private set;}

    public override void Load()
    {
        Effeciency = ShipStats.GetValue("CoreEffeciency");
        ShipStats.StatChanged += ObserveStat;
    }

    protected virtual void OnDisable() {
        ShipStats.StatChanged -= ObserveStat;
    }

    protected virtual void ObserveStat(string name, float val)
    {
        if (name == "CoreEffeciency")
        {
            Effeciency = ShipStats.GetValue("CoreEffeciency");
        } 
    }
}

public class Ability : Gear
{
    public delegate void useAction();
    public event useAction AbilityUsed;

    [SerializeField] protected float _energyConsume;
    [SerializeField] protected float _reloadTime;
    [SerializeField] protected bool _bindToPlayer;
    [SerializeField] private SoundObject _soundWork;

    public float EnergyConsume => _energyConsume;
    public float ReloadTime => _reloadTime;

    private AbilityUI _abilityUI;
    private float _timer;

    public override void Load()
    {
        _abilityUI = SceneStatics.UICore.GetComponent<AbilityUI>();
        _abilityUI.SetStats(this);

        _abilityUI.ControlButton.onClick.AddListener(TryUse);

        if (_bindToPlayer)
        {
            transform.position = Player.PlayerTransform.position;
            transform.SetParent(Player.PlayerTransform);
        }
    }

    private void Update()
    {
        _timer -= ESTime.worldDeltaTime;

        _abilityUI.ToggleInteractable((_timer <= 0f && PlayerCore.EnoughtEnergy(_energyConsume)));

        _abilityUI.SetFillIcon(Mathf.Clamp01(1f - (_timer / _reloadTime)));
    }

    public virtual void TryUse()
    {
        if (PlayerCore.EnoughtEnergy(_energyConsume) && _timer <= 0f)
        {
            PlayerCore.ConsumeEnergy(_energyConsume);
            SoundPlayer.PlaySound(_soundWork);
            
            Use();

            _timer = _reloadTime;
        }
    }

    public virtual void Use()
    {
        AbilityUsed?.Invoke();
    }
}

public class Ultratech : Gear
{
    public delegate void useAction();
    public event useAction UTUsed;

    [SerializeField] protected GameObject _transformedShip;
    [SerializeField] protected int _ramsReload;
    [SerializeField] protected float _transformationDuration;

    public GameObject TransformedShip => _transformedShip;
    public int RamsReload => _ramsReload;
    public float TransformationDuration => _transformationDuration;

    public override void Load()
    {
        UltratechRoot ultratechRoot = SceneStatics.CharacterCore.GetComponent<UltratechRoot>();
        ultratechRoot.Initialize(this);
    }

    public virtual void OnUse() 
    {
        UTUsed?.Invoke();
    }
}

namespace ModuleWork
{
    public delegate void CommonAction();

    public enum GearType
    {
        Passive = 0,
        Weapon = 1,
        Device = 2,
        Ultratech = 3
    }  

    public enum ModuleStackType
    {
        NoneStacks = 0,
        Stacking = 1,
        Pack = 2
    }
}