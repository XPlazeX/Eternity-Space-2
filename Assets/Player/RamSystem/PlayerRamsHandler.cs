using UnityEngine;

public class PlayerRamsHandler : MonoBehaviour
{
    public delegate void ramAction();

    public static event ramAction RamSuccess;

    private static bool CanRam {get; set;} = true;
    private static bool RamShielding {get; set;} = true;
    private static bool RamSaveWaving {get; set;} = true;

    public static int MoneyValue {get; set;} = 3;
    public static int HealValue {get; set;} = 0;
    private static RamShield _ramShield;
    private static VictoryHandler _victoryHandler;

    public void Initialize() 
    {
        _victoryHandler = SceneStatics.CharacterCore.GetComponent<VictoryHandler>();

        RamShielding = false;
        RamSaveWaving = false;

        Player.StartPlayerReturn += FindShield;
        ShipStats.StatChanged += ObserveStat;
        
        FindShield();

        MoneyValue = ShipStats.GetIntValue("RamMoneyValue");
        HealValue = ShipStats.GetIntValue("RamHealValue");
    }

    private void OnDisable() {
        Player.StartPlayerReturn -= FindShield;
        ShipStats.StatChanged -= ObserveStat;
    }

    public static void TryRam()
    {
        if (!CanRam)
            return;

        _victoryHandler.AddAurite(MoneyValue);
        if (HealValue > 0)
            PlayerShipData.RegenerateHP(HealValue);

        if (RamShielding && _ramShield != null)
        {
            _ramShield.EnableShield(false);

            if (RamSaveWaving)
            {
                ParringObject exp = ParryingHandler.GetParringObject(0);
                exp.transform.position = _ramShield.transform.position;
            }
        }
            
        RamSuccess?.Invoke();
    }

    private void FindShield()
    {
        _ramShield = Player.PlayerObject.GetComponentInChildren<RamShield>();
        if (_ramShield == null)
            throw new System.Exception("Не найден RamShield для PlayerRamsHandler");
    }

    private void ObserveStat(string name, float val)
    {
        if (name == "RamMoneyValue")
        {
            MoneyValue = ShipStats.GetIntValue("RamMoneyValue");
            print($"Ram money value : {MoneyValue}");
        } else if (name == "RamHealValue")
        {
            HealValue = ShipStats.GetIntValue("RamHealValue");
            print($"Ram heal value : {HealValue}");
        }

    }

}
