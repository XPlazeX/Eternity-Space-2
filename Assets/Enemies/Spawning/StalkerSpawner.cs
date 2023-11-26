using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class StalkerSpawner : SpawnerRoot
{
    [SerializeField] private AssetReference[] _stalkers;
    
    private AsyncOperationHandle _stalkerOperationHandle;
    public float StalkerChance {get; private set;} = 0f;

    public void SetChance(float chance) => StalkerChance = chance;

    public override void StartSpawning()
    {
        print("СПАУНЕР СТАЛКЕРОВ ЗАПУЩЕН");

        base.StartSpawning();
    }

    protected override bool CheckConditions()
    {
        return true;
    }

    protected override IEnumerator SpawnBody()
    {
        yield return new WaitForSeconds(_reloadTime);

        if ((Random.value / ShipStats.GetValue("StalkerChanceMultiplier")) > StalkerChance)
            yield break;

        yield return StartCoroutine(SpawningStalker());
    }

    public IEnumerator SpawningStalker()
    {
        if (_stalkerOperationHandle.IsValid())
        {
            Addressables.Release(_stalkerOperationHandle);
        }

        var stalkerReference = _stalkers[Random.Range(0, _stalkers.Length)];
        _stalkerOperationHandle = stalkerReference.LoadAssetAsync<GameObject>();
        yield return _stalkerOperationHandle;

        if (_stalkerOperationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            DamageBody spawnedStalker = Spawner.SpawnDamageBody(((GameObject)_stalkerOperationHandle.Result).GetComponent<DamageBody>());

            Spawner.InitializeHPBar(spawnedStalker);
            spawnedStalker.PositionDeathed += OnStalkerDeath;
        }
    }

    public void OnStalkerDeath(Vector3 position)
    {
        SceneStatics.CharacterCore.GetComponent<PickupSpawner>().SpawnPositronium(position);
    }
}
