using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "LevelCore", menuName = "Teleplane2/LevelCore", order = 0)]
public class LevelCore : ScriptableObject {
    [SerializeField] private BackgroundLoader _background;
    [SerializeField] private SpawnerModifier _mainSpawnerModifier;
    [SerializeField] private LevelEnemyDatabase _enemyDB;
    [SerializeField] private SoundObject _soundtrack;
    [Header("Дополнительно")]
    [SerializeField] private EnvironmentDB[] _additiveEDBs;
    [SerializeField] private SpawnObject[] _additiveObjects;

    public void Enforce()
    {
        bool alone = SceneManager.GetActiveScene().name != "Game";

        // if (GameSessionInfoHandler.GetSessionSave().BeaconLevel)
        // {
        //     _background.BeaconInitialize(alone);
        // } else 
        // {
        //     _background.Initialize(alone);
        // }

        if (alone)
            return;

        _mainSpawnerModifier.Enforce();

        _enemyDB.Initialize();

        Debug.Log("Дополнительный спаун отключён");

        // if (_additiveEDBs.Length != 0)
        // {
        //     GameObject.FindWithTag("Level core").GetComponent<EnvironmentSpawner>().AddEnvironmentDB(_additiveEDBs[0]);
        // }
        if (_additiveObjects != null)
        {
            for (int i = 0; i < _additiveObjects.Length; i++)
            {
                Instantiate(_additiveObjects[i].gameObject, _additiveObjects[i].spawnPosition, Quaternion.Euler(0, 0, _additiveObjects[i].spawnAngles));
            }
        }

        SceneStatics.AudioCore.GetComponent<SoundPlayer>().SetSoundtrack(_soundtrack);
    }

}
