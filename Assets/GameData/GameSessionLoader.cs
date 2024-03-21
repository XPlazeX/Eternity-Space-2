using UnityEngine;
using System.Collections;

public class GameSessionLoader : MonoBehaviour
{
    //[SerializeField] private bool _dontLoadPlayer = false;
    [SerializeField] private bool _testMode = false;
    [SerializeField] private int _testLocationID = 0;

    private void Awake() {
        ShipStats temp = new ShipStats();
        temp.Initialize();

        GameSessionInfoHandler.Initialize();

        if (SceneStatics.CoresFinded)
            Initialize();
        else
            SceneStatics.CoresLoaded += Initialize;
    }

    private void OnDisable() {
        SceneStatics.CoresLoaded -= Initialize;
    }

    private IEnumerator SessionLoading()
    {
        print($"HHHHHHHHHHHHHHHHHHHHHH   Начало загрузки уровня прогрессом: {GameSessionInfoHandler.LevelProgress} | №{GameSessionInfoHandler.GetSessionSave().CurrentLevel}   HHHHHHHHHHHHHHHHHHHHHH");
        SceneStatics.SceneCore.GetComponent<Spawner>().Initialize();
        GameObject.FindWithTag("BetweenScenes").GetComponent<DialogueOpener>().FindCanvas();

        SceneStatics.CharacterCore.GetComponent<VictoryHandler>().Initialize();

        int locID = _testMode ? _testLocationID : GameSessionInfoHandler.GetSessionSave().LocationID;
        
        MissionsDatabase mdb = GameObject.FindWithTag("BetweenScenes").GetComponent<MissionsDatabase>();
        yield return mdb.StartCoroutine(mdb.EnforcingLevelCore(locID));

        CharacterLoader cl = SceneStatics.CharacterCore.GetComponent<CharacterLoader>();

        yield return cl.StartCoroutine(cl.LoadingPlayerShip()); // >> Player >> PlayerShipData 
        print("Character loader >> loaded player ship!");

        Player.UpdatePlayer();
        SceneStatics.SceneCore.GetComponent<ShieldDistributor>().Initialize();

        SceneStatics.SceneCore.GetComponent<GenericBulletDatabase>().Initialize();
        SceneStatics.SceneCore.GetComponent<ReflectorHandler>().Initialize();
        SceneStatics.CharacterCore.GetComponent<ModuleCore>().StartInitialization(); // >> CharacterModules >> CharacterBulletDatabase >> ParryingHandler >> AllModules

        SceneStatics.CharacterCore.GetComponent<PlayerRamsHandler>().Initialize();
        SceneStatics.CharacterCore.GetComponent<DeviceHandler>().Initialize();
        GameObject.FindWithTag("Level core").GetComponent<EnvironmentSpawner>().Initialize();
        TimeHandler.Initialize();

        PlayerController.Initialize();

        SceneStatics.CharacterCore.GetComponent<GameContagionExecutor>().Initialize();

        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
        
        if (!save.EnterNewLevel)
        {
            SceneTransition.SceneLoaded();
            print("END SESSION INITIALIZATION NO NEW LEVEL");
            yield break;
        }

        save.NewLevelLoaded();

        if (!save.SessionInitialized)
        {
            save.AllSysInitialized();
        }

        GameSessionInfoHandler.RewriteSessionSave(save);
        GameSessionInfoHandler.SaveAll();

        SceneTransition.SceneLoaded();
        print("END SESSION INITIALIZATION");
    }

    private void Initialize()
    {
        print("START SESSION INITIALIZATION");
        StartCoroutine(SessionLoading());
    }
}
