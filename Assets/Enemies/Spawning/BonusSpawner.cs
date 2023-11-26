using System.Collections;
using UnityEngine;

public class BonusSpawner : SpawnerRoot
{
    public string[] BonusEnemies {get; private set;}
    public int MaxCount {get; private set;} = 0;
    public float ReloadTime {get; private set;}
    public int ActiveEnemies {get; private set;}

    public void SetParams(string[] enemies, int count, float reload)
    {
        BonusEnemies = enemies;
        MaxCount = count;
        ReloadTime = reload;
    }

    public void Modify(int additiveCount, float reloadMultiplier)
    {
        MaxCount += additiveCount;
        ReloadTime *= reloadMultiplier;
    }

    protected override void Start() {
        base.Start();
        SceneStatics.SceneCore.GetComponent<Spawner>().PrintBonusCountUI(0);
    }

    public override void StartSpawning()
    {
        print("БОНУСНЫЙ СПАУНЕР ЗАПУЩЕН");

        base.StartSpawning();
    }

    protected override bool CheckConditions()
    {
        return true;
    }

    protected override IEnumerator SpawnBody()
    {
        while (ActiveEnemies < MaxCount)
        {
            yield return new WaitForSeconds(ReloadTime);

            var request = Resources.LoadAsync<DamageBody>($"Enemies/Bonus/Bonus {BonusEnemies[Random.Range(0, BonusEnemies.Length)]}");
            yield return request;

            DamageBody spawnedDB = Spawner.SpawnDamageBody((DamageBody)request.asset);

            spawnedDB.Deathed += UnregisterEnemy;
            ActiveEnemies ++;
            _spawner.PrintBonusCountUI(ActiveEnemies);

            Spawner.InitializeHPBar(spawnedDB);
        }
    }

    public void UnregisterEnemy()
    {
        ActiveEnemies --;
        _spawner.PrintBonusCountUI(ActiveEnemies);
    }
}
