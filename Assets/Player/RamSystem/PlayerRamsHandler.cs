using System.Collections;
using UnityEngine;

public class PlayerRamsHandler : MonoBehaviour
{
    public delegate void ramAction();

    public static event ramAction RamSuccess;

    private static bool CanRam {get; set;} = true;
    private static bool RamShielding {get; set;} = true;
    private static bool RamSaveWaving {get; set;} = true;
    //public static int LowerEnemyHealthLimit {get; set;} = 10;

    public static int MoneyValue {get; set;} = 3;
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
    }

    private void OnDisable() {
        Player.StartPlayerReturn -= FindShield;
        ShipStats.StatChanged -= ObserveStat;
    }

    public static void TryRam()
    {
        if (!CanRam)
            return;

        // if (RamShielding)
        // {
        //     // if (PlayerShipData.CriticalState)
        //     //     PlayerShipData.RegenerateHP(MoneyValue * ShipStats.GetIntValue("RamRegenOnCriticalStateMultiplier"));
        //     // else
        //     //     PlayerShipData.RegenerateHP(MoneyValue);
        // }
        _victoryHandler.AddAurite(MoneyValue);

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
        //print($"invoking ram success. invokation list: {RamSuccess.GetInvocationList().Length}");
    }

    // private void OnUltratechCharged() => RamShielding = false;
    // private void OnUltratechEmpty() => RamShielding = true;

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
        }

    }

}
