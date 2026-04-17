using UnityEditor;
using UnityEngine;

// Encounter может использоваться в EncounterNode и передаваться EnemyDirector
[System.Serializable]
public class Encounter: ISerializationCallbackReceiver
{
    [Header("General")]
    [SerializeField] private string runtimeId;
    [SerializeField][Range(0f, 1f)] private float rareCardProbability = 0.05f;
    [SerializeField][Range(0f, 1f)] private float uncommonCardProbability = 0.2f;
    [Header("Planned")]
    [Tooltip("Задержка перед началом планового спауна.")]
    [SerializeField] private float plannedWaitTime = 3f;
    [Tooltip("Бюджет разыгрываемых карт, каждая карта тратит бюджет. Когда иссякнет - спаун прекращается.")]
    [SerializeField] private int plannedBudget = 30;
    [Tooltip("Веса вероятностей выборки стоимости карт. Обычно дешевые выпадают чаще, чем дорогие.")]
    [SerializeField] private SpawnCardsCostWeights plannedCostWeights;
    [Tooltip("Пул карт. Сначала выбирается стоимость, потом редкость, потом карта. Если такой стоимости нет, или бюджета недостаточно - берется стоимость на 1 меньше. Редкость аналогично - понижается, или берется любая доступная")]
    [SerializeField] private SpawnCardsPool spawnCardsPool;
    [Tooltip("Каждый враг имеет вес. Определяет максимальный суммарный вес врагов, при котором спаун останавливается.")]
    [SerializeField] private float plannedMaxWeight = 10f;
    [Tooltip("Определяет минимальный суммарный вес врагов, при котором спаун возобновляется.")]
    [SerializeField] private float plannedThresholdWeight = 1.5f;
    [Tooltip("Базовый кулдаун разыгрывания карты, начинает отсчёт после того, как все враги из карты заспаунены")]
    [SerializeField] private float plannedPlayCooldown = 0.5f;

    [Header("Bonus")]
    [Tooltip("Задержка перед началом бонусного спауна.")]
    [SerializeField] private float bonusWaitTime = 20f;
    [Tooltip("Веса вероятностей выборки стоимости карт. Обычно дешевые выпадают чаще, чем дорогие.")]
    [SerializeField] private SpawnCardsCostWeights bonusCostWeights;
    [Tooltip("Пул карт. Сначала выбирается стоимость, потом редкость, потом карта. Если такой стоимости нет, или бюджета недостаточно - берется стоимость на 1 меньше. Редкость аналогично - понижается, или берется любая доступная")]
    [SerializeField] private SpawnCardsPool bonusCardsPool;
    [Tooltip("Каждый враг имеет вес. Определяет максимальный суммарный вес врагов, при котором бонусный спаун останавливается.")]
    [SerializeField] private float bonusMaxWeight = 2f;
    [Tooltip("Определяет минимальный суммарный вес врагов, при котором бонусный спаун возобновляется.")]
    [SerializeField] private float bonusThresholdWeight = 0.5f;
    [Tooltip("Базовый кулдаун разыгрывания карты, начинает отсчёт после того, как все враги из карты заспаунены")]
    [SerializeField] private float bonusPlayCooldown = 10f;

    [Header("Final Cards")]
    [Tooltip("Использовать финальные карты? Будут разыгрываться, когда плановый бюджет иссякнет, а плановый вес достигнет threshold. Бонусный спаун не останавливается.")]
    [SerializeField] private bool playFinalCards = true;
    [Tooltip("Пул карт по порядку. Стоимость, редкость, тип не имеют значения, карты разыграются по порядку.")]
    [SerializeField] private SpawnCardsPool sortedFinalCardsPool;
    [Tooltip("Базовый кулдаун разыгрывания карты, начинает отсчёт после того, как все враги из карты заспаунены")]
    [SerializeField] private float finalPlayCooldown = 5f;

    public string RuntimeID => runtimeId;
    public float RareCardProbability { get => rareCardProbability; private set => rareCardProbability = value; }
    public float UncommonCardProbability { get => uncommonCardProbability; private set => uncommonCardProbability = value; }

    public float PlannedWaitTime { get => plannedWaitTime; private set => plannedWaitTime = value; }
    public int PlannedBudget { get => plannedBudget; private set => plannedBudget = value; }
    public SpawnCardsCostWeights PlannedCostWeights { get => plannedCostWeights; private set => plannedCostWeights = value; }
    public SpawnCardsPool PlannedCardsPool { get => spawnCardsPool; private set => spawnCardsPool = value; }
    public float PlannedMaxWeight { get => plannedMaxWeight; private set => plannedMaxWeight = value; }
    public float PlannedThresholdWeight { get => plannedThresholdWeight; private set => plannedThresholdWeight = value; }
    public float PlannedPlayCooldown { get => plannedPlayCooldown; private set => plannedPlayCooldown = value; }

    public float BonusWaitTime { get => bonusWaitTime; private set => bonusWaitTime = value; }
    public SpawnCardsCostWeights BonusCostWeights { get => bonusCostWeights; private set => bonusCostWeights = value; }
    public SpawnCardsPool BonusCardsPool { get => bonusCardsPool; private set => bonusCardsPool = value; }
    public float BonusMaxWeight { get => bonusMaxWeight; private set => bonusMaxWeight = value; }
    public float BonusThresholdWeight { get => bonusThresholdWeight; private set => bonusThresholdWeight = value; }
    public float BonusPlayCooldown { get => bonusPlayCooldown; private set => bonusPlayCooldown = value; }

    public bool PlayFinalCards { get => playFinalCards; private set => playFinalCards = value; }
    public SpawnCardsPool SortedFinalCardsPool { get => sortedFinalCardsPool; private set => sortedFinalCardsPool = value; }
    public float FinalPlayCooldown { get => finalPlayCooldown; private set => finalPlayCooldown = value; }

    public void OnBeforeSerialize()
    {
        if (string.IsNullOrEmpty(runtimeId))
        {
            runtimeId = GUID.Generate().ToString();
        }
    }

    public void OnAfterDeserialize() {}
}
