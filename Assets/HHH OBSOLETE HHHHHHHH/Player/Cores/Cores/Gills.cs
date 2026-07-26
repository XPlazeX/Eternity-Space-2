using UnityEngine;

public class Gills : Core
{
    private Vector3 _oldPlayerPosition;

    private void Update() {
        Vector3 delta = Player.PlayerTransform.position - _oldPlayerPosition;

        PlayerCore.AddEnergy(_megawattsGrowth * delta.magnitude * Effeciency);

        _oldPlayerPosition = Player.PlayerTransform.position;
    }
}
