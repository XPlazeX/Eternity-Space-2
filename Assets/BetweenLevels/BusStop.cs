using UnityEngine;

public class BusStop : MonoBehaviour
{
    [SerializeField] private Bus[] _buses;

    public void SpawnBus()
    {
        Quaternion borders = CameraController.Borders_xXyY;

        int id = 0;
        if (Dev.RuStoreVersionSprites)
            id = 1;

        Bus spawnedBus = Instantiate(_buses[id], new Vector3(
            Random.Range(borders.x / 2f, borders.y / 2f),
            Random.Range(borders.z / 2f, borders.w / 2f)), Quaternion.identity);
    }
}

