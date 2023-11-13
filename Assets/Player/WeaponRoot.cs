using UnityEngine;

public delegate void AttackHandler();
public class WeaponRoot : MonoBehaviour
{
    private AttackHandler mainAttack; // делегат атаки главного оружия

    [SerializeField] private AttackPattern _defaultWeaponPattern;
    [SerializeField] private AttackPattern _autoLoadWeaponPattern;
    [SerializeField] private Transform[] _barrels;
    [SerializeField] private float _prepareTime;
    [SerializeField] private DeviceUI _customWeaponUI;

    public Transform[] PlayerBarrels => _barrels;

    private DeviceUI _weaponUI;
    private bool Active => Player.Alive;
    public bool Prepared => (_preparing >= _prepareTime) && Active;

    private float _preparing = 0f;

    private void Start()
    {      
        if (_customWeaponUI != null)
        {
            _weaponUI = _customWeaponUI;
        } else
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

    void Update()
    {
        if (!Active)
            return;

        if ((Input.touchCount > 0) || Input.GetMouseButton(0))
        {
            SetPreparing(_preparing += Time.unscaledDeltaTime);
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
        _weaponUI.Fill(_preparing / _prepareTime);
    }

    //private void Deactivate() => Active = false;
}
