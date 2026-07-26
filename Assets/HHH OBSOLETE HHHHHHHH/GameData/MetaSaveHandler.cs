using UnityEngine;

public class MetaSaveHandler : MonoBehaviour
{
    private const string filename = "Metasave";

    public delegate void saveDataOperation();
    public static event saveDataOperation SavingAll;
    
    private static MetaSave _save;
    private static Storage _storage;

    private static bool _initialized = false;

    private void Awake() {
        if (!_initialized)
            Initialize();
    }

    public static void Initialize() {
        _storage = new Storage(filename);
        _save = (MetaSave)_storage.Load(new MetaSave());
        _initialized = true;
    }

    public static MetaSave GetSave()
    {
        if (!_initialized)
            Initialize();
        return _save;
    }

    public static void RewriteSave(MetaSave save)
    {
        _storage.Save(save);
        _save = save;//(GameSessionSave)_storage.Load(new GameSessionSave());
        //SaveAll();
    }

    public static void SaveAll()
    {
        SavingAll?.Invoke();
    }
}
