using UnityEngine;

public class TestEnemyCardPlayer : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private bool spawn = true;
    [SerializeField] private SpawnCard[] spawnCards;
    [SerializeField] private EnemyEncounterSpawnRequest[] enemyEncounterSpawnRequests;

    private void Start() {
        if (spawn) Spawn();
    }

    private void Spawn()
    {
        for (int i = 0; i < spawnCards.Length; i++)
        {
            for (int s = 0; s < spawnCards[i].spawnRequests.Count; s++)
            {
                enemySpawner.Spawn(spawnCards[i].spawnRequests[s]);
            }
        }

        for (int i = 0; i < enemyEncounterSpawnRequests.Length; i++)
        {
            enemySpawner.Spawn(enemyEncounterSpawnRequests[i]);
        }
    }
}
