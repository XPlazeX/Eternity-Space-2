using UnityEngine;
using ScenarioSystem;

public class EncounterThresholdCondition : Condition
{
    [SerializeField] private bool useEnemyCountThreshold = true;
    [SerializeField] private int enemyCountThreshold = 5;
    [SerializeField] private bool useBossCountThreshold = false;
    [SerializeField] private int bossCountThreshold = 1;
    [SerializeField] private bool useEnemyWeightThreshold = false;
    [SerializeField] private float enemyWeightThreshold = 10f;
    [SerializeField] private bool useDefeatedEnemyCountThreshold = false;
    [SerializeField] private int defeatedEnemyCountThreshold = 10;
    [SerializeField] private bool useDefeatedBossCountThreshold = false;
    [SerializeField] private int defeatedBossCountThreshold = 1;

    public override bool Evaluate(ScenarioContext ctx)
    {
        if (useEnemyCountThreshold && ctx.EnemiesAlive < enemyCountThreshold)
            return true;
        if (useBossCountThreshold && ctx.BossesAlive < bossCountThreshold)
            return true;
        if (useEnemyWeightThreshold && ctx.EnemiesWeight < enemyWeightThreshold)
            return true;
        if (useDefeatedEnemyCountThreshold && ctx.DefeatedEnemiesCount >= defeatedEnemyCountThreshold)
            return true;
        if (useDefeatedBossCountThreshold && ctx.DefeatedBosses >= defeatedBossCountThreshold)
            return true;
        
        return false;
    }
}
