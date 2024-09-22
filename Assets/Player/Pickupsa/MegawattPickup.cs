using UnityEngine;

public class MegawattPickup : Pickup
{
    [SerializeField] private float _megawatts;

    protected override void Picked()
    {
        PlayerCore.AddEnergy(_megawatts);
        base.Picked();
    }
}
