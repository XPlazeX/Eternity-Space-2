using UnityEngine;
using ScenarioSystem;

public class SledgeWaitCondition : Condition
{
    private const float SCENARIO_TIME_JITTER = 1f;

    public override bool Evaluate(ScenarioContext ctx)
    {
        return !ctx.IsParsing && ctx.ScenarioTime > SCENARIO_TIME_JITTER;
    }
}
