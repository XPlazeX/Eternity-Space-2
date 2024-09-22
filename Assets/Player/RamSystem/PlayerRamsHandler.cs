using UnityEngine;
using System.Collections.Generic;

public class PlayerRamsHandler : MonoBehaviour
{
    public const int ram_shielding_unlock_ID = 561;

    public delegate void ramAction();

    public static event ramAction RamSuccess;

    [SerializeField] private PullableObject[] _ramPoolableObjects;
    [SerializeField] private SoundObject _ramSuccesSound;

    private static List<PullForObjects> RamObjectPools = new List<PullForObjects>();

    private static bool CanRam {get; set;} = true;
    private static bool RamShielding {get; set;} = true;
    private static bool RamSaveWaving {get; set;} = true;

    public static int MoneyValue {get; set;} = 3;
    public static int MoneyPerEnemy {get; set;} = 0;
    public static int CosmiliteMoneyValue {get; set;} = 3;
    public static int HealValue {get; set;} = 0;
    public static int DecadesBlockForRam {get; private set;} = 1;
    private static RamShield _ramShield;
    private static VictoryHandler _victoryHandler;
    private static SoundObject _ramSound;

    public void Initialize() 
    {
        RamObjectPools.Clear();

        for (int i = 0; i < _ramPoolableObjects.Length; i++)
        {
            RamObjectPools.Add(new PullForObjects(_ramPoolableObjects[i]));
        }

        _victoryHandler = SceneStatics.CharacterCore.GetComponent<VictoryHandler>();

        _ramSound = _ramSuccesSound;

        RamShielding = Unlocks.HasUnlock(ram_shielding_unlock_ID);
        RamSaveWaving = false;

        Player.StartPlayerReturn += FindShield;
        ShipStats.StatChanged += ObserveStat;
        
        FindShield();

        MoneyValue = ShipStats.GetIntValue("RamMoneyValue") + Mathf.Clamp(GameSessionInfoHandler.CurrentLevel, 0, 7);
        MoneyPerEnemy = ShipStats.GetIntValue("RamMoneyPerEnemy");
        CosmiliteMoneyValue = ShipStats.GetIntValue("RamCosmiliteValue");
        HealValue = ShipStats.GetIntValue("RamHealValue");
        DecadesBlockForRam = ShipStats.GetIntValue("DecadesBlockForRam");
    }

    private void OnDisable() {
        Player.StartPlayerReturn -= FindShield;
        ShipStats.StatChanged -= ObserveStat;
    }

    #if UNITY_EDITOR
    private void Update() {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TryRam();
        }
    }
    #endif

    public static void TryRam()
    {
        if (!CanRam)
            return;

        if (GameSessionInfoHandler.FinalLevel)
            _victoryHandler.AddCosmilite(CosmiliteMoneyValue);
        else
            _victoryHandler.AddAurite(MoneyValue + MoneyPerEnemy * Spawner.EnemyCount);

        
        if (HealValue > 0)
            PlayerShipData.RegenerateHP(HealValue);

        if (PlayerShipData.CriticalState)
            PlayerShipData.RegenerateHP(PlayerShipData.CriticalStateBorder - PlayerShipData.HitPoints);

        if (RamShielding && _ramShield != null)
        {
            _ramShield.EnableShield(false);

            if (RamSaveWaving)
            {
                ParringObject exp = ParryingHandler.GetParringObject(0);
                exp.transform.position = _ramShield.transform.position;
            }
        }
        
        SpawnRamObject(0, Player.PlayerTransform.position);

        SoundPlayer.PlayUISound(_ramSound);

        RamSuccess?.Invoke();

        Unlocks.ProgressUnlock(930, 1);
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
            MoneyValue = ShipStats.GetIntValue("RamMoneyValue") + Mathf.Clamp(GameSessionInfoHandler.CurrentLevel, 0, 7);
            print($"Ram money value : {MoneyValue}");
        } else if (name == "RamHealValue")
        {
            HealValue = ShipStats.GetIntValue("RamHealValue");
            print($"Ram heal value : {HealValue}");
        } else if (name == "RamMoneyPerEnemy")
        {
            MoneyPerEnemy = ShipStats.GetIntValue("RamMoneyPerEnemy");
            print($"Ram money per enemy value : {MoneyPerEnemy}");
        } else if (name == "DecadesBlockForRam")
        {
            DecadesBlockForRam = ShipStats.GetIntValue("DecadesBlockForRam");
            print($"DecadesBlockForRam : {DecadesBlockForRam}");
        }

    }

    public static void SpawnRamObject(int id, Vector3 position)
    {
        GameObject ramObject = RamObjectPools[id].GetGameObject();

        ramObject.transform.position = position;
    }

}
