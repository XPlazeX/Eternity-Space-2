using UnityEngine;
//using BankSystem;

public class GlobalSaveHandler : MonoBehaviour
{
    private const string filename = "GlobalSave";

    public delegate void saveDataOperation();
    public static event saveDataOperation SavingAll;
    
    private static GlobalSave _save;
    private static Storage _storage;

    private static bool _initialized = false;

    private void Awake() {
        if (!_initialized)
            Initialize();
    }

    public static void Initialize() {
        _storage = new Storage(filename);
        _save = (GlobalSave)_storage.Load(new GlobalSave());
        _initialized = true;
    }

    public static GlobalSave GetSave()
    {
        if (!_initialized)
            Initialize();
        return _save;
    }

    public static void RewriteSave(GlobalSave save)
    {
        _storage.Save(save);
        _save = save;//(GameSessionSave)_storage.Load(new GameSessionSave());
        //SaveAll();
    }

    public static void ClearSave()
    {
        _storage.Save(new GlobalSave());
        print("Clear global save.");
        _save = (GlobalSave)_storage.Load(new GlobalSave());
    }

    public static void SaveAll()
    {
        SavingAll?.Invoke();
    }
}
