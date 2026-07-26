using ScenarioSystem;
using UnityEngine;

public class ZoneArriveCondition : Condition
{
    [SerializeField] private string targetZoneId;
    [SerializeField] private float targetZoneTime = 3f;

    public override bool Evaluate(ScenarioContext ctx)
    {
        foreach (var zone in ctx.SectorStateSnapshot.playerCurrentSectorsTimes)
        {
            if (zone.Key == targetZoneId && zone.Value >= targetZoneTime)
            {
                return true;
            }
        }

        return false;
    }
}
