using UnityEngine;

public class SledgeCore : MonoBehaviour
{
    public static event System.Action RepairCharged;
    public static event System.Action RepairReleased;
    public static event System.Action FullRepairReleased;
    public static event System.Action<int> RepairChanged; // UI сам подпишется

    [SerializeField] private bool disableRepair;
    [SerializeField] private int baseRepairCharge;
    [SerializeField] private float repairStepMultiplier = 1.15f;

    private int _repairCharge = 0;
    private int _chargeStep = 0;

    private static SledgeCore instance;

    public static int RepairCharge
    {
        get { return instance._repairCharge;}
        private set
        {
            instance._repairCharge = Mathf.Clamp(value, 0, PlayerShipData.MaxHP);
            RepairChanged?.Invoke(instance._repairCharge);
        }
    }

    public static bool HasCharge => RepairCharge > 0;

    private void Awake() 
    {
        instance = this;
    }

    private void OnEnable() 
    {
        if (!disableRepair)
            PlayerRamsHandler.RamSuccess += OnRamSuccess;
    }

    void OnDisable()
    {
        if (!disableRepair)
            PlayerRamsHandler.RamSuccess -= OnRamSuccess;
    }

    private void OnRamSuccess()
    {
        if (!disableRepair)
            Charge();
    }

    public void Charge()
    {
        if (disableRepair) return;
        
        RepairCharge += Mathf.CeilToInt((float)baseRepairCharge * Mathf.Pow(repairStepMultiplier, _chargeStep));

        _chargeStep ++;

        RepairCharged?.Invoke();
    }

    public void ReleaseCharge()
    {
        PlayerShipData.RegenerateHP(RepairCharge);

        if (RepairCharge >= PlayerShipData.MaxHP)
        {
            FullRepairReleased?.Invoke();
        }

        RepairCharge = 0;
        _chargeStep = 0;

        RepairReleased?.Invoke();
    }
}
