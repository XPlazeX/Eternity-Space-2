using System.Collections.Generic;
using UnityEngine;

public class GetModulesOnAsquiring : Module
{
    [SerializeField][Range(1, 20)] private int _moduleCount;

    public override void Asquiring()
    {
        MissionEventsDistributor med = SceneStatics.SceneCore.GetComponent<MissionEventsDistributor>();

        List<int> selectedEvents = med.SelectEventsFromPool(med.GetAvaiableEvents(false, true), _moduleCount);

        ModulasSave moduleSave = ModulasSaveHandler.GetSave();

        for (int i = 0; i < selectedEvents.Count; i++)
        {
            LevelEvent le = med.GetLevelEvent(selectedEvents[i]);
            le.handingModule.Asquiring();
            moduleSave.AddEvent(le);
            Debug.Log($"$$$ RANDOM MODULE GETTED $$$ LevelEventID={selectedEvents[i]}");
        }
        
        ModulasSaveHandler.RewriteSave(moduleSave);
    }
}
