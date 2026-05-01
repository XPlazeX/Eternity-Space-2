using System;
using ScenarioSystem;

public class InstantTransition : TransitionData
{
    public override ActiveTransitionRuntime CreateRuntime()
    {
        InstantActiveTransitionRuntime runtime = new InstantActiveTransitionRuntime();
        runtime.data = this;

        return runtime;
    }
}

public class InstantActiveTransitionRuntime : ActiveTransitionRuntime
{
    public override event Action Completed;

    public override void Begin(ScenarioContext context)
    {
        return;
    }

    public override void End(ScenarioContext context)
    {
        return;
    }

    public override float GetProgress01(ScenarioContext ctx)
    {
        return 1f;
    }

    public override void Tick(ScenarioContext context, float dt)
    {
        Completed?.Invoke();
    }
}
