using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelEnemyDatabase
{
    private const string bonusEnemyPath = "Enemies/Bonus/Bonus ";
    private const string towerPath = "Enemies/Towers/Tower ";

    [SerializeField] private LevelPack[] _levelPacks;
    [SerializeField] private int _weightMultiplier = 10;

    private List<EnemyPack> EnemyPacks;

    public LevelPack[] LevelPacks => _levelPacks;
    public int WieghtMultiplier => _weightMultiplier;

    public void Initialize()
    {
        EnemyPacks = new List<EnemyPack>();

        for (int i = 0; i < _levelPacks.Length; i++)
        {
            if (_levelPacks[i].NeededPercentage <= GameSessionInfoHandler.LevelProgress)
            {
                EnemyPacks.AddRange(_levelPacks[i].GetPacks);
            }
        }

        if (EnemyPacks.Count == 0)
        {
            EnemyPacks.AddRange(_levelPacks[0].GetPacks);
            Debug.Log("!!! аварийная загрузка паков врагов по умолчанию");
        }

        Debug.Log($"загружено паков: {EnemyPacks.Count}, уровень: {GameSessionInfoHandler.CurrentLevel} : {GameSessionInfoHandler.LevelProgress}");
    }

    public void AddEnemyPacks(EnemyPack[] ePacks)
    {
        EnemyPacks.AddRange(ePacks);
        Debug.Log($"> Добавлены пакеты Враг-Х-Вес | Дополнительно");
    }

    public EnemyPack GetRandomEnemyPacks(float weight, int maxAviableWeight, out int selectedWeight)
    {
        int trueWeight = (int)Mathf.Clamp(Mathf.CeilToInt(weight * _weightMultiplier), 1, maxAviableWeight); // выборщик веса всегда даёт значение в диапазоне 0-1
        selectedWeight = trueWeight;

        //print(weight.ToString() + " W | T " + trueWeight.ToString());

        List<EnemyPack> temp = new List<EnemyPack>();

        for (int i = 0; i < EnemyPacks.Count; i++)
        {
            if (EnemyPacks[i].Weight == trueWeight)
                temp.Add(EnemyPacks[i]);
        }

        if (temp.Count == 0)
        {
            return GetRandomEnemyPacks(weight - 1, maxAviableWeight, out selectedWeight);
        }

        return temp[Random.Range(0, temp.Count)];
    }

    [System.Serializable]
    public class LevelPack
    {
        [SerializeField][Range(0,1f)] private float _neededMissionProgress;
        [SerializeField] private EnemyPackObject _enemyPackObject;

        public float NeededPercentage => _neededMissionProgress;
        public EnemyPack[] GetPacks => _enemyPackObject.EnemyPacks;
    }
}

[System.Serializable]
public class EnemyPack
{
    [SerializeField][Range(1, 10)] private int _weight;
    [SerializeField] private int _count = 2;
    [SerializeField] private DamageBody _enemy;

    public int Weight => _weight;
    public int Count => _count;
    public DamageBody Enemy => _enemy;
}