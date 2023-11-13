using UnityEngine;

[System.Serializable]
public class MissionLevelCore
{
    [SerializeField] private BackgroundLoader _background;
    [SerializeField] private SpawnerModifier _mainSpawnerModifier;
    [SerializeField] private LevelEnemyDatabase _enemyDB;
    [SerializeField] private SoundObject _soundtrack;
    [Header("Дополнительно")]
    [SerializeField] private SpawnObject[] _additiveObjects;

    public void Enforce(bool beacon)
    {
        bool alone = SceneTransition.ActiveSceneName != "Game";

        // if (beacon)
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

        //Debug.Log("Дополнительный спаун отключён");

        // if (_additiveEDBs.Length != 0)
        // {
        //     GameObject.FindWithTag("Level core").GetComponent<EnvironmentSpawner>().AddEnvironmentDB(_additiveEDBs[0]);
        // }
        if (_additiveObjects != null)
        {
            for (int i = 0; i < _additiveObjects.Length; i++)
            {
                MonoBehaviour.Instantiate(_additiveObjects[i].gameObject, _additiveObjects[i].spawnPosition, Quaternion.Euler(0, 0, _additiveObjects[i].spawnAngles));
            }
        }

        SceneStatics.AudioCore.GetComponent<SoundPlayer>().SetSoundtrack(_soundtrack);

        // if (GameSessionInfoHandler.GetSessionSave().CurrentLevel != GameSessionInfoHandler.GetSessionSave().MaxLevel - 1)
        // {
        //     _hasBoss = false;
        // }

        SceneStatics.SceneCore.GetComponent<Spawner>().Initialize();

        //if (GameSessionInfoHandler.GetSessionSave().Bossfight)
            

        // if (_hasBoss)
        //     SceneStatics.SceneCore.GetComponent<Spawner>().SetBoss(_bosses);
    }
}
