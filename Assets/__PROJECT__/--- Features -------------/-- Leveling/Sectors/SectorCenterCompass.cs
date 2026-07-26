using UnityEngine;

public class SectorCenterCompass : MonoBehaviour
{
    private void Update() {
        Vector3 target = Map.CurrentSector == null ? Vector3.zero : Map.CurrentSector.Center;
        transform.up = target - transform.position;
    }
}
