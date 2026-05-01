using UnityEngine;
using System.Collections.Generic;

public class Mission : MonoBehaviour 
{
    public const string boost_modules_data_collection = "BoostingModulas";

    [Header("Данные")]
    [SerializeField] private int _unlockID = -1;
    [SerializeField] private bool _boostFirstLevel;
    [SerializeField][Range(-5, 10)] private int _firstLevelBonusModulas = 0;
    [SerializeField][Range(1, 100)] private int _levelCount = 1;
    [SerializeField][Range(-5, 10)] private int _bonusModuleChoice = 0;
    [SerializeField][Range(50, 10000)] private int _startAurite = 50;
    [SerializeField][Range(80, 1000)] private int _auritePerLevel = 80;
    [SerializeField] private Vector2Int _minMaxCosmilite;
    [SerializeField] private int _nameLocalizationID = 0;
    [SerializeField] private int _radioID = 0;
    [SerializeField] private bool _isSignal = false;
    [SerializeField] private bool _autoExitToLobby;
    [SerializeField] private bool _introByExit;
    [SerializeField] private bool _clearDataOnExit;
    [Header("Кастомизация")]
    [SerializeField] private int _customShip = -1;
    [SerializeField] private string _customWeaponModel = null;
    [Header("Диалоги")]
    [SerializeField] private LevelDialoguePair[] _levelDialogueTriggers;
    [SerializeField] private string _completedMissionLobbyDialogue = null;
    [SerializeField] private string _defeatMissionLobbyDialogue = null;
    [SerializeField] private bool _uniqueDefeatDialog = false;
    [Header("------------ ЯДРО ------------")]
    [SerializeField] private BackgroundLoader _background;
    [SerializeField] private SoundObject _soundtrack;
    [SerializeField] private Vector2 _force = Vector2.zero;
    //место для геймплейного носителя

    public int UnlockID => _unlockID;
    public bool BeaconLevel => _background.Beacon;
    public bool SignalLevel => _isSignal;
    public bool BoostFirstLevel => _boostFirstLevel;
    public int CustomShip => _customShip;
    public string CustomWeaponModel => _customWeaponModel;

    public virtual void StartPlay() 
    {
        _background.Load();
        GameObject.FindWithTag("AudioCore").GetComponent<InteriorSoundController>().SetOSTDelayed(_soundtrack, 3f);
        InteriorSoundSetting isc = gameObject.GetComponentInChildren<InteriorSoundSetting>();
        if (isc != null)
            isc.Enforce();

        PlayerController.DefaultForce = _force;

        if (_introByExit)
        {
            VictoryHandler.CustomSceneOnExit = "Intro";
        }
        if (_clearDataOnExit)
        {
            VictoryHandler.ClearDataOnExit = true;
        }

        print("МИССИЯ ЗАПУЩЕНА");
    }

    protected virtual void TriggerVictory()
    {
        SceneStatics.CharacterCore.GetComponent<VictoryHandler>().LevelVictory();
    }


    // public void OnDisable() {
    //     VictoryHandler.MissionVictored -= OnMissionVictoried;
    //     DeathUIHandler.DeathedCleared -= OnDeathed;

    //     print("МИССИЯ ВЫКЛЮЧЕНА");
    // }

    public void SetDataForSessionSave(int locID, bool rewriteAll = true)
    {
        if (rewriteAll)
        {
            GameSessionInfoHandler.ClearGameSession();
        }

        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        save.LocationID = locID;

        save.LevelUnlockCode = _unlockID;

        save.MaxLevel = _levelCount;

        if (_firstLevelBonusModulas != 0 || _bonusModuleChoice != 0)
        {
            save.AdditiveData[boost_modules_data_collection] = new List<int>();
            save.AdditiveData[boost_modules_data_collection].Add(_firstLevelBonusModulas);
            save.AdditiveData[boost_modules_data_collection].Add(_bonusModuleChoice);
        }

        save.Boosted = BoostFirstLevel;

        if (rewriteAll)
            save.Money = _startAurite;

        save.MoneyPerLevel = _auritePerLevel;

        save.LocalizedLocationName = new TextLoader("Locations", _nameLocalizationID, 0).FirstCell;

        save.DialogueEntry = _radioID;

        GameSessionInfoHandler.RewriteSessionSave(save);

        GlobalSave gsave = GlobalSaveHandler.GetSave();
        gsave.LastSelectedLocation = locID;
        GlobalSaveHandler.RewriteSave(gsave);

        UnityEngine.Debug.Log($"Перезапись экземпляра сохранения:UID({save.LevelUnlockCode})||| {save.LocalizedLocationName}| уровней: {save.MaxLevel}|| аурит за уровень: {save.MoneyPerLevel}| космилит: {save.RecievedCosmilite}");

        VictoryHandler.MissionVictored += OnMissionVictoried;
        DeathUIHandler.DeathedCleared += OnDeathed;
    }

    private void OnDeathed()
    {
        VictoryHandler.MissionVictored -= OnMissionVictoried;
        DeathUIHandler.DeathedCleared -= OnDeathed;

        bool writeDeathedDialog = true;

        if (((PlayerPrefs.GetFloat("DialogMod", 0) == 1) && Unlocks.HasUnlock(_unlockID)) || (_uniqueDefeatDialog && Unlocks.HasUnlock(_unlockID + 1000)))
        {
            writeDeathedDialog = false;
        }

        if (writeDeathedDialog && !string.IsNullOrEmpty(_defeatMissionLobbyDialogue))
            GlobalSaveHandler.GetSave().LobbyDialogue = _defeatMissionLobbyDialogue;

        if (_uniqueDefeatDialog)
            Unlocks.NewUnlock(_unlockID + 1000);

        if (GameSessionInfoHandler.CurrentLevel == 0)
            return;

        int cosmiliteReward = Random.Range(_minMaxCosmilite.x, _minMaxCosmilite.y + 1);

        cosmiliteReward = Mathf.FloorToInt(0.6f * GameSessionInfoHandler.LevelProgressFloored * (float)cosmiliteReward);
        Bank.PutCash(BankSystem.Currency.Cosmilite, cosmiliteReward);
    }

    private void OnMissionVictoried()
    {
        VictoryHandler.MissionVictored -= OnMissionVictoried;
        DeathUIHandler.DeathedCleared -= OnDeathed;
        
        bool writeCompletedDialog = true;

        if ((PlayerPrefs.GetFloat("DialogMod", 0) == 1) && Unlocks.HasUnlock(_unlockID))
        {
            writeCompletedDialog = false;
        }

        if (writeCompletedDialog)
            GlobalSaveHandler.GetSave().LobbyDialogue = _completedMissionLobbyDialogue;

        Unlocks.ProgressUnlock(_unlockID, 1);

        if (_isSignal)
        {
            Unlocks.ProgressUnlock(7, 1); // кол-во пройденных маяков
        }
        Unlocks.ProgressUnlock(8, 1); // кол-во пройденных миссий

        int cosReward = Random.Range(_minMaxCosmilite.x, _minMaxCosmilite.y + 1) * (GameSessionInfoHandler.GetSessionSave().NoDamage ? 2 : 1);
        Bank.PutCash(BankSystem.Currency.Cosmilite, cosReward);
        print($"Начислен космилит за уровень: {cosReward}");

        if (_autoExitToLobby)
            VictoryHandler.VictorySession();
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