using UnityEngine;

public class Elitizer : MonoBehaviour
{
    private const float pickup_spawn_offset = 0.5f;

    [SerializeField] private float _hpMultiplier = 1.5f;
    [SerializeField] private int _hpAdditive = 20;
    [SerializeField] private int _shieldAdditive = 0;
    [SerializeField] private int _flatArmor = 0;
    [SerializeField] private float _aggroMultiplier = 1.33f;
    [SerializeField] private float _mobilityMultiplier = 1.33f;
    [Header("Spawns")]
    [SerializeField] private GameObject[] _spawnsOnDefeat;
    [SerializeField][Range(0, 10)] private int _currencyPickupSpawns;

    protected DamageBody _bindedDB;

    private void Start() {
        _bindedDB = transform.parent.GetComponent<DamageBody>();
        _bindedDB.Deathed += OnDeathed;
        Enforce();
    }

    protected void Enforce()
    {
        _bindedDB.MultiplyHP(_hpMultiplier);
        _bindedDB.AddMaxHP(_hpAdditive);
        _bindedDB.GetShield(_bindedDB.StartShield + _shieldAdditive);
        _bindedDB.AddFlatArmor(_flatArmor);

        EnemyAIRoot eair = _bindedDB.GetComponent<EnemyAIRoot>();

        if (eair != null)
        {
            eair.LocalMultiplyMobility(_mobilityMultiplier);
        }

        IAttackModule[] attackModules = _bindedDB.GetComponents<IAttackModule>();

        for (int i = 0; i < attackModules.Length; i++)
        {
            attackModules[i].LocalMultiplyAggro(_aggroMultiplier);
        }
    }

    protected void OnDeathed()
    {
        for (int i = 0; i < _spawnsOnDefeat.Length; i++)
        {
            Instantiate(_spawnsOnDefeat[i], transform.position + new Vector3(Random.Range(-pickup_spawn_offset, pickup_spawn_offset), Random.Range(-pickup_spawn_offset, pickup_spawn_offset), 0f), Quaternion.identity);
        }

        PickupSpawner pickupSpawner = SceneStatics.CharacterCore.GetComponent<PickupSpawner>();

        for (int i = 0; i < _currencyPickupSpawns; i++)
        {
            if (GameSessionInfoHandler.FinalLevel)
                pickupSpawner.SpawnCosmilite(transform.position + new Vector3(Random.Range(-pickup_spawn_offset, pickup_spawn_offset), Random.Range(-pickup_spawn_offset, pickup_spawn_offset), 0f));
            else
            {
                pickupSpawner.SpawnAurite(transform.position + new Vector3(Random.Range(-pickup_spawn_offset, pickup_spawn_offset), Random.Range(-pickup_spawn_offset, pickup_spawn_offset), 0f));
            }
        }
    }
}
