public class PositroniumPickup : Pickup
{
    protected override void Picked()
    {
        SceneStatics.CharacterCore.GetComponent<VictoryHandler>().AddPositronium(1);
        base.Picked();
    }
}
