using UnityEngine;

public delegate void AttackHandler();
public class WeaponRoot : MonoBehaviour
{
    public delegate void weaponOperation();
    public event weaponOperation WeaponCharged;

    private AttackHandler mainAttack; // делегат атаки главного оружия 

    [SerializeField] private AttackPattern _autoLoadWeaponPattern;
    [SerializeField] private Transform[] _barrels;
    [SerializeField] private bool useDefaultUI = false;
    [SerializeField] private DeviceUI mainWeaponUI;
    [SerializeField] private DeviceUI[] mainWeaponEnergyUIs;
    [SerializeField] private DeviceUI secondaryWeaponUI;
    [SerializeField] private bool _noWeaponUI;

    public Transform[] PlayerBarrels => _barrels;

    private DeviceUI _weaponUI;
    public bool CanAttack => Player.Alive && Player.CanAttack;

    private AttackPattern _bindedAttackPattern;
    private Device _bindedDevice;

    private float _preparing = 0f;

    private void OnEnable() {
        ShipStats.StatChanged += ObserveStat;
    }

    private void OnDisable() {
        ShipStats.StatChanged -= ObserveStat;
    }

    private void Start()
    {      
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
        if (useDefaultUI && _bindedAttackPattern != null)
        {
            mainWeaponUI.Fill(_bindedAttackPattern.PrepareNormalized);
            for (int i = 0; i < mainWeaponEnergyUIs.Length; i++)
            {
                mainWeaponEnergyUIs[i].Fill(_bindedAttackPattern.EnergyNormalized + 0.1f * (1f - _bindedAttackPattern.EnergyNormalized));
            }
        }
            
        if (useDefaultUI && _bindedDevice != null)
            secondaryWeaponUI.Fill(_bindedDevice.GetChargeNormalized());
    }

    private void DoAttack()
    {
        mainAttack?.Invoke();
    }

    private void SetPreparing(float val)
    {
    }

    protected virtual void ObserveStat(string name, float val)
    {


    }
}
