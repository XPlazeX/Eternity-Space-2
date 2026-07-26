using ScenarioSystem;
using UnityEngine;

public class AlarmCondition : Condition
{
    [SerializeField] private float timeSinceAlarmRequirement = 10f;

    public override bool Evaluate(ScenarioContext ctx)
    {
        return ctx.EnemyHq.State == EnemyHQState.Alarm && ctx.EnemyHq.TimeSinceAlarm > timeSinceAlarmRequirement;
    }
}
