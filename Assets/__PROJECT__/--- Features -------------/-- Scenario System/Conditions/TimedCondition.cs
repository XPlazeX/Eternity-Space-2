using ScenarioSystem;
using UnityEngine;

public class TimedCondition : Condition
{
    [Header("Требует, чтобы прошло X времени в секундах.")]
    [SerializeField] TimedConditionMode timedConditionMode;
    [SerializeField] private float requiringTime;

    public override bool Evaluate(ScenarioContext ctx)
    {
        if (timedConditionMode == TimedConditionMode.ScenarioTime)
        {
            return ctx.ScenarioTime >= requiringTime;
        } else if (timedConditionMode == TimedConditionMode.LastNodeTime)
        {
            return ctx.NodeTime >= requiringTime;
        }
        
        return false;
    }

    private enum TimedConditionMode
    {
        ScenarioTime = 0,
        LastNodeTime = 1
    }
}
