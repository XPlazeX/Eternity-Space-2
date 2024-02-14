using UnityEngine;

public class BusStop : MonoBehaviour
{
    [SerializeField] private Bus[] _buses;
    [SerializeField] private Doc[] _docs;

    public void SpawnBus()
    {
        Quaternion borders = CameraController.Borders_xXyY;

        int id = 0;
        if (Dev.RuStoreVersionSprites)
            id = 1;

        Bus spawnedBus = Instantiate(_buses[id], new Vector3(
            Random.Range(borders.x + 1.5f, borders.y - 1.5f),
            Random.Range(borders.z / 1.33f, borders.w / 1.33f)), Quaternion.identity);
    }

    public void SpawnDoc()
    {
        Quaternion borders = CameraController.Borders_xXyY;

        int id = 0;
        // if (Dev.RuStoreVersionSprites)
        //     id = 1;

        Doc spawnedBus = Instantiate(_docs[id], new Vector3(
            Random.Range(borders.x + 1.5f, borders.y - 1.5f),
            Random.Range(borders.z / 1.33f, borders.w / 1.33f)), Quaternion.identity);
    }
}

