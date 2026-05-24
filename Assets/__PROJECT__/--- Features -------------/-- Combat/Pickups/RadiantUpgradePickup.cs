using UnityEngine;

public class RadiantUpgradePickup : Pickup
{
    protected override void Picked()
    {
        SimpleCoreV1.RadiantPowerup();

        base.Picked();
    }
}
