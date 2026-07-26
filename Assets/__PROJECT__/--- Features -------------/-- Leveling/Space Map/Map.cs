using UnityEngine;

public static class Map
{
    public static event System.Action CurrentSectorUpdated;

    public static SectorRoot CurrentSector {get; private set;}

    public static Vector3 SectorUp => CurrentSector == null ? Vector3.up : CurrentSector.Up;
    public static Vector3 SectorDown => CurrentSector == null ? Vector3.down : CurrentSector.Down;
    public static Vector3 SectorLeft => CurrentSector == null ? Vector3.left : CurrentSector.Left;
    public static Vector3 SectorRight => CurrentSector == null ? Vector3.right : CurrentSector.Right;

    public static void SetCurrentSector(SectorRoot sectorRoot)
    {
        CurrentSector = sectorRoot;
        CurrentSectorUpdated?.Invoke();
    }
}
