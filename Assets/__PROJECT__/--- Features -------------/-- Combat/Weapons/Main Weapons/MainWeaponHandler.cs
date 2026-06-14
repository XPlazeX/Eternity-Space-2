using System;
using UnityEngine;

public class MainWeaponHandler : MonoBehaviour
{
    public static Action WeaponChanged;
    // Задача - держать наши виды оружия и их вариации, включать необходимое
    [SerializeField] private WeaponSlot[] weaponSlots;
    [SerializeField] private bool simpleSystem = true;
    [SerializeField] private Device simpleUseDevice;
    [SerializeField] private AttackPattern[] radiantProgression;

    public static WeaponRoot MainWeaponRoot {get; private set;}

    public static AttackPattern ActiveMainWeapon {get; private set;}
    public static Device ActiveSecondaryDevice {get; private set;}
    public static string ActiveWeaponID {get; private set;}
    public static string ActiveSecondaryID {get; private set;}
    public static int ActiveSlot {get; private set;}
    public static int ActiveSecondary {get; private set;}
    public static bool UsedSimpleDevice {get; private set;}
    public static int RadiantSimpleLevel {get; private set;} = -1;

    public void Initialize() {
        Player.PlayerChanged += OnPlayerChanged;
        OnPlayerChanged();

        if (simpleSystem)
        {
            SelectWeapon(0, 0);
            SetupSimpleUseDevice();
            SetRadiantLevel(0);
        }

        else
        {
            SelectWeapon(0, 0);
        }
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

    public void RadiantUpgrade(out bool overcharged)
    {
        overcharged = false;

        if (!simpleSystem) return;

        if (RadiantSimpleLevel == radiantProgression.Length - 1)
        {
            overcharged = true;
            return;
        }

        SetRadiantLevel(RadiantSimpleLevel + 1);
    }

    public void RadiantDowngrade(out bool overzero)
    {
        overzero = false;

        if (!simpleSystem) return;

        if (RadiantSimpleLevel <= 0)
        {
            overzero = true;
            return;
        }

        SetRadiantLevel(RadiantSimpleLevel - 1);
    }

    private void SetRadiantLevel(int lvl)
    {
        lvl = Mathf.Clamp(lvl, 0, radiantProgression.Length - 1);

        ActiveWeaponID = radiantProgression[lvl].ID;

        ActiveMainWeapon = radiantProgression[lvl];

        MainWeaponRoot.BindTargetAttackPattern(ActiveMainWeapon);
        RadiantSimpleLevel = lvl;

        WeaponChanged?.Invoke();
    }

    private void UpdateMainWeaponRoot()
    {
        MainWeaponRoot = Player.PlayerObject.GetComponent<WeaponRoot>();
    }

    private void PrewarmWeapons()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].MainWeapon != null)
                weaponSlots[i].MainWeapon.Load();

            for (int j = 0; j < weaponSlots[i].ModeCount; j++)
            {
                if (weaponSlots[i].Secondaries[j] != null)
                    weaponSlots[i].Secondaries[j].Load();
            }
        }

        for (int i = 0; i < radiantProgression.Length; i++)
        {
            radiantProgression[i].Load();
        }
    }

    private void SelectWeapon(int slot, int secodary)
    {
        ActiveSlot = slot;
        ActiveSecondary = secodary;

        ActiveWeaponID = weaponSlots[slot].MainWeapon.ID;
        ActiveSecondaryID = weaponSlots[slot].Secondaries[secodary] == null ? "null" : weaponSlots[slot].Secondaries[secodary].ID;

        ActiveMainWeapon = weaponSlots[slot].MainWeapon;
        ActiveSecondaryDevice = weaponSlots[slot].Secondaries[secodary] == null ? null : weaponSlots[slot].Secondaries[secodary];

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
