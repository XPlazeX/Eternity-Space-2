using UnityEngine;
using UnityEngine.UI;

public class MissionSelector : MonoBehaviour
{
    [SerializeField] private RectTransform _selectorFrame;
    [SerializeField] private Button _startButton;

    private int _minCosReward;
    private int _maxCosReward;
    private bool _started;
    private MissionsDatabase _missionDatabase;

    private void Start() 
    {
        _missionDatabase = GameObject.FindWithTag("BetweenScenes").GetComponent<MissionsDatabase>();

        UpdateData();
        _selectorFrame.gameObject.SetActive(false);
        _startButton.interactable = false;
    }

    public void SelectMission(int ID)
    {
        GlobalSave gsave = GlobalSaveHandler.GetSave();
        gsave.LastSelectedLocation = ID;
        GlobalSaveHandler.RewriteSave(gsave);

        UpdateData();
    }

    public void UpdateFramePosition(RectTransform rect)
    {
        _selectorFrame.anchoredPosition = rect.anchoredPosition;
        _selectorFrame.gameObject.SetActive(true);

        _startButton.interactable = true;
    }

    private void UpdateData()
    {
        int locID = GlobalSaveHandler.GetSave().LastSelectedLocation;

        _missionDatabase.SetSessionData(locID);
        print($"Перезапись сохранения (лобби): {locID}");
    }

    public void StartMission()
    {
        if (_started)
            return;

        GlobalSave gsave = GlobalSaveHandler.GetSave();
        gsave.TotalMissionsTries ++;
        GlobalSaveHandler.RewriteSave(gsave);

        if (_missionDatabase._lastMissionReference.BoostFirstLevel)
        {
            SceneTransition.SwitchToScene("MissionMenu");
        } else
            SceneTransition.SwitchToScene("Game");

        _started = true;
    }
}
