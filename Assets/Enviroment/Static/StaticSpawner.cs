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

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3();

            while (pos.magnitude < 3f)
            {
                pos = ArenaLocal.GetRandomFieldPosition(1f);
            } 

            Instantiate(_spawnObjects[Random.Range(0, _spawnObjects.Length)], pos, Quaternion.Euler(0, 0, Random.Range(0, 360f)));
        }
    }
}
