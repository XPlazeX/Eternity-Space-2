using UnityEngine;

public class SpawningMission : Mission
{
    [Header("BonusSpawning")]
    [SerializeField] private bool _hasBonusSpawn;
    [SerializeField] private string[] _bonusEnemies;
    [SerializeField][Range(-5, 10)] private int _bonusCount;
    [SerializeField] private float _bonusSpawnReload;

    public override void StartPlay()
    {
        if (_hasBonusSpawn)
        {
            BonusSpawner bs = SceneStatics.SceneCore.GetComponent<BonusSpawner>();
            bs.SetParams(_bonusEnemies, _bonusCount, _bonusSpawnReload);

            bs.StartSpawning();
        }

        base.StartPlay();
    }
}
