using UnityEngine;

public class SpawningMission : Mission
{
    [Header("BonusSpawning")]
    [SerializeField] private bool _hasBonusSpawn;
    [SerializeField] private string[] _bonusEnemies;
    [SerializeField][Range(-5, 10)] private int _bonusCount;
    [SerializeField] private int _bonusLerpBoostOnFinalLevel = 0;
    [SerializeField] private float _bonusSpawnReload;
    [Header("Stalkers")]
    [SerializeField] private bool _hasStalkers = false;
    [SerializeField][Range(0, 1f)] private float _chance;

    public override void StartPlay()
    {
        SceneStatics.SceneCore.GetComponent<Spawner>().SetWaveHeadText(SceneLocalizator.GetLocalizedString("Game", 0, 0));

        if (_hasBonusSpawn)
        {
            BonusSpawner bs = SceneStatics.SceneCore.GetComponent<BonusSpawner>();
            bs.SetParams(_bonusEnemies, _bonusCount + Mathf.RoundToInt(Mathf.Lerp(0f, _bonusLerpBoostOnFinalLevel, GameSessionInfoHandler.LevelProgress)), _bonusSpawnReload);

            bs.StartSpawning();
        }
        
        if (_hasStalkers)
        {
            StalkerSpawner ss = SceneStatics.SceneCore.GetComponent<StalkerSpawner>();
            ss.SetChance(_chance);

            ss.StartSpawning();
        }

        base.StartPlay();
    }
}
