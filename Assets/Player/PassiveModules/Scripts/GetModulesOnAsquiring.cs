using System.Collections.Generic;
using UnityEngine;

public class GetModulesOnAsquiring : Module
{
    [SerializeField][Range(1, 20)] private int _moduleCount;

    public override void Asquiring()
    {
        MissionEventsDistributor med = SceneStatics.SceneCore.GetComponent<MissionEventsDistributor>();

        List<int> selectedEvents = med.SelectEventsFromPool(med.GetAvaiableEvents(false), _moduleCount);

        ModulasSave moduleSave = ModulasSaveHandler.GetSave();

        for (int i = 0; i < selectedEvents.Count; i++)
        {
            moduleSave.AddEvent(med.GetLevelEvent(selectedEvents[i]));
            Debug.Log($"$$$ RANDOM MODULE GETTED $$$ LevelEventID={selectedEvents[i]}");
        }
        
        ModulasSaveHandler.RewriteSave(moduleSave);
    }
}
