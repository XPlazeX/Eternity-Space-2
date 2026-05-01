using UnityEngine;

[RequireComponent(typeof(DamageBody))]
public class OnDeathCurrencyReward : MonoBehaviour
{
    [SerializeField] private BankSystem.Currency _currency = BankSystem.Currency.Positronium;
    [SerializeField] private bool _unique = true;
    [SerializeField] private int _unlockCode;
    [SerializeField] private int _count;
    [SerializeField] private float _offsetSpawn;

    private void OnEnable() {
        GetComponent<DamageBody>().Deathed += OnDeathed;
    }

    public void OnDeathed()
    {
        //print($"on deathed, has unlock: {Unlocks.HasUnlock(_unlockCode)}");
        if (_unique)
        {
            if (Unlocks.HasUnlock(_unlockCode))
            return;

            Unlocks.NewUnlock(_unlockCode);
        }

        PickupSpawner ps = SceneStatics.CharacterCore.GetComponent<PickupSpawner>();

        for (int i = 0; i < _count; i++)
        {
            switch (_currency)
            {
                case BankSystem.Currency.Aurite:
                    ps.SpawnAurite(transform.position + new Vector3(Random.Range(-_offsetSpawn, _offsetSpawn), Random.Range(-_offsetSpawn, _offsetSpawn), 0f));
                    break;
                case BankSystem.Currency.Cosmilite:
                    ps.SpawnCosmilite(transform.position + new Vector3(Random.Range(-_offsetSpawn, _offsetSpawn), Random.Range(-_offsetSpawn, _offsetSpawn), 0f));
                    break;
                case BankSystem.Currency.Positronium:
                    ps.SpawnPositronium(transform.position + new Vector3(Random.Range(-_offsetSpawn, _offsetSpawn), Random.Range(-_offsetSpawn, _offsetSpawn), 0f));
                    break;
                
                default:
                    break;
            }
        }
    }
}
