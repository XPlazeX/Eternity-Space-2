using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Eternity Space/Spawn Card")]
public class SpawnCard : ScriptableObject 
{
    #if UNITY_EDITOR
    public Sprite inspectorIcon;
    #endif

    [Range(1, 10)] public int cost = 1;
    public SpawnCardRarity rarity = SpawnCardRarity.Common;
    [Tooltip("Сколько % бюджета должно быть потрачено, чтобы карта могла быть разыграна")]
    [Range(0f, 1f)] public float budgetProgressRequirement = 0f;
    public bool exhaust = false;
    public float cooldownBetweenRequests = 0f;
    public float cooldownAfterPlay = 0f;
    public List<EnemyEncounterSpawnRequest> spawnRequests = new List<EnemyEncounterSpawnRequest>();
    [Space()]
    public List<SpawnCardTags> tags = new List<SpawnCardTags>();
}

[System.Serializable]
public class SpawnCardsPool
{
    public List<SpawnCard> spawnCards = new List<SpawnCard>();

    public int Count => spawnCards.Count;

    public SpawnCardsPool()
    {
        spawnCards = new List<SpawnCard>();
    }

    public SpawnCardsPool(SpawnCardsPool original)
    {
        spawnCards = new List<SpawnCard>();

        for (int i = 0; i < original.Count; i++)
        {
            spawnCards.Add(original.spawnCards[i]);
        }
    }
}

[System.Serializable]
public class EnemyEncounterSpawnRequest
{
    public DamageBody enemy;
    public float enemyWeight = 1f;
    public int count = 1;
    [Space()]
    public SpawnPositionMode spawnPositionMode;
    public Vector2 spawnPosition = Vector2.zero;
    public SpawnAreaMode spawnAreaMode = SpawnAreaMode.TopArc;
    [Tooltip("Если true - каждый спаун будет заново пересчитывать позицию")]
    public bool separateSpawn = false;
    public Vector2 randomSpawnOffsets = Vector2.zero;
    [Space()]
    public bool isBoss = false;
}

[System.Serializable]
public class SpawnCardsCostWeights
{
    [SerializeField][Range(0f, 1f)] private float cost_1 = 1f;
    [SerializeField][Range(0f, 1f)] private float cost_2 = 0.5f;
    [SerializeField][Range(0f, 1f)] private float cost_3 = 0.15f;
    [SerializeField][Range(0f, 1f)] private float cost_4andMore = 0.06f;

    public int GetCost()
    {
        float a = Random.Range(0f, cost_1 + cost_2 + cost_3 + cost_4andMore);

        if (a <= cost_4andMore) return 4;
        else if (a <= cost_4andMore + cost_3) return 3;
        else if (a <= cost_4andMore + cost_3 + cost_2) return 2;
        else return 1;
    }
}

[System.Serializable]
public class ExternalSpawnRequest
{
    public List<EnemyEncounterSpawnRequest> spawnRequests = new List<EnemyEncounterSpawnRequest>();
}

public enum SpawnAreaMode
{
    TopArc,
    BottomArc,
    LeftArc,
    RightArc,
    FullRing,
    FullLevel
}

public enum SpawnPositionMode
{
    OnlyUseSpawnAreaMode,
    ArenaLocalPosition,
    PlayerLocalPosition,
    WorldPosition
}

public enum SpawnCardRarity
{
    Common = 0,
    Uncommon = 1,
    Rare = 2
}

public enum SpawnCardTags
{
    Sniper
}
