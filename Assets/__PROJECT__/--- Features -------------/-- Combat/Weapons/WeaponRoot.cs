using UnityEngine;

public delegate void AttackHandler();
public class WeaponRoot : MonoBehaviour
{
    public delegate void weaponOperation();
    public event weaponOperation WeaponCharged;

    private AttackHandler mainAttack; // делегат атаки главного оружия 

    [SerializeField] private AttackPattern _autoLoadWeaponPattern;
    [SerializeField] private Transform[] _barrels;
    [SerializeField] private DeviceUI mainWeaponUI;
    [SerializeField] private DeviceUI secondaryWeaponUI;
    [SerializeField] private bool _noWeaponUI;

    public Transform[] PlayerBarrels => _barrels;

    private DeviceUI _weaponUI;
    public bool CanAttack => Player.Alive && Player.CanAttack;

    private AttackPattern _bindedAttackPattern;
    private Device _bindedDevice;
    // public bool Prepared => (_preparing >= _prepareTime) && Active;
    // public float PrepareSpeed {get; private set;} = 1f;

    private float _preparing = 0f;

    private void OnEnable() {
        ShipStats.StatChanged += ObserveStat;
    }

    private void OnDisable() {
        ShipStats.StatChanged -= ObserveStat;
    }

    private void Start()
    {      
        // if (_customWeaponUI != null)
        // {
        //     _weaponUI = _customWeaponUI;
        // } else if (!_noWeaponUI)
        //     _weaponUI = GameObject.FindWithTag("WeaponCharge").GetComponent<DeviceUI>();

        if (_autoLoadWeaponPattern != null)
        {
            _autoLoadWeaponPattern.Load();
        }
    }

    public void ReplaceBarrel(int id, Transform newBarrel)
    {
        _barrels[id] = newBarrel;
    }
    
    public void BindTargetAttackPattern(AttackPattern attackPattern)
    {
        _bindedAttackPattern = attackPattern;
    }

    public void BindTargetDevice (Device device)
    {
        _bindedDevice = device;
    }

    void Update()
    {
        mainWeaponUI.Fill(_bindedAttackPattern.PrepareNormalized);
        secondaryWeaponUI.Fill(_bindedDevice.GetChargeNormalized());
    }

    private void DoAttack()
    {
        mainAttack?.Invoke();
    }

    private void SetPreparing(float val)
    {
        // _preparing = val;
        // if (!_noWeaponUI)
        //     _weaponUI.Fill(_preparing / _prepareTime);
    }

    protected virtual void ObserveStat(string name, float val)
    {


    }

    //private void Deactivate() => Active = false;
}
