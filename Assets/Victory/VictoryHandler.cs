using UnityEngine;

public class VictoryHandler : MonoBehaviour
{
    private const int _auriteExplosionID = 12;
    public const int arcade_aurite_penalty = 25;

    public delegate void victoryAction();

    public static event victoryAction LevelVictored;
    public static event victoryAction MissionVictored;
    public static event victoryAction AddTempCurrency;

    [SerializeField] private GameObject _levelVictoryObject;
    [SerializeField] private GameObject _missionVictoryObject;
    [SerializeField] private GameObject _noDamageVictoryObject;

    public static string CustomSceneOnVictory {get; set;}
    public static string CustomSceneOnDeath {get; set;}
    public static string CustomSceneOnExit {get; set;}
    public static bool CustomTransitionAsDeath {get; set;}
    public static bool ClearDataOnExit {get; set;}

    public int RequiredLevelCountForDocs {get; private set;} = 7;
    public int DocOnLevels {get; private set;} = 5;
    public int TempAurite => _tempAurite;
    public int TempCosmilite => _tempCosmilite;
    public int TempPositronium => _tempPositronium;
    public static bool LevelVictoried {get; private set;} = false;

    private int _tempCosmilite = 0;
    private int _tempPositronium = 0;
    private int _tempAurite = 0;
    private int _healOnVictory = 0;

    private static bool _loadLobby = false;

    public void Initialize()
    {
        // LevelVictored = null;
        // MissionVictored = null;
        LevelVictoried = false;

        _tempCosmilite = 0;
        _tempPositronium = 0;
        _tempAurite = 0;
    }

    public void AddCosmilite(int val)
    {
        _tempCosmilite += val;
        print($"Добавлен временный космилит: {val}");
        AddTempCurrency?.Invoke();

        if (LevelVictoried)
            Bank.PutCash(BankSystem.Currency.Cosmilite, val);
    }

    public static void EnableFastRestartOnDeath()
    {
        CustomSceneOnDeath = "Game";
        DeathUIHandler.NoEraseData = true;
        DeathUIHandler.FastRestart = true;
    }

    public static void EnableRestartLevelOnDeath()
    {
        CustomSceneOnDeath = "MissionMenu";
        DeathUIHandler.NoEraseData = true;
        DeathUIHandler.FastRestart = true;
    }

    public void AddPositronium(int val)
    {
        _tempPositronium += val;
        print($"Добавлен временный позитроний: {val}");
        AddTempCurrency?.Invoke();

        if (LevelVictoried)
            Bank.PutCash(BankSystem.Currency.Positronium, val);
    }

    public void AddAurite(int val)
    {
        _tempAurite += val;
        print($"Добавлен временный аурит: {val}");
        AddTempCurrency?.Invoke();

        if (LevelVictoried)
            Bank.PutCash(BankSystem.Currency.Aurite, val);
    }

    public void AddHealOnVictory(int amount)
    {
        _healOnVictory += amount;
    }

    public void LevelVictory(bool andMission = false)
    {
        if (LevelVictoried)
            return;

        LevelVictoried = true;

        if (_healOnVictory > 0)
            PlayerShipData.RegenerateHP(_healOnVictory);

        PlayerCore.SaveMegawatts();

        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        save.PrepareToNewLevel(); // Current level ++
        ModulasSaveHandler.FlushChoice();

        Bank.PutCash(BankSystem.Currency.Cosmilite, _tempCosmilite);
        Bank.PutCash(BankSystem.Currency.Positronium, _tempPositronium);
        Bank.PutCash(BankSystem.Currency.Aurite, _tempAurite + save.MoneyPerLevel - (PlayerPrefs.GetFloat("GameMode", 0) == 1f ? arcade_aurite_penalty : 0));

        GameSessionInfoHandler.RewriteSessionSave(save);

        LevelVictored?.Invoke();

        if ((save.CurrentLevel != save.MaxLevel) && !andMission)
        {
            _loadLobby = false;
            _levelVictoryObject.SetActive(true);
            print("УРОВЕНЬ МИССИИ ПРОЙДЕН");
        } 
        else
        {
            _loadLobby = true;
            MissionVictory();
        }

        GameObject.FindWithTag("AudioCore").GetComponent<InteriorSoundController>().SetInteriorOST();

        SceneStatics.SceneCore.GetComponent<BusStop>().SpawnBus();

        if (GameSessionInfoHandler.MaxLevel < RequiredLevelCountForDocs || GameSessionInfoHandler.FinalLevel)
            return;

        if (((GameSessionInfoHandler.CurrentLevel + 1) % DocOnLevels) == 0)
        {
            SceneStatics.SceneCore.GetComponent<BusStop>().SpawnDoc();
        }
    }

    public void MissionVictory()
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        _missionVictoryObject.SetActive(true);

        // ID Анлока миссии и подсчёт прогресса уровней маяков ведётся непосредственно из класса Mission
        if (GameSessionInfoHandler.GetSessionSave().NoDamage)
        {
            _noDamageVictoryObject.SetActive(true);
            // Bank.PutCash(BankSystem.Currency.Cosmilite, save.RecievedCosmilite * 2);

            Unlocks.ProgressUnlock(9, 1);

            if (GameSessionInfoHandler.MaxLevel >= 10)
            {
                Unlocks.ProgressUnlock(932, 1);
            }
        } else
        {
            //Bank.PutCash(BankSystem.Currency.Cosmilite, save.RecievedCosmilite);
        }

        //Bank.PutCash(BankSystem.Currency.Positronium, save.RecievedPositronium);

        GameSessionInfoHandler.ClearGameSession();
        CustomSceneOnExit = "Lobby";
        //FinalLevel = true;
        print("МИССИЯ ПРОЙДЕНА");

        MissionVictored?.Invoke();
    }

    public static void VictorySession()
    {
        if (!string.IsNullOrEmpty(CustomSceneOnVictory))
        {
            SceneTransition.SwitchToScene(CustomSceneOnVictory);
            ClearCustomData();
            return;
        }

        ClearCustomData();

        if (_loadLobby)
            SceneTransition.SwitchToScene("Lobby");
        else
            SceneTransition.SwitchToScene("MissionMenu");
    }

    public static void ExitSession()
    {
        if (ClearDataOnExit)
        {
            GameSessionInfoHandler.ClearGameSession();
        }

        if (!string.IsNullOrEmpty(CustomSceneOnExit))
        {
            SceneTransition.SwitchToScene(CustomSceneOnExit);
            ClearCustomData();
            return;
        }

        ClearCustomData();

        SceneTransition.SwitchToScene("MissionMenu");
    }

    public static void LoseSession()
    {
        if (!string.IsNullOrEmpty(CustomSceneOnDeath))
        {
            SceneTransition.SwitchToScene(CustomSceneOnDeath, CustomTransitionAsDeath ? 4 : -1);
            ClearCustomData();
            return;
        }

        ClearCustomData();
        SceneTransition.SwitchToScene("Lobby", 4);
    }

    private static void ClearCustomData()
    {
        CustomSceneOnVictory = null;
        CustomSceneOnDeath = null;
        CustomSceneOnExit = null;
        DeathUIHandler.NoEraseData = false;
        DeathUIHandler.FastRestart = false;
        CustomTransitionAsDeath = false;
        ClearDataOnExit = false;
    }

    #if UNITY_EDITOR
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddAurite(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            LevelVictory();
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            LevelVictory(true);
        } if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            DamageBody[] dbs = GameObject.FindObjectsOfType<DamageBody>();
            for (int i = 0; i < dbs.Length; i++)
            {
                dbs[i].TakeDamage(new DamageSystem.DamageBundle()
                {
                    damageKey = DamageSystem.DamageKey.Everything,
                    damageValue = 20
                });
            }
        }
    }
    #endif
}
