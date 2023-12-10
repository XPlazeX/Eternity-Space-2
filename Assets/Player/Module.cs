using UnityEngine;
using ModuleWork;
using DamageSystem;

public class Module : MonoBehaviour
{
    [SerializeField] private Sprite _icon;
    public Sprite Icon => _icon;
    
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

    [SerializeField] protected AttackObject _bulletSample;
    [SerializeField] protected float _firerate = 1f;
    [SerializeField] private float _spread = 0;
    [Range(0, 1f)][SerializeField] protected float _spreadBulletSpeed = 0;
    [SerializeField] protected AudioClip _soundWork;
    [SerializeField][Range(0, 1f)] private float _volume = 1f;
    [SerializeField][Range(0, 2f)] private float _startPitch = 1f;
    [SerializeField][Range(0, 3f)] private float _pitchSpread = 0f;
    [Space()]
    [SerializeField] private WeaponRoot _customWeaponRoot;

    public AttackObject BulletSample => _bulletSample;
    public int CharacterBulletIndex => _bulletIndex;

    protected Transform[] _barrels => _bindedWR.PlayerBarrels; // нужен, для ссылок на Barrels
    protected WeaponRoot _bindedWR;
    //private SoundPlayer _soundPlayer;
    protected int _bulletIndex;
    protected int _normalDamage;
    private float _firerateMultiplier = 1f;
    private float _fireReloading = 0f;
    public float Spread {get; private set;} = 0f;

    public float FireReload
    {
        get { return _firerate; }
        set { _firerate = value; }
    }

    public bool Active => _bindedWR.Prepared;

    public override void Load()
    {
        if (_customWeaponRoot != null)
        {
            _bindedWR = _customWeaponRoot;
        } else
            _bindedWR = Player.PlayerObject.GetComponent<WeaponRoot>();

        _bulletIndex = CharacterBulletDatabase.InitializeAttackSample(_bulletSample);

        _normalDamage = Mathf.CeilToInt(CharacterBulletDatabase.GetForChangeAttackObject(_bulletIndex).Damage);

        ShipStats.StatChanged += ObserveStat;
        _firerateMultiplier = ShipStats.GetValue("MainWeaponFirerateMultiplier");
        Spread = _spread + ShipStats.GetValue("FlatSpread");
    }

    protected virtual void ObserveStat(string name, float val)
    {
        if (name == "MainWeaponFirerateMultiplier")
        {
            _firerateMultiplier = ShipStats.GetValue("MainWeaponFirerateMultiplier");
        } else if (name == "FlatSpread")
        {
            Spread = _spread + ShipStats.GetValue("FlatSpread");
        }

    }

    protected virtual void Update()
    {
        _fireReloading -= Time.deltaTime;
        
        if (!Active)
            return;

        if ((Input.GetMouseButton(0) || (Input.touchCount > 0)) && _fireReloading <= 0)
        {
            Fire();
            _fireReloading = FireReload * (1f / _firerateMultiplier);
        }
    }

    private void OnDisable() {
        ShipStats.StatChanged -= ObserveStat;
    }

    public virtual void Fire()
    {
        Fired?.Invoke();
        SoundPlayer.PlaySound(_soundWork, _volume, Random.Range(_startPitch - _pitchSpread, _startPitch + _pitchSpread));
    }

    protected void SpawnBullet(Vector3 position, float startRotation)
    {
        AttackObject bulletSample = CharacterBulletDatabase.GetAttackObject(_bulletIndex);

        bulletSample.transform.rotation = Quaternion.Euler(0, 0, ShipStats.GetValue("NoSpread") == 1 ? 0 : (startRotation + (Random.Range(-Spread, Spread) * ShipStats.GetValue("SpreadMultiplier"))));
        bulletSample.transform.position = position;

        if (_spreadBulletSpeed != 0)
            ((Bullet)bulletSample).MultiplySpeedParams(1f + (Random.Range(-_spreadBulletSpeed, _spreadBulletSpeed)));
    }

    public AttackObject SpawnBullet()
    {
        return CharacterBulletDatabase.GetAttackObject(_bulletIndex);
    }
}

public class Device : AttackPattern 
{
    // firerate shows charge time

    protected override void Update()
    {
        
    }

    public override void Load()
    {
        _bindedWR = Player.PlayerObject.GetComponent<WeaponRoot>();

        Player.StartPlayerReturn += OnStartPlayerReturned;
        OnStartPlayerReturned();

        SceneStatics.SceneCore.GetComponent<DeviceHandler>().SetDevice(this);

        _bulletIndex = CharacterBulletDatabase.InitializeAttackSample(_bulletSample);

        print($"device setted with bullet id : {_bulletIndex}");

        _normalDamage = Mathf.CeilToInt(CharacterBulletDatabase.GetForChangeAttackObject(_bulletIndex).Damage);
        if (Dev.IsLogging) print($"Load device ---------- normal damage : {_normalDamage}");

        ShipStats.StatChanged += SetDamage;
        SetDamage("DeviceDamageMultiplier", ShipStats.GetValue("DeviceDamageMultiplier"));
    }

    private void OnStartPlayerReturned()
    {
        WeaponRoot weaponRoot = Player.PlayerObject.GetComponent<WeaponRoot>();
    }

    private void SetDamage(string input, float newVal)
    {
        if (input != "DeviceDamageMultiplier")
            return;
        
        AttackObject bullet = CharacterBulletDatabase.GetForChangeAttackObject(_bulletIndex);
        bullet.Damage = Mathf.CeilToInt(_normalDamage * newVal);

        if (Dev.IsLogging) print($"Mod device damage---- current damage : {bullet.Damage}");
    }

    public override void Fire()
    {
        Fired?.Invoke();
        print("Empty Device attack fired!");
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
}