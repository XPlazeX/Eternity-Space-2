using UnityEngine;

public class GearsToWeaponsTranslator : MonoBehaviour
{
    [SerializeField] private MainWeaponHandler mainWeaponHandler;
    [Header("Railgun & Armorbreaker")]
    [SerializeField] private bool disableRailgun = false;
    [SerializeField] private GearSlot railgunGear = GearSlot.First;
    [SerializeField] private Vector2 railgunSlot = new Vector2(0, 0);
    [Header("VMKT & Disraptor")]
    [SerializeField] private bool disableVMKT = false;
    [SerializeField] private GearSlot vmktGear = GearSlot.Second;
    [SerializeField] private Vector2 vmktSlot = new Vector2(1, 0);
    [Header("Cutter & Regenerator")]
    [SerializeField] private bool disableCutter = false;
    [SerializeField] private GearSlot cutterGear = GearSlot.Third;
    [SerializeField] private Vector2 cutterSlot = new Vector2(2, 0);
    [Header("Scanner & Gamma")]
    [SerializeField] private bool disableScanner = false;
    [SerializeField] private GearSlot scannerGear = GearSlot.Fourth;
    [SerializeField] private Vector2 scannerSlot = new Vector2(3, 0);
    [Header("Magnetite & Manipulator")]
    [SerializeField] private bool disableMagnetite = false;
    [SerializeField] private GearSlot magnetiteGear = GearSlot.Fifth;
    [SerializeField] private Vector2 magnetiteSlot = new Vector2(4, 0);
    [Header("Barrier Deployer & Synchronizer")]
    [SerializeField] private bool disableBarrierDeployer = false;
    [SerializeField] private GearSlot barrierDeployerGear = GearSlot.Sixth;
    [SerializeField] private Vector2 barrierDeployerSlot = new Vector2(5, 0);


    void OnEnable()
    {
        FGB.GearChanged += OnGearChanged;
    }

    void OnDisable()
    {
        FGB.GearChanged -= OnGearChanged;
    }

    private void OnGearChanged(GearSlot gear)
    {
        if (mainWeaponHandler == null) return;

        switch (gear)
        {
            case GearSlot.Neutral:
                mainWeaponHandler.SetNeutral();
                return;
            
            case GearSlot.First:
                if (disableRailgun) return;
                break;
            case GearSlot.Second:
                if (disableVMKT) return;
                break;
            case GearSlot.Third:
                if (disableCutter) return;
                break;
            case GearSlot.Fourth:
                if (disableScanner) return;
                break;
            case GearSlot.Fifth:
                if (disableMagnetite) return;
                break;
            case GearSlot.Sixth:
                if (disableBarrierDeployer) return;
                break;
            default:
                break;
        }

        int slot = (int)gear;
        int secondary = 0;

        mainWeaponHandler.SelectWeapon(slot, secondary);
    }
}
