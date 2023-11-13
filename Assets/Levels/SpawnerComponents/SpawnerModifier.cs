using UnityEngine;

[System.Serializable]
public class SpawnerModifier : TypeModifier {
    [Header("Спаунер волн")]
    [Range(0.1f, 10f)][SerializeField] private float _additiveWaveCountMultiplier = 1f;
    [Range(0.1f, 10f)][SerializeField] private float _additiveWeightMultiplier = 1f;
    [Range(0, 10)][SerializeField] private int _additiveEnemyBuffer = 0;
    [SerializeField] private float _enemySpawnReloadMultiplier = 1f;
    [Space()]
    [Header("Бонусный спаунер")]
    [Range(-10, 10)][SerializeField] private int _flatBonusEnemiesBonus = 0;
    [SerializeField] private float _bonusEnemySpawnReloadMultiplier = 1f;
    [Header("Прогрессия по уровням.")]
    [SerializeField] private bool _levelProgression;
    [SerializeField] private float _missionWeightAdditiveFinalMultiplier = 1f;
    [SerializeField][Range(-10, 10)] private int _additiveBonusEnemies;

    public void Enforce()
    {
        BonusSpawner bonusSpawner = SceneStatics.SceneCore.GetComponent<BonusSpawner>();
        EnemyDBSpawner edbSpawner = SceneStatics.SceneCore.GetComponent<EnemyDBSpawner>();

        if (_levelProgression)
        {
            GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
            float multiplier = Mathf.Lerp(1f, _missionWeightAdditiveFinalMultiplier, GameSessionInfoHandler.LevelProgress);
            Debug.Log($"level hardness multiplier: {multiplier} уровень {save.CurrentLevel} / {save.MaxLevel}");

            _additiveWaveCountMultiplier *= multiplier;
            _additiveWeightMultiplier *= multiplier;
            _flatBonusEnemiesBonus += Mathf.CeilToInt(multiplier * _additiveBonusEnemies);
        }

        bonusSpawner.Modify(_flatBonusEnemiesBonus, _bonusEnemySpawnReloadMultiplier);
        edbSpawner.Modify(_additiveWaveCountMultiplier, _additiveWeightMultiplier, _additiveEnemyBuffer, _enemySpawnReloadMultiplier);
        
        Debug.Log($"применен модификатор спаунера, волны: {_additiveWaveCountMultiplier}, вес: {_additiveWeightMultiplier}, буфер: {_additiveEnemyBuffer}, бонусные: {_flatBonusEnemiesBonus}");
    }
}

