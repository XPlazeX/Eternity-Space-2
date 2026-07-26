using UnityEngine;

public class SectorCompass : MonoBehaviour
{
    void Update()
    {
        transform.up = Map.SectorUp;
    }
}
