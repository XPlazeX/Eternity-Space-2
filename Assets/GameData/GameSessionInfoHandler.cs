using UnityEngine;
using System.Collections.Generic;

public class GameSessionInfoHandler : MonoBehaviour
{
    private const string filename = "GameSession";

    public delegate void saveDataOperation();
    public static event saveDataOperation SavingAll;
    //public static event saveDataOperation LevelDischarge;

    private static GameSessionSave _save;
    private static Storage _storage;

    public static int CurrentLevel {get; private set;} = 0;
    public static int MaxLevel {get; private set;} = 0;
    public static bool FinalLevel => CurrentLevel >= MaxLevel - 1;
    public static float LevelProgress => MaxLevel == 1 ? 1 : ((float)CurrentLevel / (MaxLevel - 1));

    private void Awake() 
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Game")
        {
            Initialize();
        }
    }

    public static void Initialize() 
    {
        _storage = new Storage(filename);
        _save = (GameSessionSave)_storage.Load(new GameSessionSave());

        CurrentLevel = _save.CurrentLevel;
        MaxLevel = _save.MaxLevel;

        Random.InitState(_save.Seed);
        print($"seed : {_save.Seed}, level: {CurrentLevel} / {MaxLevel} | Progress: {LevelProgress}");
    }

    public static int GetSeed()
    {
        return _save.Seed + _save.CurrentLevel;
    }
// DATA COLLECTIONS
    public static void AddDataCollection(string name, List<int> defaultValues)
    {
        _save.AdditiveData[name] = defaultValues;
        RewriteSessionSave(_save);
    }

    public static void AddValueToCollection(string name, int value)
    {
        _save.AdditiveData[name].Add(value);
        RewriteSessionSave(_save);
    }

    public static List<int> GetDataCollection(string name)
    {
        if (!_save.AdditiveData.ContainsKey(name))
            AddDataCollection(name, new List<int>());
        
        return _save.AdditiveData[name];
    }

    public static bool ExistDataCollection(string name)
    {
        return _save.AdditiveData.ContainsKey(name);
    }

    public static void RemoveDataCollection(string name)
    {
        if (!_save.AdditiveData.ContainsKey(name))
            return;

        _save.AdditiveData.Remove(name);
        RewriteSessionSave(_save);
    }

    public static GameSessionSave GetSessionSave()
    {
        return _save;
    }

    public static void RewriteSessionSave(GameSessionSave save)
    {
        _storage.Save(save);
        _save = save;
    }

    public static void ClearGameSession()
    {
        _save = new GameSessionSave();

        _storage.Save(_save);
        print("Clear session.");

        ModulasSaveHandler.ClearSave();
    }

    public static void SaveAll()
    {
        print("call Save All");
        SavingAll?.Invoke();
    }
// EDITOR
    #if UNITY_EDITOR
    private void Update() {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _save.ChangeSeed();
            RewriteSessionSave(_save);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            _save.ChangeSeed();
            ClearGameSession();
            SceneTransition.ReloadScene();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            _save.ChangeSeed();
            //DischargeLevel();
            SceneTransition.ReloadScene();
        }
    }
    #endif
}