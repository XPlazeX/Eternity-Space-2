using System.Collections;
using UnityEngine;

public class BossMission : SpawningMission
{
    [Header("Boss spawning")]
    [SerializeField] private BossSpawnObject[] _bossesPerLevels = new BossSpawnObject[1];
    [SerializeField] private float _waitTime;
    [SerializeField] private WeightSelector _preLevelWS;

    private int _bossesAlive;
    private int _bossesMax;
    private Spawner _spawner;

    public override void StartPlay()
    {
        _preLevelWS.Initialize();
        SceneTransition.SceneOpened += StartOnSceneLoaded;
        _spawner = SceneStatics.SceneCore.GetComponent<Spawner>();
        _spawner.PrintProgressUI(SceneLocalizator.GetLocalizedString("Game", 0, 1));
        UpdateUIData();

        base.StartPlay();
    }

    private IEnumerator Spawning()
    {
        yield return new WaitForSeconds(_waitTime);

        SpawnBosses(_bossesPerLevels[GameSessionInfoHandler.GetSessionSave().CurrentLevel]);
    }

    private void StartOnSceneLoaded()
    {
        StartCoroutine(Spawning());
    }

     private void OnDisable() {
        SceneTransition.SceneOpened -= StartOnSceneLoaded;
    }

    private void SpawnBosses(BossSpawnObject bso)
    {
        print("СТАРТ БОСС СПАУНА");
        for (int i = 0; i < bso.bosses.Length; i++)
        {
            Boss boss = (Spawner.SpawnDamageBody(bso.bosses[i].gameObject.GetComponent<DamageBody>(), bso.bosses[i].spawnPosition)).GetComponent<Boss>();

            if (boss == null)
                continue;

            _bossesAlive ++;
            boss.GetComponent<DamageBody>().Deathed += UnregisterBoss;
        }

        _bossesMax = _bossesAlive;
        UpdateUIData();

        print("КОНЕЦ БОСС СПАУНА");
    }

    public void UnregisterBoss()
    {
        _bossesAlive --;
        UpdateUIData();

        if (_bossesAlive <= 0)
        {
            TriggerVictory();
        }
    }

    private void UpdateUIData()
    {
        _spawner.PrintCountUI(_bossesAlive);
    }

    [System.Serializable]
    private struct BossSpawnObject 
    {
        public SpawnObject[] bosses;
    }
}
