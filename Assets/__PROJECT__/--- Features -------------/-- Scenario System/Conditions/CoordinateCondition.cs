using ScenarioSystem;
using UnityEngine;

public class CoordinateCondition : Condition
{
    [SerializeField] private CoordinateConditionMode coordinateConditionMode;
    [SerializeField] private Vector3 destinationPoint = Vector3.zero;
    [SerializeField] private float tolerance = 5f;

    public override bool Evaluate(ScenarioContext ctx)
    {
        Vector3 targetPos = Vector3.zero;

        switch (coordinateConditionMode)
        {
            case CoordinateConditionMode.PivotArrival:
                targetPos = ctx.CurrentPivotPosition;
            break;
            case CoordinateConditionMode.PlayerArrival:
                targetPos = ctx.CurrentPlayerPosition;
            break;
            default:
                break;
        }

        return (targetPos - destinationPoint).magnitude < tolerance;
    }

    public enum CoordinateConditionMode
    {
        PivotArrival,
        PlayerArrival
    }
}
