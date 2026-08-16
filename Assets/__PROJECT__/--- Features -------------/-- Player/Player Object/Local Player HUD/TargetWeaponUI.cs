using UnityEngine;

public class TargetWeaponUI : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private bool useWeapon;
    [SerializeField] private string targetWeaponID;
    [SerializeField] private WeaponFillingBind[] weaponFillBinds;
    [Header("Device")]
    [SerializeField] private bool useDevice;
    [SerializeField] private string targetDeviceID;
    [SerializeField] private SimpleFillingUI[] deviceFillUIs;

    protected AttackPattern _targetWeapon;
    protected Device _targetDevice;

    private void OnEnable() 
    {
        GameObject weaponHandlerObject = GameObject.FindGameObjectWithTag("WeaponHandler");

        if (useWeapon && _targetWeapon == null)
        {
            AttackPattern[] attackPatterns = weaponHandlerObject.GetComponentsInChildren<AttackPattern>();

            for (int i = 0; i < attackPatterns.Length; i++)
            {
                if (attackPatterns[i].ID == targetWeaponID)
                {
                    _targetWeapon = attackPatterns[i];
                    break;
                }
            }
        }
        if (useWeapon && _targetWeapon == null)
        {
            Debug.LogError($"Не найден AttackPattern с ID={targetWeaponID}");
        }

        if (useDevice && _targetDevice == null)
        {
            Device[] devices = weaponHandlerObject.GetComponentsInChildren<Device>();

            for (int i = 0; i < devices.Length; i++)
            {
                if (devices[i].ID == targetDeviceID)
                {
                    _targetDevice = devices[i];
                    break;
                }
            }
        }
        if (useDevice && _targetDevice == null)
        {
            Debug.LogError($"Не найден Device с ID={targetDeviceID}");
        }
    }

    void LateUpdate()
    {
        if (useWeapon && _targetWeapon != null)
        {
            for (int i = 0; i < weaponFillBinds.Length; i++)
            {
                switch (weaponFillBinds[i].fillType)
                {
                    case WeaponFillType.Cooldown:
                        weaponFillBinds[i].fillingUI.UpdateState(_targetWeapon.CooldownNormalized, Time.deltaTime, _targetWeapon.CurrentCooldown);
                    break;
                    case WeaponFillType.Energy:
                        weaponFillBinds[i].fillingUI.UpdateState(_targetWeapon.EnergyNormalized, Time.deltaTime, _targetWeapon.CurrentEnergy);
                    break;
                    case WeaponFillType.Preparing:
                        weaponFillBinds[i].fillingUI.UpdateState(_targetWeapon.PrepareNormalized, Time.deltaTime, _targetWeapon.CurrentPrepare);
                    break;
                    default:
                        break;
                }
            }
        }

        if (useDevice && _targetDevice != null)
        {
            for (int i = 0; i < deviceFillUIs.Length; i++)
            {
                deviceFillUIs[i].UpdateState(_targetDevice.GetChargeNormalized(), Time.deltaTime, _targetDevice.GetChargeRaw());
            }
        }
    }

    [System.Serializable]
    public struct WeaponFillingBind
    {
        public WeaponFillType fillType;
        public SimpleFillingUI fillingUI;
    }

    public enum WeaponFillType
    {
        Preparing,
        Energy,
        Cooldown
    }
}

