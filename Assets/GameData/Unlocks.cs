using UnityEngine;

public static class Unlocks
{
    public static bool HasUnlock(int code)
    {
        if (code == -1)
            return true;
        return GlobalSaveHandler.GetSave().HasUnlock(code);
    }

    public static bool HasUnlock(int code, int progress)
    {
        if (code == -1)
            return true;
        return GlobalSaveHandler.GetSave().HasUnlock(code, progress);
    }

    public static bool HasUnlock(UnlockRequire unlockRequire)
    {
        return unlockRequire.Completed;
    }

    public static bool HasUnlocks(UnlockRequire[] unlockRequires)
    {
        for (int i = 0; i < unlockRequires.Length; i++)
        {
            if (!unlockRequires[i].Completed) 
            {
                return false;
            }
        }

        return true;
    }

    public static void NewUnlock(int code)
    {
        if (code == -1)
            return;
        GlobalSave gsave = GlobalSaveHandler.GetSave();
        gsave.NewUnlock(code);
        GlobalSaveHandler.RewriteSave(gsave);
    }

    public static void ProgressUnlock(int code, int progress)
    {
        if (code == -1)
            return;
        GlobalSave gsave = GlobalSaveHandler.GetSave();
        gsave.ProgressUnlock(code, progress);
        GlobalSaveHandler.RewriteSave(gsave);
    }
}

[System.Serializable]
public class UnlockRequire
{
    [SerializeField] private int _UID = -1;
    [SerializeField] private int _amount = 0;

    public bool Completed => Unlocks.HasUnlock(_UID, _amount);
}
