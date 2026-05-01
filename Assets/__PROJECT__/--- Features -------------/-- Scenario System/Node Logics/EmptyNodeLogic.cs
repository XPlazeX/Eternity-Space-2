using ScenarioSystem;
using UnityEngine;

public class EmptyNodeLogic : NodeLogicData
{
    public override ActiveNodeLogicRuntime CreateRuntime()
    {
        EmptyNodeLogicRuntime runtime = new EmptyNodeLogicRuntime();
        runtime.data = this;

        return runtime;
    }
}

public class EmptyNodeLogicRuntime : ActiveNodeLogicRuntime
{
    public override void Begin(ScenarioContext context)
    {
        return;
    }

    public override void End(ScenarioContext context)
    {
        return;
    }

    public override void Tick(ScenarioContext context, float dt)
    {
        return;
    }
}
