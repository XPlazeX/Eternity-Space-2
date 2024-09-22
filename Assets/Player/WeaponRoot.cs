using UnityEngine;

public delegate void AttackHandler();
public class WeaponRoot : MonoBehaviour
{
    public delegate void weaponOperation();
    public event weaponOperation WeaponCharged;

    private AttackHandler mainAttack; // делегат атаки главного оружия 

    [SerializeField] private AttackPattern _defaultWeaponPattern;
    [SerializeField] private AttackPattern _autoLoadWeaponPattern;
    [SerializeField] private Transform[] _barrels;
    [SerializeField] private float _prepareTime;
    [SerializeField] private DeviceUI _customWeaponUI;
    [SerializeField] private bool _noWeaponUI;

    public Transform[] PlayerBarrels => _barrels;

    private DeviceUI _weaponUI;
    private bool Active => Player.Alive && Player.CanAttack;
    public bool Prepared => (_preparing >= _prepareTime) && Active;
    public float PrepareSpeed {get; private set;} = 1f;

    private float _preparing = 0f;

    private void OnEnable() {
        ShipStats.StatChanged += ObserveStat;
        PrepareSpeed = ShipStats.GetValue("PrepareTimeMultiplier");
    }

    private void OnDisable() {
        ShipStats.StatChanged -= ObserveStat;
    }

    private void Start()
    {      
        if (_customWeaponUI != null)
        {
            _weaponUI = _customWeaponUI;
        } else if (!_noWeaponUI)
            _weaponUI = GameObject.FindWithTag("WeaponCharge").GetComponent<DeviceUI>();

        if (_defaultWeaponPattern != null)
        {
            AttackPattern loadedModule = Instantiate(_defaultWeaponPattern, transform.position, Quaternion.identity);
            loadedModule.Load();
        }

        if (_autoLoadWeaponPattern != null)
        {
            _autoLoadWeaponPattern.Load();
        }
    }

    public void ReplaceBarrel(int id, Transform newBarrel)
    {
        _barrels[id] = newBarrel;
    }

    void Update()
    {
        if (!Active || Time.timeScale == 0)
            return;

        if ((Input.touchCount > 0) || Input.GetMouseButton(0))
        {
            if ((_preparing < _prepareTime) && (_preparing + Time.unscaledDeltaTime * PrepareSpeed >= _prepareTime))
            {
                WeaponCharged?.Invoke();
            }

            SetPreparing(_preparing += Time.unscaledDeltaTime * PrepareSpeed);
        }
        #if UNITY_EDITOR
            if (Input.GetMouseButtonUp(0))
            {
                SetPreparing(0);
            }
        #elif UNITY_ANDROID
            if (Input.touchCount == 0)
            {
                SetPreparing(0);
            }
        #endif
    }

    private void DoAttack()
    {
        mainAttack?.Invoke();
    }

    private void SetPreparing(float val)
    {
        _preparing = val;
        if (!_noWeaponUI)
            _weaponUI.Fill(_preparing / _prepareTime);
    }

    protected virtual void ObserveStat(string name, float val)
    {
        if (name == "PrepareTimeMultiplier")
        {
            PrepareSpeed = ShipStats.GetValue("PrepareTimeMultiplier");
        }

    }

    //private void Deactivate() => Active = false;
}
