using UnityEngine;

public class Intro : MonoBehaviour
{
    private const int cryoDream_Training_Unlock_ID = 5;
    private const int simulation_Training_Unlock_ID = 6;

    private void Start() {
        GameObject.FindWithTag("BetweenScenes").GetComponent<MissionsDatabase>().SetSessionData(GlobalSaveHandler.GetSave().LastSelectedLocation, false);
    }

    public void Entry()
    {
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
