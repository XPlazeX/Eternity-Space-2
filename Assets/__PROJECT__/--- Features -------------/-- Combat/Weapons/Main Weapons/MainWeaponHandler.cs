using System;
using UnityEngine;

public class MainWeaponHandler : MonoBehaviour
{
    public static Action WeaponChanged;
    // Задача - держать наши виды оружия и их вариации, включать необходимое
    [SerializeField] private WeaponSlot[] weaponSlots;
    [SerializeField] private Device simpleUseDevice;

    public static WeaponRoot MainWeaponRoot {get; private set;}

    public static AttackPattern ActiveMainWeapon {get; private set;}
    public static Device ActiveSecondaryDevice {get; private set;}
    public static string ActiveWeaponID {get; private set;}
    public static string ActiveSecondaryID {get; private set;}
    public static int ActiveSlot {get; private set;}
    public static int ActiveSecondary {get; private set;}
    public static bool UsedSimpleDevice {get; private set;}

    public void Initialize() {
        Player.PlayerChanged += OnPlayerChanged;
        OnPlayerChanged();
        SelectWeapon(0, 0);
        SetupSimpleUseDevice();
    }

    private void OnDisable() {
        Player.PlayerChanged -= OnPlayerChanged;
    }

    private void SetupSimpleUseDevice()
    {
        // condition
        if (simpleUseDevice == null) return;
        
        simpleUseDevice.Load();
        UsedSimpleDevice = true;
    }

    // private void Start() 
    // {
    //     SelectWeapon(0, 0);
    // }

    private void UpdateMainWeaponRoot()
    {
        MainWeaponRoot = Player.PlayerObject.GetComponent<WeaponRoot>();
    }

    private void PrewarmWeapons()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            weaponSlots[i].MainWeapon.Load();
            for (int j = 0; j < weaponSlots[i].ModeCount; j++)
            {
                weaponSlots[i].Secondaries[j].Load();
            }
        }
    }

    private void SelectWeapon(int slot, int secodary)
    {
        ActiveSlot = slot;
        ActiveSecondary = secodary;

        ActiveWeaponID = weaponSlots[slot].MainWeapon.ID;
        ActiveSecondaryID = weaponSlots[slot].Secondaries[secodary].ID;

        ActiveMainWeapon = weaponSlots[slot].MainWeapon;
        ActiveSecondaryDevice = weaponSlots[slot].Secondaries[secodary];

        MainWeaponRoot.BindTargetAttackPattern(ActiveMainWeapon);
        MainWeaponRoot.BindTargetDevice(ActiveSecondaryDevice);

        WeaponChanged?.Invoke();
    }

    private void OnPlayerChanged()
    {
        UpdateMainWeaponRoot();
        PrewarmWeapons();
    }
}

[System.Serializable]
public struct WeaponSlot
{
    [SerializeField] private AttackPattern attackPattern;
    [SerializeField] private Device[] devices;

    public AttackPattern MainWeapon => attackPattern;
    public Device[] Secondaries => devices;
    public int ModeCount => devices.Length;
}
