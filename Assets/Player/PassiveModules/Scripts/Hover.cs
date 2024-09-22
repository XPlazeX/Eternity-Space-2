using UnityEngine;

public class Hover : MonoBehaviour
{
    [SerializeField] private bool _forEnemyHover = false;

    private void OnEnable() {
        if (_forEnemyHover)
            return;
        PlayerShipData.Hover = true;
    }

    private void OnDisable() {
        if (_forEnemyHover)
            return;
        PlayerShipData.Hover = false;
    }
}
