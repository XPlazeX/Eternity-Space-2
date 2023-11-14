using UnityEngine;

public class VictoryHandler : MonoBehaviour
{
    private const int _auriteExplosionID = 12;
    public delegate void victoryAction();

    public static event victoryAction LevelVictored;
    public static event victoryAction MissionVictored;

    [SerializeField] private GameObject _levelVictoryObject;
    [SerializeField] private GameObject _missionVictoryObject;

    public static string CustomSceneOnVictory {get; set;}
    public static string CustomSceneOnDeath {get; set;}
    public static string CustomSceneOnExit {get; set;}

    private int _tempCosmilite = 0;
    private int _tempPositronium = 0;
    private int _tempAurite = 0;

    private static bool _loadLobby = false;

    public void Initialize()
    {
        LevelVictored = null;
        MissionVictored = null;

        _tempCosmilite = 0;
        _tempPositronium = 0;
        _tempAurite = 0;
    }

    public void AddCosmilite(int val)
    {
        _tempCosmilite += val;
    }

    public void AddPositronium(int val)
    {
        _tempPositronium += val;
    }

    public void AddAurite(int val)
    {
        _tempAurite += val;
        //print("Adding temp aurite");
        //_explosionHandler.SpawnExplosion(Player.PlayerTransform.position, _auriteExplosionID);
    }

    public void LevelVictory(bool andMission = false)
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        save.PrepareToNewLevel(); // Current level ++
        ModulasSaveHandler.FlushChoice();

        //GameSessionInfoHandler.DischargeLevel();

        save.RecievedCosmilite += _tempCosmilite;
        save.RecievedPositronium += _tempPositronium;
        Bank.PutCash(BankSystem.Currency.Aurite, _tempAurite + save.MoneyPerLevel);

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

        SceneStatics.SceneCore.GetComponent<BusStop>().SpawnBus();
    }

    public void MissionVictory()
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        _missionVictoryObject.SetActive(true);

        Bank.PutCash(BankSystem.Currency.Cosmilite, save.RecievedCosmilite);
        Bank.PutCash(BankSystem.Currency.Positronium, save.RecievedPositronium);

        // ID Анлока миссии и подсчёт прогресса уровней маяков ведётся непосредственно из класса Mission

        GameSessionInfoHandler.ClearGameSession();
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
            SceneTransition.SwitchToScene(CustomSceneOnDeath);
            ClearCustomData();
            return;
        }

        ClearCustomData();
        SceneTransition.SwitchToScene("Lobby");
    }

    private static void ClearCustomData()
    {
        CustomSceneOnVictory = null;
        CustomSceneOnDeath = null;
        CustomSceneOnExit = null;
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
        }
    }
    #endif
}
