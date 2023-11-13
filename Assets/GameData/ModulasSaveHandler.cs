using UnityEngine;

public class ModulasSaveHandler : MonoBehaviour
{
    private const string filename = "Modulas";

    public delegate void saveDataOperation();
    public static event saveDataOperation SavingAll;
    
    private static ModulasSave _save;
    private static Storage _storage;

    private static bool _initialized = false;

    private void Awake() {
        if (!_initialized)
            Initialize();
    }

    public static void Initialize() {
        _storage = new Storage(filename);
        _save = (ModulasSave)_storage.Load(new ModulasSave());
        _initialized = true;
    }

    public static void UnloadData()
    {
        _storage = null;
        _save = null;
        _initialized = false;
    }

    public static ModulasSave GetSave()
    {
        if (!_initialized)
            Initialize();
        return _save;
    }

    public static void RewriteSave(ModulasSave save)
    {
        _storage.Save(save);
        _save = save;//(GameSessionSave)_storage.Load(new GameSessionSave());
        //SaveAll();
    }

    public static void FlushChoice()
    {
        _save.FlushLevel();
        _storage.Save(_save);
    }

    public static void ClearSave()
    {
        _storage.Save(new ModulasSave());
        print("Clear modulas save.");
        _save = (ModulasSave)_storage.Load(new ModulasSave());
    }

    public static void SaveAll()
    {
        SavingAll?.Invoke();
    }
}
