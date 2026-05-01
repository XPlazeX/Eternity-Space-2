using System.Collections.Generic;
using UnityEngine;

public static class SpawnCardSelector
{
    /// <summary>
    /// Выбирает карту из пула, опираясь на входящие данные. Модифицирует пул в случае если карта подразумевает это.
    /// 1. Сначала проверяется валидность данных. Если данные невалидны - вернет null.
    /// 2. Далее отсекаются карты, не проходящие по budgetProgressRequirement.
    /// 3. Далее - выбор редкости. Если данной редкости нет, то она понижается на ступень. Если какая-то редкость была пропущена и карта не нашлась - проверяются другие редкости.
    /// 4. Далее - выбор стоимости, с учетом бюджета. Если данной стоимости нет, то она понижается на 1. Если не нашлась карта стоимостью 1 - берется наименьшая стоимость из возможных.
    /// 5. Если невозможно выбрать карту - вернёт null и failReason
    /// </summary>
    /// <param name="rareChance">Шанс редкой карты.</param>
    /// <param name="uncommonChance">Шанс необычной карты. Шанс обычной карты = 1 - (ШР + ШН)</param>
    /// <param name="currentBudget">Доступный бюджет стоимости карты</param>
    /// <param name="startBudget">Начальный бюджет Encounter</param>
    /// <param name="costWeights">Развесовка стоимостей. Для стоимости 4+ в выборку попадают все карты со стоимостью 4+</param>
    /// <param name="spawnCardsPool">Пул карт. Если выбранная карта exhaust - она будет удалена из списка</param>
    /// <param name="failReason">Причина возвращения null SpawnCard</param>
    /// <returns></returns>
    public static SpawnCard Select(
        float rareChance, float uncommonChance,
        int currentBudget, int startBudget,
        SpawnCardsCostWeights costWeights,
        ref SpawnCardsPool spawnCardsPool, out SpawnCardSelectFailReason failReason)
    {
        failReason = SpawnCardSelectFailReason.None;

        if (spawnCardsPool == null)
        {
            Debug.Log($"SpawnCardsPool is null!");
            failReason = SpawnCardSelectFailReason.NullReferenceContainment;
            return null;
        }

        if (spawnCardsPool.Count <= 0 || currentBudget <= 0 || startBudget <= 0)
        {
            Debug.Log($"Невалидные данные для выбора карты: spawnCardsPool.Count <= 0 ? ={spawnCardsPool.Count <= 0} || currentBudget <= 0 ? ={currentBudget <= 0} || startBudget <= 0 ? ={startBudget <= 0}");
            failReason = SpawnCardSelectFailReason.InvalidData;
            return null;
        }

        for (int i = 0; i < spawnCardsPool.Count; i++)
        {
            if (spawnCardsPool.spawnCards[i] == null)
            {
                Debug.Log($"Найдена null SpawnCard по индексу ={i}");
                failReason = SpawnCardSelectFailReason.NullReferenceContainment;
                return null;
            }
        }

        SpawnCardsPool availiablePool = new SpawnCardsPool();

        // отсекаем карты, которые слишком рано использовать
        float budgetProgress = Mathf.Clamp01(1f - ((float)currentBudget / startBudget));

        for (int i = 0; i < spawnCardsPool.Count; i++)
        {
            if (spawnCardsPool.spawnCards[i].budgetProgressRequirement <= budgetProgress)
            {
                availiablePool.spawnCards.Add(spawnCardsPool.spawnCards[i]);
            }
        }

        if (availiablePool.Count <= 0)
        {
            failReason = SpawnCardSelectFailReason.NoByBudgetProgress;
            return null;
        }

        SpawnCardsPool bufferPool = new SpawnCardsPool();

        // выбираем редкость
        float r = Random.value;

        SpawnCardRarity targetRarity = r <= rareChance ? SpawnCardRarity.Rare : (r <= uncommonChance + rareChance ? SpawnCardRarity.Uncommon : SpawnCardRarity.Common);
        SpawnCardRarity[] raritySequence = null;

        switch (targetRarity)
        {
            case SpawnCardRarity.Common:
                raritySequence = new SpawnCardRarity[3] {SpawnCardRarity.Common, SpawnCardRarity.Uncommon, SpawnCardRarity.Rare};
            break;
            case SpawnCardRarity.Uncommon:
                raritySequence = new SpawnCardRarity[3] {SpawnCardRarity.Uncommon, SpawnCardRarity.Common, SpawnCardRarity.Rare};
            break;
            default:
                raritySequence = new SpawnCardRarity[3] {SpawnCardRarity.Rare, SpawnCardRarity.Uncommon, SpawnCardRarity.Common};
            break;
        }

        for (int i = 0; i < raritySequence.Length; i++)
        {   
            bufferPool = new SpawnCardsPool();

            for (int j = 0; j < availiablePool.Count; j++)
            {
                if (availiablePool.spawnCards[j].rarity == raritySequence[i])
                {
                    bufferPool.spawnCards.Add(availiablePool.spawnCards[j]);
                }
            }
            
            if (bufferPool.Count > 0) break;
        }

        if (bufferPool.Count <= 0)
        {
            failReason = SpawnCardSelectFailReason.NoAnyRarity;
            return null;
        }

        availiablePool = bufferPool;
        bufferPool = new SpawnCardsPool();

        // выбираем цену
        int selectedCost = costWeights.GetCost();

        while (selectedCost > 0)
        {
            if (selectedCost <= currentBudget) break;

            selectedCost --;

            if (selectedCost == 0)
            {
                failReason = SpawnCardSelectFailReason.ZeroBudget;
                return null;
            }
        }

        while (selectedCost > 0)
        {
            for (int i = 0; i < availiablePool.Count; i++)
            {
                if (availiablePool.spawnCards[i].cost == selectedCost || (selectedCost == 4 && availiablePool.spawnCards[i].cost > 4))
                {
                    bufferPool.spawnCards.Add(availiablePool.spawnCards[i]);
                }
            }

            if (bufferPool.Count > 0)
            {
                availiablePool = bufferPool;
                break;
            }

            selectedCost --;

            if (selectedCost <= 0)
            {
                int minCost = 999;
                for (int i = 0; i < availiablePool.Count; i++)
                {
                    if (availiablePool.spawnCards[i].cost < minCost)
                    {
                        minCost = availiablePool.spawnCards[i].cost;
                        if (minCost == 1) break;
                    }
                }

                if (minCost > currentBudget)
                {
                    failReason = SpawnCardSelectFailReason.OnlyCostlyStayed;
                    return null;
                }

                for (int i = 0; i < availiablePool.Count; i++)
                {
                    if (availiablePool.spawnCards[i].cost == minCost)
                    {
                        bufferPool.spawnCards.Add(availiablePool.spawnCards[i]);
                    }
                }
                break;
            }
        }

        if (bufferPool.Count <= 0) // не должно быть возможно, чтобы такое случилось в этом месте
        {
            failReason = SpawnCardSelectFailReason.LogicError;
            return null;
        }

        availiablePool = bufferPool;

        SpawnCard selectedCard = availiablePool.spawnCards[Random.Range(0, availiablePool.Count)];

        if (selectedCard.exhaust)
        {
            spawnCardsPool.spawnCards.Remove(selectedCard);
        }

        return selectedCard;
    }

    
}

public enum SpawnCardSelectFailReason
{
    None,
    NullReferenceContainment,
    InvalidData,
    NoByBudgetProgress,
    NoAnyRarity,
    ZeroBudget,
    OnlyCostlyStayed,
    LogicError
}
