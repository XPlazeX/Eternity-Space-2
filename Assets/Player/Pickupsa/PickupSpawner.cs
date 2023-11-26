using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _pickupPool;
    [SerializeField] private GameObject _positroniumPickup;

    [SerializeField][Range(0, 1f)] private float _chance;

    private void Start() {
        Spawner.DamageBodySpawned += BindDamageBody;
    }

    private void OnDisable() {
        Spawner.DamageBodySpawned -= BindDamageBody;
    }

    private void BindDamageBody(DamageBody db)
    {
        db.PositionDeathed += TrySpawn;
    }

    private void TrySpawn(Vector3 pos)
    {
        if (Random.value > _chance * ShipStats.GetValue("PickupChanceMultiplier"))
            return;

        Instantiate(_pickupPool[Random.Range(0, _pickupPool.Length)], pos, Quaternion.Euler(0, 0, Random.Range(-90f, 90f)));
    } 

    public void SpawnPositronium(Vector3 pos)
    {
        Instantiate(_positroniumPickup, pos, Quaternion.identity);
    }
}
