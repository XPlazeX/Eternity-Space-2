using UnityEngine;

public class UnlockPickup : Pickup
{
    [SerializeField] private int _unlockCode;
    [SerializeField] private int _addingProgress;

    protected override void Picked()
    {
        Unlocks.ProgressUnlock(_unlockCode, _addingProgress);
        base.Picked();
    }
}
