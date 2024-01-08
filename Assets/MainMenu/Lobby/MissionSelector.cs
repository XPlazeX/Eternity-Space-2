using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MissionSelector : MonoBehaviour
{
    [SerializeField] private RectTransform _selectorFrame;
    [SerializeField] private Button _startButton;

    private int _minCosReward;
    private int _maxCosReward;
    private bool _started;
    private MissionsDatabase _missionDatabase;
    private bool _loadAvaiable = true;

    private void Start() 
    {
        _missionDatabase = GameObject.FindWithTag("BetweenScenes").GetComponent<MissionsDatabase>();

        _selectorFrame.gameObject.SetActive(false);
        _startButton.interactable = false;
    }

    public void SelectMission(int ID)
    {
        if (!_loadAvaiable)
            return;

        GlobalSave gsave = GlobalSaveHandler.GetSave();
        gsave.LastSelectedLocation = ID;
        GlobalSaveHandler.RewriteSave(gsave);

        UpdateData(ID);
    }

    public void UpdateFramePosition(RectTransform rect)
    {
        _selectorFrame.anchoredPosition = rect.anchoredPosition;
        _selectorFrame.gameObject.SetActive(true);
    }

    private void UpdateData(int ID = -1)
    {
        int locID = ID;

        if (locID < 0)
            locID = GlobalSaveHandler.GetSave().LastSelectedLocation;

        StartCoroutine(PreparingMission(locID));
    }

    public void StartMission()
    {
        if (_started)
            return;

        Unlocks.ProgressUnlock(3, 1); // кол-во запусков миссий

        if (_missionDatabase._lastMissionReference.BoostFirstLevel)
        {
            SceneTransition.SwitchToScene("MissionMenu");
        } else
            SceneTransition.SwitchToScene("Game");

        _started = true;
    }

    private IEnumerator PreparingMission(int ID)
    {
        _startButton.interactable = false;
        _loadAvaiable = false;

        yield return _missionDatabase.StartCoroutine(_missionDatabase.SettingGameSessionData(ID, true));

        print($"Подготовлена миссия (лобби): {ID}");
        _startButton.interactable = true;
        _loadAvaiable = true;
    }
}
