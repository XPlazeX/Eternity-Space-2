using UnityEngine;

public class RepairPickup : Pickup
{
    [SerializeField] private int _healValue = 50;

    protected override void Picked()
    {
        PlayerShipData.RegenerateHP(_healValue);

        // GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
        // save.Repaired = true;

        // GameSessionInfoHandler.RewriteSessionSave(save);
        // GameSessionInfoHandler.SaveAll();

        base.Picked();
    }
}
