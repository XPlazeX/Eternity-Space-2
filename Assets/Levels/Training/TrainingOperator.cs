using UnityEngine;

public class TrainingOperator : MonoBehaviour
{
    [SerializeField] private string _trainingLocalizationFilename;
    [SerializeField] private int _tipCount;
    [SerializeField] private ControllableText _controllableText;
    [SerializeField] private Vector3 _textPosition;
    [SerializeField] private SpawnObject[] _spawnObjects;

    private int _currentTip = 0;
    private ControllableText _sampleText;

    private void Start() {
        Initialize();
    }

    public void Initialize()
    {
        //SceneStatics.CharacterCore.GetComponent<ModuleCore>().SpawnGear(ModuleWork.GearType.Weapon, training_weapon_name);

        _sampleText = Instantiate(_controllableText, _textPosition, Quaternion.identity);
        _sampleText.Initialize(_trainingLocalizationFilename, 0);

        for (int i = 0; i < _spawnObjects.Length; i++)
        {
            Instantiate(_spawnObjects[i].gameObject, _spawnObjects[i].spawnPosition, Quaternion.Euler(0, 0, _spawnObjects[i].spawnAngles));
        }

        SceneStatics.SceneCore.GetComponent<EnemyDBSpawner>().WaveCompleted += NextTip;
        SceneStatics.SceneCore.GetComponent<EnemyDBSpawner>().Modify(0.1f * _tipCount, 1f, 0, 1f);

        PlayerShipData.PlayerDeath += RestartTraining;
        // PausePanelMethods.SavedAndExited += EndTraining;
        VictoryHandler.MissionVictored += EndTraining;
        VictoryHandler.CustomSceneOnVictory = "MissionMenu";
        VictoryHandler.CustomSceneOnExit = "Intro";
    }

    public void NextTip()
    {
        _currentTip ++;

        _sampleText.SetText(_currentTip);

        // if (_currentTip == _tipCount)
        // {
        //     EndTraining();
        // }
    }

    public void EndTraining()
    {
        GameSessionInfoHandler.ClearGameSession();

        PrepareSimulationTraining();

        print("END TRAINING");
    }

    public static void PrepareSimulationTraining()
    {
        GameObject.FindWithTag("BetweenScenes").GetComponent<MissionsDatabase>().SetSessionData(1);

        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
        save.AllSysInitialized();
        GameSessionInfoHandler.RewriteSessionSave(save);
    }

    public void RestartTraining()
    {
        SceneTransition.ReloadScene();
    }

    private void OnDisable() {
        PlayerShipData.PlayerDeath -= RestartTraining;
        // PausePanelMethods.SavedAndExited -= EndTraining;
        VictoryHandler.MissionVictored -= EndTraining;
    }
}
