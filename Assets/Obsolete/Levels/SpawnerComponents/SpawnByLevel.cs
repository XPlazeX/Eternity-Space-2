using UnityEngine;

public class SpawnByLevel : MonoBehaviour
{
    [SerializeField] private int _targetLevel = 0;
    [SerializeField] private float _delay;
    [SerializeField] private SpawnObject[] _spawnObjects;

    private void Start() 
    {
        if (GameSessionInfoHandler.CurrentLevel == _targetLevel)
        {
            Invoke("Spawn", _delay);
        }
    }

    private void Spawn()
    {
        if (_spawnObjects.Length == 0)
            return;
        
        for (int i = 0; i < _spawnObjects.Length; i++)
        {
            DamageBody db = Instantiate(_spawnObjects[i].gameObject, _spawnObjects[i].spawnPosition, Quaternion.Euler(0,0, _spawnObjects[i].spawnAngles)).GetComponent<DamageBody>();
            // if (db != null)
            // {
            //     Spawner.InitializeHPBar(db);
            // }
        }
    }
}
