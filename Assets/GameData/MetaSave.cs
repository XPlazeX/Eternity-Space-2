using System.Collections.Generic;

[System.Serializable]
public class MetaSave
{
    public int ActiveEnding {get; set;} = 0;

    public List<int> VisitedEndings {get; set;} = new List<int>();

    public Dictionary<int, int> Achievemnts {get; private set;} = new Dictionary<int, int>();

    public void NewAchievement(int code)
    {
        if (Achievemnts.ContainsKey(code))
            return;

        Achievemnts[code] = 0;
        UnityEngine.Debug.Log($"<color=magenta>new Achievement: {code}</color>");
    }

    public void ProgressAchievement(int code, int addingValue)
    {
        if (!Achievemnts.ContainsKey(code))
        {
            NewAchievement(code);
        }

        Achievemnts[code] += addingValue;
        UnityEngine.Debug.Log($"<color=magenta>progress Achievement now: {code} : {Achievemnts[code]}</color>");
    }

    public void RewriteAchievementProgress(int code, int newValue)
    {
        if (!Achievemnts.ContainsKey(code))
        {
            NewAchievement(code);
        }

        Achievemnts[code] = newValue;
        UnityEngine.Debug.Log($"<color=magenta>progress Achievement now: {code} : {Achievemnts[code]}</color>");
    }

    public bool HasAchievement(int code)
    {
        return Achievemnts.ContainsKey(code);
    }

    public bool HasAchievement(int code, int progress)
    {
        if (!Achievemnts.ContainsKey(code))
            return false;
        
        return Achievemnts[code] >= progress;
    }
    
    public int ValueOfAchievement(int code, bool zeroIfNot = false)
    {
        if (!Achievemnts.ContainsKey(code))
            return zeroIfNot ? 0 : -1;

        else return Achievemnts[code];

    }
    #if UNITY_EDITOR
    public void RemoveAchievement(int code)
    {
        UnityEngine.Debug.Log($"<color=red>Achievement: {code} had progress {Achievemnts[code]}, but was removed</color>");
        Achievemnts.Remove(code);
    }
    #endif
}
