using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetWeaponUI : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private string targetWeaponID;
    [Header("Device")]
    [SerializeField] private string targetDeviceID;

    protected AttackPattern _targetWeapon;
    protected Device _targetDevice;

    private void OnEnable() 
    {
        GameObject weaponHandlerObject = GameObject.FindGameObjectWithTag("WeaponHandler");

        if (_targetWeapon == null)
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
        if (_targetWeapon == null)
        {
            Debug.LogError($"Не найден AttackPattern с ID={targetWeaponID}");
        }

        if (_targetDevice == null)
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
        if (_targetDevice == null)
        {
            Debug.LogError($"Не найден Device с ID={targetDeviceID}");
        }
    }
}


