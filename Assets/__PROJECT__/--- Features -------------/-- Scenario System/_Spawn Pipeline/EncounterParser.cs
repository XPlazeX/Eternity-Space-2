using System.Collections.Generic;
using UnityEngine;

public class EncounterParser
{
    public event System.Action<Encounter> EncounterCleared;
    public event System.Action<EnemyHandle> BossSpawned;
    public event System.Action<EnemyHandle> BossDied;
    public event System.Action<EnemyHandle> EnemySpawned;
    public event System.Action<EnemyHandle> EnemyDied;

    public Encounter EncounterData {get; private set;}

    public int LastBudget {get; private set;} = 0;
    public float BudgetProgress => _startBudget == 0 ? 0 : 1f - ((float)LastBudget / _startBudget);

    public List<EnemyHandle> PlannedEnemiesAlive {get; private set;} = new List<EnemyHandle>();
    public List<EnemyHandle> BonusEnemiesAlive {get; private set;} = new List<EnemyHandle>();
    public List<EnemyHandle> FinalEnemiesAlive {get; private set;} = new List<EnemyHandle>();
    public List<EnemyHandle> BossEnemiesAlive {get; private set;} = new List<EnemyHandle>();

    private SpawnCardsPool _plannedSpawnCardPool;
    private SpawnCardsPool _bonusSpawnCardPool;
    private SpawnCardsPool _finalSpawnCardPool;

    private SpawnCard _plannedPlayingSpawnCard;
    private SpawnCard _bonusPlayingSpawnCard;
    private SpawnCard _finalPlayingSpawnCard;

    private int _plannedCardProcessingRequest;
    private int _bonusCardProcessingRequest;
    private int _finalCardProcessingRequest;
    private int _finalCardPlayIndex;

    private int _startBudget = 0;

    private float _plannedCooldown = 0f;
    private float _bonusCooldown = 0f;
    private float _finalCooldown = 0f;

    private int _defeatedEnemies = 0;
    private int _defeatedBosses = 0;

    public int EnemiesAlive => PlannedEnemiesAlive.Count + BonusEnemiesAlive.Count + FinalEnemiesAlive.Count; // боссы входят в эти списки
    public int BossesAlive => BossEnemiesAlive.Count;
    public float TotalWeight => CalculatePlannedWeight() + CalculateBonusWeight() + CalculateFinalWeight();
    public int DefeatedEnemies => _defeatedEnemies;
    public int DefeatedBosses => _defeatedBosses;

    public bool IsPlannedSpawnPaused {get; private set;}
    public bool IsPlannedSpawnEnded {get; private set;}
    public bool IsBonusSpawnPaused {get; private set;}
    public bool IsBonusSpawnEnded {get; private set;}
    public bool IsFinalSpawnEnded {get; private set;}
    public bool IsEnded {get; private set;}

    private EnemySpawner _spawner;

    public EncounterParser(Encounter encounter, EnemySpawner enemySpawner)
    {
        Initialize(encounter, enemySpawner);
    }

    public void Initialize(Encounter encounter, EnemySpawner enemySpawner)
    {
        EncounterData = encounter;

        _startBudget = encounter.PlannedBudget;
        LastBudget = _startBudget;

        _plannedSpawnCardPool = new SpawnCardsPool(encounter.PlannedCardsPool);
        _bonusSpawnCardPool = new SpawnCardsPool(encounter.BonusCardsPool);
        _finalSpawnCardPool = new SpawnCardsPool(encounter.SortedFinalCardsPool);

        _plannedCooldown = encounter.PlannedWaitTime;
        _bonusCooldown = encounter.BonusWaitTime;
        _finalCooldown = 0f;

        _spawner = enemySpawner;
    }

    public void Tick(float dt)
    {
        if (IsEnded) return;

        if (IsPlannedSpawnEnded && IsBonusSpawnEnded && IsFinalSpawnEnded && BossEnemiesAlive.Count <= 0 && CalculatePlannedWeight() <= 0f && CalculateBonusWeight() <= 0f && CalculateFinalWeight() <= 0f)
        {
            IsEnded = true;
            EncounterCleared?.Invoke(EncounterData);
        }

        // Debug.Log($"Tick encounter: {EncounterData.RuntimeID}, planned spawn ended: {IsPlannedSpawnEnded}, bonus spawn ended: {IsBonusSpawnEnded}, final spawn ended: {IsFinalSpawnEnded}, bosses alive: {BossEnemiesAlive.Count}, planned alive: {PlannedEnemiesAlive.Count}, planned weight: {CalculatePlannedWeight()}, bonus alive: {BonusEnemiesAlive.Count}, bonus weight: {CalculateBonusWeight()}, final alive: {FinalEnemiesAlive.Count}, final weight: {CalculateFinalWeight()}");

        if (!IsPlannedSpawnEnded)
        {
            PlannedTick(dt);
        }
        else
        {
            if (!EncounterData.PlayFinalCards)
            {
                IsFinalSpawnEnded = true;
            }
            else if (!IsFinalSpawnEnded)
            {
                FinalTick(dt);
            }
        }

        if (IsPlannedSpawnEnded && IsFinalSpawnEnded && !IsBonusSpawnEnded && TotalWeight <= EncounterData.BonusStopWeightThreshold)
        {
            IsBonusSpawnEnded = true;
            Debug.Log($"Остановлен бонусный спаун, вес всех врагов на арене достиг порога для остановки: {TotalWeight} <= {EncounterData.BonusStopWeightThreshold}");
        }

        if (_bonusPlayingSpawnCard == null && ((IsPlannedSpawnEnded && IsFinalSpawnEnded) || IsBonusSpawnEnded))
            return;

        BonusTick(dt);
    }

    private void PlannedTick(float dt)
    {
        if (_plannedCooldown > 0f)
        {
            _plannedCooldown -= dt;
            return;
        }

        if (_plannedPlayingSpawnCard != null)
        {
            List<EnemyHandle> handles = _spawner.Spawn(_plannedPlayingSpawnCard.spawnRequests[_plannedCardProcessingRequest]);

            for (int i = 0; i < handles.Count; i++)
            {
                handles[i].DiedCallback += OnPlannedEnemyDie;
                if (handles[i].isBoss)
                {
                    handles[i].DiedCallback += OnBossEnemyDie;
                    BossEnemiesAlive.Add(handles[i]);
                    BossSpawned?.Invoke(handles[i]);
                }

                PlannedEnemiesAlive.Add(handles[i]);
                EnemySpawned?.Invoke(handles[i]);
            }

            _plannedCardProcessingRequest ++;

            if (_plannedCardProcessingRequest >= _plannedPlayingSpawnCard.spawnRequests.Count)
            {
                if (LastBudget <= 0)
                {
                    _plannedPlayingSpawnCard = null;
                    return;
                }
                if (CalculatePlannedWeight() >= EncounterData.PlannedMaxWeight)
                {
                    IsPlannedSpawnPaused = true;
                }

                _plannedCooldown = _plannedPlayingSpawnCard.cooldownAfterPlay + EncounterData.PlannedPlayCooldown;
                _plannedPlayingSpawnCard = null;
                return;
            }

            _plannedCooldown = _plannedPlayingSpawnCard.cooldownBetweenRequests;

            return;
        }

        float weight = CalculatePlannedWeight();

        if (weight <= EncounterData.PlannedThresholdWeight)
        {
            IsPlannedSpawnPaused = false;
            if (LastBudget <= 0)
            {
                IsPlannedSpawnEnded = true;
                Debug.Log($"Остановлен плановый спаун, закончился бюджет");
            }
        }

        if (IsPlannedSpawnPaused || weight >= EncounterData.PlannedMaxWeight || LastBudget <= 0)
        {
            return;
        }

        SpawnCardSelectFailReason selectFailReason = SpawnCardSelectFailReason.None;

        SpawnCard newPlannedSpawnCard = SpawnCardSelector.Select
        (
            EncounterData.RareCardProbability, EncounterData.UncommonCardProbability,
            LastBudget, _startBudget,
            EncounterData.PlannedCostWeights,
            ref _plannedSpawnCardPool, out selectFailReason
        );

        if (newPlannedSpawnCard == null)
        {
            IsPlannedSpawnEnded = true;
            Debug.Log($"Остановлен плановый спаун по причине: {selectFailReason}");
            return;
        }

        _plannedPlayingSpawnCard = newPlannedSpawnCard;
        LastBudget -= newPlannedSpawnCard.cost;
        _plannedCardProcessingRequest = 0;
    }

    private void OnPlannedEnemyDie(EnemyHandle handle)
    {
        PlannedEnemiesAlive.Remove(handle);
        _defeatedEnemies ++;
        EnemyDied?.Invoke(handle);
    }

    private void BonusTick(float dt)
    {
        if (EncounterData.BonusCardsPool == null || EncounterData.BonusCardsPool.Count == 0)
        {
            IsBonusSpawnEnded = true;
            Debug.Log($"Остановлен бонусный спаун, так как не задан пул карт или развесовка стоимости для бонусного спауна");
            return;
        }
        if (_bonusCooldown > 0f)
        {
            _bonusCooldown -= dt;
            return;
        }

        if (_bonusPlayingSpawnCard != null)
        {
            List<EnemyHandle> handles = _spawner.Spawn(_bonusPlayingSpawnCard.spawnRequests[_bonusCardProcessingRequest]);

            for (int i = 0; i < handles.Count; i++)
            {
                handles[i].DiedCallback += OnBonusEnemyDie;
                if (handles[i].isBoss)
                {
                    handles[i].DiedCallback += OnBossEnemyDie;
                    BossEnemiesAlive.Add(handles[i]);
                    BossSpawned?.Invoke(handles[i]);
                }

                BonusEnemiesAlive.Add(handles[i]);
                EnemySpawned?.Invoke(handles[i]);
            }

            _bonusCardProcessingRequest ++;

            if (_bonusCardProcessingRequest >= _bonusPlayingSpawnCard.spawnRequests.Count)
            {
                
                if (CalculateBonusWeight() >= EncounterData.BonusMaxWeight)
                {
                    IsBonusSpawnPaused = true;
                }

                _bonusCooldown = _bonusPlayingSpawnCard.cooldownAfterPlay + EncounterData.BonusPlayCooldown;
                _bonusPlayingSpawnCard = null;
                return;
            }

            _bonusCooldown = _bonusPlayingSpawnCard.cooldownBetweenRequests;

            return;
        }

        float weight = CalculateBonusWeight();

        if (weight <= EncounterData.BonusThresholdWeight)
        {
            IsBonusSpawnPaused = false;
        }

        if (IsBonusSpawnPaused || weight >= EncounterData.BonusMaxWeight)
        {
            return;
        }

        SpawnCardSelectFailReason selectFailReason = SpawnCardSelectFailReason.None;

        SpawnCard newBonusSpawnCard = SpawnCardSelector.Select
        (
            EncounterData.RareCardProbability, EncounterData.UncommonCardProbability,
            LastBudget + 1, _startBudget + 1,
            EncounterData.BonusCostWeights,
            ref _bonusSpawnCardPool, out selectFailReason
        );

        if (newBonusSpawnCard == null)
        {
            IsBonusSpawnEnded = true;
            Debug.Log($"Остановлен бонусный спаун по причине: {selectFailReason}");
            return;
        }

        _bonusPlayingSpawnCard = newBonusSpawnCard;
        _bonusCardProcessingRequest = 0;
    }

    private void OnBonusEnemyDie(EnemyHandle handle)
    {
        BonusEnemiesAlive.Remove(handle);
        _defeatedEnemies ++;
        EnemyDied?.Invoke(handle);
    }

    private void FinalTick(float dt)
    {
        if (_finalCooldown > 0f)
        {
            _finalCooldown -= dt;
            return;
        }

        if (_finalPlayingSpawnCard != null)
        {
            List<EnemyHandle> handles = _spawner.Spawn(_finalPlayingSpawnCard.spawnRequests[_finalCardProcessingRequest]);

            for (int i = 0; i < handles.Count; i++)
            {
                handles[i].DiedCallback += OnFinalEnemyDie;
                if (handles[i].isBoss)
                {
                    handles[i].DiedCallback += OnBossEnemyDie;
                    BossEnemiesAlive.Add(handles[i]);
                    BossSpawned?.Invoke(handles[i]);
                }

                FinalEnemiesAlive.Add(handles[i]);
                EnemySpawned?.Invoke(handles[i]);
            }

            _finalCardProcessingRequest ++;

            if (_finalCardProcessingRequest >= _finalPlayingSpawnCard.spawnRequests.Count)
            {
                _finalCardPlayIndex ++;

                if (_finalCardPlayIndex >= _finalSpawnCardPool.Count)
                {
                    IsFinalSpawnEnded = true;
                    return;
                }

                _finalCooldown = _finalPlayingSpawnCard.cooldownAfterPlay + EncounterData.FinalPlayCooldown;
                _finalPlayingSpawnCard = null;
                return;
            }

            _finalCooldown = _finalPlayingSpawnCard.cooldownBetweenRequests;

            return;
        }

        if (_finalCardPlayIndex >= _finalSpawnCardPool.Count)
        {
            IsFinalSpawnEnded = true;
            return;
        }

        _finalPlayingSpawnCard = _finalSpawnCardPool.spawnCards[_finalCardPlayIndex];
        _finalCardProcessingRequest = 0;
    }

    private void OnFinalEnemyDie(EnemyHandle handle)
    {
        FinalEnemiesAlive.Remove(handle);
        _defeatedEnemies ++;
        EnemyDied?.Invoke(handle);
    }

    private void OnBossEnemyDie(EnemyHandle handle)
    {
        BossEnemiesAlive.Remove(handle);
        _defeatedBosses ++;
        BossDied?.Invoke(handle);
    }
    
    private float CalculatePlannedWeight()
    {
        float w = 0f;

        for (int i = 0; i < PlannedEnemiesAlive.Count; i++)
        {
            w += PlannedEnemiesAlive[i].weight;
        }

        return w;
    }

    private float CalculateBonusWeight()
    {
        float w = 0f;

        for (int i = 0; i < BonusEnemiesAlive.Count; i++)
        {
            w += BonusEnemiesAlive[i].weight;
        }

        return w;
    }

    private float CalculateFinalWeight()
    {
        float w = 0f;

        for (int i = 0; i < FinalEnemiesAlive.Count; i++)
        {
            w += FinalEnemiesAlive[i].weight;
        }

        return w;
    }

    public EncounterSnapshot GenerateSnapshot()
    {
        return new EncounterSnapshot()
        {
            encounterId = EncounterData.RuntimeID,
            lastBudget = LastBudget,
            budgetProgress = BudgetProgress,
            startBudget = _startBudget,
            plannedEnemiesAlive = PlannedEnemiesAlive.Count,
            bonusEnemiesAlive = BonusEnemiesAlive.Count,
            finalEnemiesAlive = FinalEnemiesAlive.Count,
            bossesAlive = BonusEnemiesAlive.Count,
            defeatedEnemies = _defeatedEnemies,
            defeatedBosses = _defeatedBosses,
            isPlannedSpawnEnded = IsPlannedSpawnEnded,
            isPlannedSpawnPaused = IsPlannedSpawnPaused,
            isBonusSpawnEnded = IsBonusSpawnEnded,
            isBonusSpawnPaused = IsBonusSpawnPaused,
            isFinalSpawnEnded = IsFinalSpawnEnded,
            isEnded = IsEnded
        };
    }
}

public struct EncounterSnapshot
{
    public string encounterId;
    public int lastBudget;
    public float budgetProgress;
    public int startBudget;

    public int plannedEnemiesAlive;
    public int bonusEnemiesAlive;
    public int finalEnemiesAlive;
    public int bossesAlive;
    public int encounterEnemiesAlive => plannedEnemiesAlive + bonusEnemiesAlive + finalEnemiesAlive;

    public int defeatedEnemies;
    public int defeatedBosses;

    public bool isPlannedSpawnPaused;
    public bool isPlannedSpawnEnded;
    public bool isBonusSpawnPaused;
    public bool isBonusSpawnEnded;
    public bool isFinalSpawnEnded;
    public bool isEnded;
}