using UnityEngine;

[RequireComponent(typeof(DamageBody))]
public class SpawnOnDeath : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private GameObject _spawningObject;
    [SerializeField] private int _count;
    [SerializeField] private float _spawnSpread;

    private void Start() {
        GetComponent<DamageBody>().Deathed += Spawn;
    }

    private void Spawn()
    {
        for (int i = 0; i < _count; i++)
        {
            if (_spawningObject.GetComponent<DamageBody>() != null)
                Spawner.SpawnDamageBody(_spawningObject.GetComponent<DamageBody>(), _spawnPoint.position + new Vector3(Random.Range(-_spawnSpread, _spawnSpread), Random.Range(-_spawnSpread, _spawnSpread), 0));   

            else
                Instantiate(_spawningObject, _spawnPoint.position, Quaternion.identity).GetComponent<DamageBody>();
        }
    }
}
