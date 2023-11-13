using UnityEngine;

public class BusStop : MonoBehaviour
{
    [SerializeField] private Bus _bus;
    [SerializeField] private Bus _portal;

    public void SpawnBus()
    {
        Quaternion borders = CameraController.Borders_xXyY;

        Bus spawnedBus = Instantiate(_bus, new Vector3(
            Random.Range(borders.x / 2f, borders.y / 2f),
            Random.Range(borders.z / 2f, borders.w / 2f)), Quaternion.identity);
    }
}

