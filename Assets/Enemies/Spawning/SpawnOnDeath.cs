using UnityEngine;

[RequireComponent(typeof(DamageBody))]
public class SpawnOnDeath : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private DamageBody _damageBody;
    [SerializeField] private int _count;
    [SerializeField] private float _spawnSpread;

    private void Start() {
        GetComponent<DamageBody>().Deathed += Spawn;
    }

    private void Spawn()
    {
        for (int i = 0; i < _count; i++)
        {
            Spawner.SpawnDamageBody(_damageBody, _spawnPoint.position + new Vector3(Random.Range(-_spawnSpread, _spawnSpread), Random.Range(-_spawnSpread, _spawnSpread), 0));   
        }
    }
}
