using UnityEngine;
using ScenarioSystem;

public class EncounterClearCondition : Condition
{
    [Tooltip("Время чтобы энкаунтер успел активироваться")]
    [SerializeField] private float nodeTimeJitter = 5f;
    [SerializeField] EncounterClearConditionMode encounterClearConditionMode;

    public override bool Evaluate(ScenarioContext ctx)
    {
        if (ctx.NodeTime < nodeTimeJitter)
            return false; // чтобы энкаунтер успел запуститься, а мы не проверяли условие в первые секунды ноды

        switch (encounterClearConditionMode)
        {
            case EncounterClearConditionMode.NoEnemiesAlive:
                return !ctx.AnyEnemyAlive;
            case EncounterClearConditionMode.OnlyNoBossesAlive:
                return ctx.BossesAlive == 0;
            case EncounterClearConditionMode.ZeroEnemiesWeight:
                return ctx.EnemiesWeight <= 0f;
        }
        
        return false;
    }

    public enum EncounterClearConditionMode
    {
        NoEnemiesAlive,
        OnlyNoBossesAlive,
        ZeroEnemiesWeight
    }
}
