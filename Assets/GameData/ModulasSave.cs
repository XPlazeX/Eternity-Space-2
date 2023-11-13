using System.Collections.Generic;

[System.Serializable]
public class ModulasSave
{
    public List<LevelEvent> PassiveEvents { get; set; } = new List<LevelEvent>();
    public List<LevelEvent> LevelEvents { get; set; } = new List<LevelEvent>();

    public ActiveEventData[] ActiveEventDatas {get; set;}  

    public bool EventsLoaded { get; private set; } = false;
    public bool HasPack { get; private set; } = false;
    public bool BlockHeal { get; set; } = false;
    public int PackID { get; private set; } = -1;

    public List<LevelEvent> GetAllLevelEvents()
    {
        List<LevelEvent> res = new List<LevelEvent>(PassiveEvents);

        res.AddRange(LevelEvents);

        return res;
    }

    public List<int> GetUniqueModulasID()
    {
        List<int> uniques = new List<int>();

        for (int i = 0; i < PassiveEvents.Count; i++)
        {
            if (!PassiveEvents[i].stackable || PassiveEvents[i].isPack)
                uniques.Add(PassiveEvents[i].moduleOperandID);
        }

        return uniques;
    }

    public void AddEvent(LevelEvent levelEvent)
    {
        if (levelEvent.isPack)
        {
            AddPackEvent(levelEvent);
            return;
        }
        if (levelEvent.destroyOnNextLevel)
        {
            LevelEvents.Add(levelEvent);
        } 
        else
        {
            PassiveEvents.Add(levelEvent);
        }
    }

    public void AddPackEvent(LevelEvent levelEvent)
    {
        for (int i = 0; i < PassiveEvents.Count; i++)
        {
            if (PassiveEvents[i].isPack)
            {
                PassiveEvents.RemoveAt(i);
                break;
            }
        }

        PassiveEvents.Add(levelEvent);
        HasPack = true;
        PackID = (SceneStatics.CharacterCore.GetComponent<CharacterModules>().GetModuleOperand(levelEvent.moduleOperandID) as PackModuleOperand).PackID;
    }
    public void InitEventChoice(int[] eventIDs)
    {
        ActiveEventDatas = new ActiveEventData[eventIDs.Length];

        for (int i = 0; i < eventIDs.Length; i++)
        {
            ActiveEventDatas[i].eventID = eventIDs[i];
        }

        EventsLoaded = true;
    }

    public void FlushLevel()
    {
        LevelEvents = new List<LevelEvent>();
        ActiveEventDatas = new ActiveEventData[0];

        EventsLoaded = false;
    }
}

[System.Serializable]
public struct ActiveEventData
{
    public int eventID;
    public bool purchased;
}
