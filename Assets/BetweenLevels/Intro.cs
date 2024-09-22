using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Intro : MonoBehaviour
{
    private const int cryoDream_Training_Unlock_ID = 5;
    private const int simulation_Training_Unlock_ID = 6;

    private const int dream_EventHorizon_mission_ID = 18;
    private const int dream_EventHorizon_unlock_ID = 35;

    private const int dream_Dilya_require_ID = 45;
    private const int dream_Dilya_mission_ID = 25;
    private const int dream_Dilya_unlock_ID = 46;

    [SerializeField] private Button _startButton;
    [SerializeField] private UnlockRequire[] _requiresDreamEH;

    private string _targetScene = "Lobby";
    private bool _entired = false;

    private void Start() 
    {
        TimeHandler.Resume();
        StartCoroutine(PreparingMission());
    }

    public void Entry()
    {
        if (_entired)
            return;
        _entired = true;
        SceneTransition.SwitchToScene(_targetScene);
    }

    private IEnumerator PreparingMission()
    {
        _startButton.interactable = false;

        MissionsDatabase mdb = GameObject.FindWithTag("BetweenScenes").GetComponent<MissionsDatabase>();

        int preparingID = GlobalSaveHandler.GetSave().LastSelectedLocation;

        _targetScene = GameSessionInfoHandler.GetSessionSave().SessionInitialized ? "MissionMenu" : "Lobby";

        if (preparingID == dream_Dilya_mission_ID || preparingID == dream_EventHorizon_mission_ID)
        {
            GameSessionInfoHandler.ClearGameSession();
        }

        if (Unlocks.HasUnlocks(_requiresDreamEH) && !Unlocks.HasUnlock(dream_EventHorizon_unlock_ID) && !GameSessionInfoHandler.GetSessionSave().SessionInitialized)
        {
            print("TRY DREAM EVENTHORIZON");
            preparingID = dream_EventHorizon_mission_ID;
            _targetScene = "Game";
        }

        if (Unlocks.HasUnlock(dream_Dilya_require_ID) && !Unlocks.HasUnlock(dream_Dilya_unlock_ID) && !GameSessionInfoHandler.GetSessionSave().SessionInitialized)
        {
            print("TRY DREAM DILYA");
            preparingID = dream_Dilya_mission_ID;
            _targetScene = "Game";
        }

        if (!Unlocks.HasUnlock(cryoDream_Training_Unlock_ID))
        {
            _targetScene = "Game";
        }
        else if (!Unlocks.HasUnlock(simulation_Training_Unlock_ID))
        {
            _targetScene = "MissionMenu";
        }

        yield return mdb.StartCoroutine(mdb.SettingGameSessionData(preparingID, false));

        _startButton.interactable = true;
    }
}
