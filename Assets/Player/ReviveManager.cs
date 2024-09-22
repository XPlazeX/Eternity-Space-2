public static class ReviveManager
{
    private const string filename = "Revives";

    public delegate void reviveOperation(string code);
    public static event reviveOperation TryingRevive;
    
    private static ReviveSave _save;
    private static Storage _storage;

    private static bool _initialized = false;

    public static void Initialize() {
        _storage = new Storage(filename);
        _save = (ReviveSave)_storage.Load(new ReviveSave());
        _initialized = true;
    }

    public static void UnloadData()
    {
        _storage = null;
        _save = null;
        _initialized = false;
    }

    public static ReviveSave GetSave()
    {
        if (!_initialized)
            Initialize();
        return _save;
    }

    public static void RewriteSave(ReviveSave save)
    {
        _storage.Save(save);
        _save = save;//(GameSessionSave)_storage.Load(new GameSessionSave());
        //SaveAll();
    }

    public static void RegisterRevive(string code)
    {
        if (!_initialized)
        {
            Initialize();
        }

        _save.RegisterRevive(code);
        RewriteSave(_save);
    }

    public static bool CanRevive()
    {
        if (!_initialized)
            Initialize();

        return _save.CanRevive;
    }

    public static void TryRevive()
    {
        if (!_initialized)
            Initialize();

        if (!CanRevive())
            return;

        VictoryHandler.EnableRestartLevelOnDeath();

        string code = _save.ExtractNearestRevive();

        TryingRevive?.Invoke(code);

        RewriteSave(_save);
    }

    public static void ClearSave()
    {
        if (!_initialized)
            Initialize();

        _storage.Save(new ReviveSave());
        //print("Clear modulas save.");
        _save = (ReviveSave)_storage.Load(new ReviveSave());
    }
}
