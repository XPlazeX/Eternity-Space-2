using UnityEngine;
using ScenarioSystem;
using System;

public class ParsingCoordinateTransition : TransitionData
{
    [SerializeField] private Vector3 targetCoordinates;

    public Vector3 TargetCoordinates => targetCoordinates;

    public override ActiveTransitionRuntime CreateRuntime()
    {
        ParsingCoordinateTransitionRuntime runtime = new ParsingCoordinateTransitionRuntime();
        runtime.data = this;

        return runtime;
    }
}

public class ParsingCoordinateTransitionRuntime : ActiveTransitionRuntime
{
    public override event Action Completed;

    protected SledgeTransitor _sledgeTransitor;

    public override void Begin(ScenarioContext context)
    {
        _sledgeTransitor = GameObject.FindAnyObjectByType<SledgeTransitor>();

        if (_sledgeTransitor == null)
        {
            throw new Exception("Не найден ни один SledgeTransitor.");
        }

        _sledgeTransitor.SetTargetCoordinates(((ParsingCoordinateTransition)data).TargetCoordinates);
        _sledgeTransitor.StartAwaitTransition();
        SledgeTransitor.Arrived += OnArrived;
    }

    private void OnArrived()
    {
        Completed?.Invoke();
    }

    public override void End(ScenarioContext context)
    {
        SledgeTransitor.Arrived -= OnArrived;
        return;
    }

    public override float GetProgress01(ScenarioContext ctx)
    {
        return SledgeTransitor.TransitionProggress01;
    }

    public override void Tick(ScenarioContext context, float dt)
    {
        return;
    }
}