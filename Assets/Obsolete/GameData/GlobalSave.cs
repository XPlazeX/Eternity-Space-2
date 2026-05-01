using System.Collections.Generic;

[System.Serializable]
public class GlobalSave
{
    public int Cosmilite {get; set;} = 100;
    public int Positronium {get; set;} = 0;
    public int WeaponStartLevel {get; set;} = 0;

    public int LastSelectedLocation {get; set;} = 0;
    
    public int LastSelectedShip {get; set;} = 0;
    public int LastSelectedWeapon {get; set;} = 0;
    public int LastSelectedDevice {get; set;} = 0;
    public int LastSelectedCore {get; set;} = 0;
    public int LastSelectedAbility {get; set;} = 0;

    public float RepairPart {get; set;} = 0.3f;

    public string LobbyDialogue {get; set;} = null;

    public float EternityMinute {get; set;} = -1f;
    public float EternityHour {get; set;} = -1f;
    public bool EternityParse {get; set;} = false;
    public float VectorErrorRate {get; set;} = 1f;

    public Dictionary<int, int> Unlocks {get; private set;} = new Dictionary<int, int>();

    public GlobalSave()
    {
        NewUnlock(601);
        NewUnlock(621);
    }

    public void NewUnlock(int code)
    {
        if (Unlocks.ContainsKey(code))
            return;

        Unlocks[code] = 0;
        UnityEngine.Debug.Log($"<color=lime>new unlock: {code}</color>");
    }

    public void ProgressUnlock(int code, int addingValue)
    {
        if (!Unlocks.ContainsKey(code))
        {
            NewUnlock(code);
        }

        Unlocks[code] += addingValue;
        UnityEngine.Debug.Log($"<color=yellow>progress unlock now: {code} : {Unlocks[code]}</color>");
    }

    public void RewriteUnlockProgress(int code, int newValue)
    {
        if (!Unlocks.ContainsKey(code))
        {
            NewUnlock(code);
        }

        Unlocks[code] = newValue;
        UnityEngine.Debug.Log($"<color=yellow>progress unlock now: {code} : {Unlocks[code]}</color>");
    }

    public bool HasUnlock(int code)
    {
        return Unlocks.ContainsKey(code);
    }

    public bool HasUnlock(int code, int progress)
    {
        if (!Unlocks.ContainsKey(code))
            return false;
        
        return Unlocks[code] >= progress;
    }
    
    public int ValueOfUnlock(int code, bool zeroIfNot = false)
    {
        if (!Unlocks.ContainsKey(code))
            return zeroIfNot ? 0 : -1;

        else return Unlocks[code];

    }
    #if UNITY_EDITOR
    public void RemoveUnlock(int code)
    {
        UnityEngine.Debug.Log($"<color=red>unlock: {code} had progress {Unlocks[code]}, but was removed</color>");
        Unlocks.Remove(code);
    }
    #endif
}
