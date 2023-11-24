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

    public float RepairPart {get; set;} = 0.3f;

    public string LobbyDialogue {get; set;} = null;

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
        UnityEngine.Debug.Log($"new unlock: {code}");
    }

    public void ProgressUnlock(int code, int addingValue)
    {
        if (!Unlocks.ContainsKey(code))
        {
            NewUnlock(code);
        }

        Unlocks[code] += addingValue;
        UnityEngine.Debug.Log($"progress unlock now: {code} : {Unlocks[code]}");
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
    
    public int ValueOfUnlock(int code)
    {
        if (!Unlocks.ContainsKey(code))
            return -1;

        else return Unlocks[code];

    }
}
