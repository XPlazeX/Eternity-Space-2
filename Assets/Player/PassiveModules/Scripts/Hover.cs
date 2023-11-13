using UnityEngine;

public class Hover : MonoBehaviour
{
    private void OnEnable() {
        PlayerShipData.Hover = true;
    }

    private void OnDisable() {
        PlayerShipData.Hover = false;
    }
}
