using UnityEngine;

public class Intro : MonoBehaviour
{
    private const int cryoDream_Training_Unlock_ID = 5;
    private const int simulation_Training_Unlock_ID = 6;

    private const int dream_EventHorizon_mission_ID = 18;
    private const int dream_EventHorizon_unlock_ID = 35;

    [SerializeField] private UnlockRequire[] _requiresDreamEH;

    private void Start() {
        GameObject.FindWithTag("BetweenScenes").GetComponent<MissionsDatabase>().SetSessionData(GlobalSaveHandler.GetSave().LastSelectedLocation, false);
    }

    public void Entry()
    {
        if (Unlocks.HasUnlocks(_requiresDreamEH) && !Unlocks.HasUnlock(dream_EventHorizon_unlock_ID) && Random.value < 1f)
        {
            print("TRY DREAM EVENTHORIZON");
            if (GameSessionInfoHandler.LevelProgressFloored == 0)
            {
                GameObject.FindWithTag("BetweenScenes").GetComponent<MissionsDatabase>().SetSessionData(dream_EventHorizon_mission_ID, true);
                SceneTransition.SwitchToScene("Game");
                return;
            } 
        }

        if (!Unlocks.HasUnlock(cryoDream_Training_Unlock_ID))
        {
            SceneTransition.SwitchToScene("Game");
            return;
        }
        else if (!Unlocks.HasUnlock(simulation_Training_Unlock_ID))
        {
            SceneTransition.SwitchToScene("MissionMenu");
            return;
        }


        SceneTransition.OpenRelevantLobbyScene();
    }
}
