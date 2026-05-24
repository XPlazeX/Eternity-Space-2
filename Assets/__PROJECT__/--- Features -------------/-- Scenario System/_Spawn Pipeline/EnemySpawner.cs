using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static System.Action<DamageBody> DamageBodySpawned;

    [SerializeField] private EnemyHealthBar enemyHealthBar;
    [SerializeField] private Vector2 arenaSpawnOutsideOffsetMinMax = new Vector2(20f, 30f);

    private static EnemySpawner instance;

    private void Awake() {
        instance = this;
    }

    public List<EnemyHandle> Spawn(EnemyEncounterSpawnRequest enemyEncounterSpawnRequest)
    {
        List<EnemyHandle> spawnedHandles = new List<EnemyHandle>();

        Vector3 spawnPosition = GetSpawnPosition(enemyEncounterSpawnRequest);
        for (int i = 0; i < enemyEncounterSpawnRequest.count; i++)
        {
            DamageBody enemy = SpawnDamageBody(enemyEncounterSpawnRequest.enemy, spawnPosition + enemyEncounterSpawnRequest.SpawnOffset, enemyEncounterSpawnRequest.elitize);
            EnemyHandle handle = enemy.gameObject.AddComponent<EnemyHandle>();

            handle.weight = enemyEncounterSpawnRequest.enemyWeight;
            handle.isBoss = enemyEncounterSpawnRequest.isBoss;
            handle.isElite = enemyEncounterSpawnRequest.elitize;

            handle.Bind(enemy);

            spawnedHandles.Add(handle);

            if (enemyEncounterSpawnRequest.separateSpawn)
            {
                spawnPosition = GetSpawnPosition(enemyEncounterSpawnRequest);
            }
        }

        return new List<EnemyHandle>();
    }

    public DamageBody SpawnDamageBody(DamageBody dbSample, Vector3 spawnPosition, bool eliteSpawn = false)
    {
        if (dbSample == null)
        {
            Debug.Log("Empty damageBody!");
            return null;
        }

        DamageBody db = Instantiate(dbSample, spawnPosition, Quaternion.Euler(0, 0, 180f)).GetComponent<DamageBody>();

        if (db.GetComponent<Boss>() == null)
        {
            InitializeHPBar(db, eliteSpawn);
        }

        DamageBodySpawned?.Invoke(db);

        return db;
    }

    public static DamageBody Spawn(DamageBody dbSample, Vector3 spawnPosition, bool eliteSpawn = false)
    {
        if (instance == null)
        {
            Debug.Log("EnemySpawner instance is null!");
            return null;
        }
        return instance.SpawnDamageBody(dbSample, spawnPosition, eliteSpawn);
    }

    private Vector3 GetSpawnPosition(EnemyEncounterSpawnRequest enemyEncounterSpawnRequest)
    {
        Vector3 spawnPosition = enemyEncounterSpawnRequest.spawnPosition;

        if (enemyEncounterSpawnRequest.spawnPositionMode == SpawnPositionMode.OnlyUseSpawnAreaMode)
        {
            switch (enemyEncounterSpawnRequest.spawnAreaMode)
            {
                case SpawnAreaMode.FullLevel:
                    throw new System.Exception("Пока не реализован full level, нужен ArenaWorld");
                case SpawnAreaMode.FullRing:
                    spawnPosition = ArenaLocal.Center 
                        + (Quaternion.Euler(0f, 0f, Random.Range(-180f, 180f)) * Vector3.up) 
                        * (ArenaLocal.VisibleRadius + Random.Range(arenaSpawnOutsideOffsetMinMax.x, arenaSpawnOutsideOffsetMinMax.y));
                    break;
                case SpawnAreaMode.TopArc:
                    spawnPosition = ArenaLocal.Center
                        + (Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)) * Vector3.up)
                        * (ArenaLocal.VisibleRadius + Random.Range(arenaSpawnOutsideOffsetMinMax.x, arenaSpawnOutsideOffsetMinMax.y));
                    break;
                case SpawnAreaMode.BottomArc:
                    spawnPosition = ArenaLocal.Center
                        + (Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)) * Vector3.down)
                        * (ArenaLocal.VisibleRadius + Random.Range(arenaSpawnOutsideOffsetMinMax.x, arenaSpawnOutsideOffsetMinMax.y));
                    break;
                case SpawnAreaMode.RightArc:
                    spawnPosition = ArenaLocal.Center
                        + (Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)) * Vector3.right)
                        * (ArenaLocal.VisibleRadius + Random.Range(arenaSpawnOutsideOffsetMinMax.x, arenaSpawnOutsideOffsetMinMax.y));
                    break;
                case SpawnAreaMode.LeftArc:
                    spawnPosition = ArenaLocal.Center
                        + (Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)) * Vector3.left)
                        * (ArenaLocal.VisibleRadius + Random.Range(arenaSpawnOutsideOffsetMinMax.x, arenaSpawnOutsideOffsetMinMax.y));
                    break;
                default:
                    break;
            }
        } else switch (enemyEncounterSpawnRequest.spawnPositionMode)
        {
            case SpawnPositionMode.ArenaLocalPosition:
                spawnPosition += ArenaLocal.Center;
                break;
            case SpawnPositionMode.PlayerLocalPosition:
                spawnPosition += Player.Position;
                break;
            case SpawnPositionMode.WorldPosition:
            default:
                break;
        }

        return spawnPosition;
    }

    public void InitializeHPBar(DamageBody targetBody, bool elite = false)
    {
        if (targetBody.GetComponent<Boss>() != null)
            return;

        EnemyHealthBar hpBar = Pool.Spawn(enemyHealthBar);
        hpBar.transform.SetParent(targetBody.transform);

        Vector3 offsetBar = Vector3.up;
        if (targetBody.GetComponent<BoxCollider2D>() != null)
        {
            offsetBar = Vector3.up * (targetBody.GetComponent<BoxCollider2D>().size.y + 0.1f);
        }

        hpBar.transform.position = targetBody.transform.position + offsetBar;
        targetBody.DamageTaking += hpBar.SetHP;
        targetBody.HealthModified += hpBar.OnHealthModified;

        hpBar.Initialize(targetBody.HitPoints, elite);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ArenaLocal.Center, ArenaLocal.VisibleRadius + arenaSpawnOutsideOffsetMinMax.x);
    }
}
