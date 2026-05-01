using System.Collections.Generic;

[System.Serializable]
public class GameSessionSave
{
    private const string default_weapon = "Blaster";
    private const string default_device = "UpsetSlingshot";

    public bool SessionInitialized {get; private set;} = false;
    public int GameMode {get; set;} = 0;
    public int Seed {get; private set;}
    public bool NoDamage {get; set;} = true;
    
    public int LocationID {get; set;} = 0;
    public string LocalizedLocationName {get; set;} = "Null-0";
    public int DialogueEntry {get; set;} = 0;

    public int CurrentLevel {get; set;} = 0;
    public int MaxLevel {get; set;} = 2;
    public bool Boosted {get; set;} = false;
    public int BeaconBG {get; set;} = -1;
    public int WeightSelectorID {get; set;} = 1;
    public bool EnterNewLevel {get; private set;} = true;

    public bool VectorError {get; set;} = false;

    public int ShipModel {get; set;} = 0;
    public string WeaponModel {get; set;} = default_weapon;
    public string DeviceModel {get; set;} = default_device;
    public int WeaponLevel {get; set;} = 0;
    public int CoreModel {get; set;} = 0;
    public int AbilityID {get; set;} = 0;

    public int MaxHealth {get; set;} = 66;
    public int HealthPoints {get; set;} = 66;

    public float Megawatts {get; set;} = 1.4f;
    public float MaxMegawatts {get; set;} = 4.4f;

    public int MoneyPerLevel {get; set;} = 100;
    public int HealsCount {get; set;} = 0;
    public int Money {get; set;} = 50;
    public float RepairAdditivePart {get; set;} = 0f;
    public float AdditiveHardness {get; set;} = 0f;

    public int RecievedCosmilite {get; set;} = 0;
    public int RecievedPositronium {get; set;} = 0;
    public int LevelUnlockCode {get; set;} = 0; // добавляется через VictoryHandler

    public Dictionary<string, List<int>> AdditiveData {get; set;} = new Dictionary<string, List<int>>();

    // для уровней - маяков
    public float BeaconColorValue {get; set;}
    public int BeaconFlipState {get; set;}

    public GameSessionSave(int gameMode = 0)
    {
        Seed = UnityEngine.Random.Range(0, 99999999);
        GameMode = gameMode;

        BeaconBG = -1;
        SessionInitialized = false;
        BeaconColorValue = UnityEngine.Random.value;
        BeaconFlipState = UnityEngine.Random.Range(0, 4);
    }

    public void AllSysInitialized()
    {
        SessionInitialized = true;
    }

    public void NewLevelLoaded() => EnterNewLevel = false;

    public void PrepareToNewLevel() // вызывается при выгрузке пройденного уровня.
    {
        EnterNewLevel = true;
        CurrentLevel ++;
        Seed ++;
    }

    #if UNITY_EDITOR
    public void ChangeSeed()
    {
        Seed = UnityEngine.Random.Range(0, 99999999);
        UnityEngine.Debug.Log($"Editor seed change : {Seed}");
        BeaconColorValue = UnityEngine.Random.value;
        BeaconFlipState = UnityEngine.Random.Range(0, 4);
        BeaconBG = -1;
    }
    #endif
}