using UnityEngine;

public class StaticSpawner : MonoBehaviour
{
    [SerializeField] private Vector2Int _minMaxCount;
    [SerializeField] private GameObject[] _spawnObjects;
    [SerializeField] private bool _autostart = true;

    private void Start() {
        if (_autostart)
            Spawn();
    }

    public void Spawn()
    {
        int count = Random.Range(_minMaxCount.x, _minMaxCount.y + 1);
        Quaternion borders = CameraController.Borders_xXyY;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3();

            while (pos.magnitude < 3f)
            {
                pos = new Vector3(Random.Range(borders.x + 1f, borders.y - 1f), Random.Range(borders.z + 1f, borders.w - 1f), 0f);
            } 

            Instantiate(_spawnObjects[Random.Range(0, _spawnObjects.Length)], pos, Quaternion.Euler(0, 0, Random.Range(0, 360f)));
        }
    }
}
