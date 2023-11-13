using UnityEngine;

public class SpawnOnStart : MonoBehaviour
{
    [SerializeField] private SpawnObject[] _spawnObjects;

    private void Start() 
    {
        if (SceneTransition.SceneReady)
        {
            Spawn();
            return;
        }    

        SceneTransition.SceneOpened += Spawn;
    }

    private void Spawn()
    {
        if (_spawnObjects.Length == 0)
            return;
        
        for (int i = 0; i < _spawnObjects.Length; i++)
        {
            DamageBody db = Instantiate(_spawnObjects[i].gameObject, _spawnObjects[i].spawnPosition, Quaternion.Euler(0,0, _spawnObjects[i].spawnAngles)).GetComponent<DamageBody>();
            if (db != null)
            {
                Spawner.InitializeHPBar(db);
            }
        }

        SceneTransition.SceneOpened -= Spawn;
    }
}

[System.Serializable]
public struct SpawnObject
{
    public GameObject gameObject;
    public Vector3 spawnPosition;
    public float spawnAngles;
}
