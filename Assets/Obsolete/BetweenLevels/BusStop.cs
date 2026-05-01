using UnityEngine;

public class BusStop : MonoBehaviour
{
    [SerializeField] private Bus[] _buses;
    [SerializeField] private Doc[] _docs;
    [SerializeField] private bool _testMode = false;

    private void Start() {
        if (_testMode)
        {
            Invoke("SpawnDoc", 2f);
        }
    }

    public void SpawnBus()
    {
        // Quaternion borders = CameraController.Borders_xXyY;

        // Bus spawnedBus = Instantiate(_buses[Skins.SOCurrentSkin()], new Vector3(
        //     Random.Range(borders.x + 1.5f, borders.y - 1.5f),
        //     Random.Range(borders.z / 2.5f, borders.w / 1.33f)), Quaternion.identity);
    }

    public void SpawnDoc()
    {
        // Quaternion borders = CameraController.Borders_xXyY;

        // Doc spawnedBus = Instantiate(_docs[Skins.SOCurrentSkin()], new Vector3(
        //     Random.Range(borders.x + 1.5f, borders.y - 1.5f),
        //     Random.Range(borders.z / 2.5f, borders.w / 1.33f)), Quaternion.identity);
    }
}

