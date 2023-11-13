using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MissionsDatabase : MonoBehaviour
{
    [SerializeField] private AssetReference[] _missions;

    private AsyncOperationHandle _missionOperationHandle;

    public Mission _activeMissionSample {get; private set;} = null;
    public Mission _lastMissionReference {get; private set;} = null;
    // Start is called before the first frame update

    public void SetSessionData(int missionID, bool rewriteAll = true)
    {
        StartCoroutine(SettingGameSessionData(missionID, rewriteAll));
        print($"MISSION DATABASE: set mission data: {missionID}");
    }

    public void EnforceLevelCore(int missionID)
    {
        StartCoroutine(EnforcingLevelCore(missionID));
        print($"MISSION DATABASE: enforcing level core: {missionID}");
    }

    public void LoadMenuDialogue(int missionID)
    {
        StartCoroutine(LoadingMenuDialogue(missionID));
        print($"MISSION DATABASE: loading menu dialogue: {missionID}");
    }
    
    private IEnumerator SettingGameSessionData(int missionID, bool rewriteAll)
    {
        if (_missionOperationHandle.IsValid())
        {
            Addressables.Release(_missionOperationHandle);
        }

        var missionReference = _missions[missionID];

        _missionOperationHandle = Addressables.LoadAssetAsync<GameObject>(missionReference);
        yield return _missionOperationHandle;

        Mission mission = ((GameObject)(_missionOperationHandle.Result)).GetComponent<Mission>();

        mission.SetDataForSessionSave(missionID, rewriteAll);

        _lastMissionReference = mission;
        //((Mission)_missionOperationHandle.Result).SetDataForSessionSave(missionID);
    }

    public IEnumerator EnforcingLevelCore(int missionID)
    {
        if (_missionOperationHandle.IsValid())
        {
            Addressables.Release(_missionOperationHandle);
        }

        var missionReference = _missions[missionID];

        _missionOperationHandle = Addressables.LoadAssetAsync<GameObject>(missionReference);
        yield return _missionOperationHandle;

        Mission missionSample = Instantiate((GameObject)_missionOperationHandle.Result).GetComponent<Mission>();
        missionSample.StartPlay();

        _activeMissionSample = missionSample;
        _lastMissionReference = missionSample;

        if ((PlayerPrefs.GetFloat("DialogMod", 0) == 1) && Unlocks.HasUnlock(missionSample.UnlockID))
        {
            yield break;
        }

        string gameDialogue = missionSample.GetDialogueName(GameSessionInfoHandler.GetSessionSave().CurrentLevel, false);
        if (!string.IsNullOrEmpty(gameDialogue))
        {
            GetComponent<DialogueOpener>().TriggerDialogue(gameDialogue);
        }
    }

    private IEnumerator LoadingMenuDialogue(int missionID)
    {
        if (_missionOperationHandle.IsValid())
        {
            Addressables.Release(_missionOperationHandle);
        }

        var missionReference = _missions[missionID];

        _missionOperationHandle = Addressables.LoadAssetAsync<GameObject>(missionReference);
        yield return _missionOperationHandle;

        Mission mission = ((GameObject)(_missionOperationHandle.Result)).GetComponent<Mission>();

        if ((PlayerPrefs.GetFloat("DialogMod", 0) == 1) && Unlocks.HasUnlock(mission.UnlockID))
        {
            yield break;
        }

        string menuDialogue = mission.GetDialogueName(GameSessionInfoHandler.GetSessionSave().CurrentLevel, true);
        if (!string.IsNullOrEmpty(menuDialogue))
        {
            GetComponent<DialogueOpener>().TriggerDialogue(menuDialogue);
        }
    }
}
