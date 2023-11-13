using UnityEngine;

public class Mission : MonoBehaviour 
{
    [Header("Данные")]
    [SerializeField] private int _unlockID = -1;
    [SerializeField] private bool _boostFirstLevel;
    [SerializeField][Range(1, 100)] private int _levelCount = 1;
    [SerializeField][Range(50, 10000)] private int _startAurite = 50;
    [SerializeField][Range(80, 1000)] private int _auritePerLevel = 80;
    [SerializeField] private Vector2Int _minMaxCosmilite;
    [SerializeField] private int _nameLocalizationID = 0;
    [SerializeField] private int _radioID = 0;
    [Header("Кастомизация")]
    [SerializeField] private int _customShip = -1;
    [SerializeField] private string _customWeaponModel = null;
    [Header("Диалоги")]
    [SerializeField] private LevelDialoguePair[] _levelDialogueTriggers;
    [SerializeField] private string _completedMissionLobbyDialogue = null;
    [Header("------------ ЯДРО ------------")]
    [SerializeField] private BackgroundLoader _background;
    [SerializeField] private SoundObject _soundtrack;
    //место для геймплейного носителя

    public int UnlockID => _unlockID;
    public bool BeaconLevel => _background.Beacon;
    public bool BoostFirstLevel => _boostFirstLevel;
    public int CustomShip => _customShip;
    public string CustomWeaponModel => _customWeaponModel;

    public virtual void StartPlay() 
    {
        _background.Load();
        SceneStatics.AudioCore.GetComponent<SoundPlayer>().SetSoundtrack(_soundtrack);

        VictoryHandler.MissionVictored += OnMissionVictoried;

        print("МИССИЯ ЗАПУЩЕНА");
    }

    protected virtual void TriggerVictory()
    {
        SceneStatics.CharacterCore.GetComponent<VictoryHandler>().LevelVictory();
    }


    private void OnDisable() {
        VictoryHandler.MissionVictored -= OnMissionVictoried;
    }

    public void SetDataForSessionSave(int locID, bool rewriteAll = true)
    {
        TextLoader txtLoader = new TextLoader("Levels", "MissionsData");

        if (rewriteAll)
        {
            GameSessionInfoHandler.ClearGameSession();
        }

        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        save.LocationID = locID;

        save.LevelUnlockCode = _unlockID;

        //save.Bossfight = _bossfight;
        //save.BeaconLevel = _isBeacon;
        //save.CurrentLevel = 0;
        save.MaxLevel = _levelCount;
        save.Boosted = BoostFirstLevel;

        if (rewriteAll)
            save.Money = _startAurite;
        save.MoneyPerLevel = _auritePerLevel;

        //save.RecievedCosmilite = UnityEngine.Random.Range(_minMaxCosmilite.x, _minMaxCosmilite.y + 1);

        save.LocalizedLocationName = new TextLoader("Locations", _nameLocalizationID, 0, true).FirstCell;

        save.DialogueEntry = _radioID;
        //save.VictoriedDialogue = _completedMissionLobbyDialogue;

        // if (_customShip != -1)
        // {
        //     save.ShipModel = _customShip;
        //     UnityEngine.Debug.Log($"-custom ship: {_customShip}");
        // }
        // if (!string.IsNullOrEmpty(_customWeaponModel))
        // {
        //     save.WeaponModel = _customWeaponModel;
        //     UnityEngine.Debug.Log($"-custom mission weapon: {_customWeaponModel}");
        // }
        
        GameSessionInfoHandler.RewriteSessionSave(save);

        GlobalSave gsave = GlobalSaveHandler.GetSave();
        gsave.LastSelectedLocation = locID;
        GlobalSaveHandler.RewriteSave(gsave);

        UnityEngine.Debug.Log($"Перезапись экземпляра сохранения:UID({save.LevelUnlockCode})||| {save.LocalizedLocationName}| уровней: {save.MaxLevel}|| аурит за уровень: {save.MoneyPerLevel}| космилит: {save.RecievedCosmilite}");
    }

    private void OnMissionVictoried()
    {
        Unlocks.NewUnlock(_unlockID);
        GlobalSaveHandler.GetSave().LobbyDialogue = _completedMissionLobbyDialogue;
        if (BeaconLevel)
        {
            Unlocks.ProgressUnlock(7, 1);
        }
        Unlocks.ProgressUnlock(8, 1);

        int cosReward = Random.Range(_minMaxCosmilite.x, _minMaxCosmilite.y + 1);
        Bank.PutCash(BankSystem.Currency.Cosmilite, cosReward);
        print($"Начислен космилит за уровень: {cosReward}");
    }

    public string GetDialogueName(int level, bool isMenu)
    {
        for (int i = 0; i < _levelDialogueTriggers.Length; i++)
        {
            if (_levelDialogueTriggers[i].targetLevel != level)
                continue;

            if (isMenu)
                return _levelDialogueTriggers[i].menuDialogue;
            else 
                return _levelDialogueTriggers[i].gameDialogue;
        }

        return null;
    }

    [System.Serializable]
    public struct LevelDialoguePair
    {
        public int targetLevel;
        public string menuDialogue;
        public string gameDialogue;
    }
}

// [CreateAssetMenu(fileName = "Mission", menuName = "Teleplane2/Mission", order = -2)]
// public class Mission : ScriptableObject
// {
//     [Header("Данные")]
//     [SerializeField] private int _unlockID;
//     [SerializeField] private bool _bossfight;
//     [SerializeField] private bool _isBeacon = false;
//     [SerializeField][Range(1, 100)] private int _levelCount;
//     [SerializeField][Range(50, 1000)] private int _auritePerLevel;
//     [SerializeField] private Vector2Int _minMaxCosmilite;
//     [SerializeField] private int _nameLocalizationID;
//     [SerializeField] private int _radioID;
//     [Header("Ядро")]
//     [SerializeField] private MissionLevelCore _missionLevelCore;
//     [Header("Диалоги")]
//     [SerializeField] private LevelDialoguePair[] _levelDialogueTriggers;
//     [SerializeField] private string _completedMissionLobbyDialogue = null;
//     [Space()]
//     [Header("Кастомизация")]
//     [SerializeField] private int _customShip = -1;
//     [SerializeField] private string _customWeaponModel = null;

//     public void RewriteSessionSave(int locID)
//     {
//         TextLoader txtLoader = new TextLoader("Levels", "MissionsData");
//         GameSessionInfoHandler.ClearGameSession();
//         GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

//         save.LocationID = locID;

//         save.LevelUnlockCode = _unlockID;

//         save.Bossfight = _bossfight;
//         save.BeaconLevel = _isBeacon;
//         //save.CurrentLevel = 0;
//         save.MaxLevel = _levelCount;

//         save.MoneyPerLevel = _auritePerLevel;

//         save.RecievedCosmilite = UnityEngine.Random.Range(_minMaxCosmilite.x, _minMaxCosmilite.y + 1);

//         save.LocalizedLocationName = new TextLoader("Locations", _nameLocalizationID, 0, true).FirstCell;

//         save.DialogueEntry = _radioID;
//         save.VictoriedDialogue = _completedMissionLobbyDialogue;

//         if (_customShip != -1)
//         {
//             save.ShipModel = _customShip;
//             UnityEngine.Debug.Log($"-custom ship: {_customShip}");
//         }
//         if (!string.IsNullOrEmpty(_customWeaponModel))
//         {
//             save.WeaponModel = _customWeaponModel;
//             UnityEngine.Debug.Log($"-custom mission weapon: {_customWeaponModel}");
//         }
        
//         GameSessionInfoHandler.RewriteSessionSave(save);

//         GlobalSave gsave = GlobalSaveHandler.GetSave();
//         gsave.LastSelectedLocation = locID;
//         GlobalSaveHandler.RewriteSave(gsave);

//         UnityEngine.Debug.Log($"Перезапись экземпляра сохранения:UID({save.LevelUnlockCode})||| {save.LocalizedLocationName}| уровней: {save.MaxLevel}| маяк?: {save.BeaconLevel}| аурит за уровень: {save.MoneyPerLevel}| космилит: {save.RecievedCosmilite}");
//     }

//     public void EnforceLevelCore()
//     {
//         _missionLevelCore.Enforce(_isBeacon);
//     }

//     public string GetMenuDialogueName(int level)
//     {
//         string result = null;

//         for (int i = 0; i < _levelDialogueTriggers.Length; i++)
//         {
//             if (_levelDialogueTriggers[i].targetLevel != level)
//                 continue;

//             result = _levelDialogueTriggers[i].menuDialogue;
//             break;
//         }

//         return result;
//     }

//     public string GetGameDialogueName(int level)
//     {
//         string result = null;

//         for (int i = 0; i < _levelDialogueTriggers.Length; i++)
//         {
//             if (_levelDialogueTriggers[i].targetLevel != level)
//                 continue;

//             result = _levelDialogueTriggers[i].gameDialogue;
//             break;
//         }

//         return result;
//     }

//     [System.Serializable]
//     public struct LevelDialoguePair
//     {
//         public int targetLevel;
//         public string menuDialogue;
//         public string gameDialogue;
//     }
// }
