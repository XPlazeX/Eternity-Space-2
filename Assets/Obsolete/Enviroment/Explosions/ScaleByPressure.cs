using UnityEngine;

public class ScaleByPressure : MonoBehaviour
{
    private void Start() {
        transform.localScale *= (1f / ShipStats.GetValue("Pressure"));
    }
}
