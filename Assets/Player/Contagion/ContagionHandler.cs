using UnityEngine;
using System.Collections.Generic;

public class ContagionHandler : MonoBehaviour
{
    public const string contagion_collection_key = "Contagion";

    public delegate void contagionAction();
    public static event contagionAction ContagionChanged;
    public static event contagionAction ContagionAdded;

    public static int ContagionLevel {get; private set;}

    public static void Initialize()
    {  
        if (!GameSessionInfoHandler.ExistDataCollection(contagion_collection_key))
        {
            GameSessionInfoHandler.AddDataCollection(contagion_collection_key, new List<int>());
            GameSessionInfoHandler.AddValueToCollection(contagion_collection_key, 0);
            Debug.Log("<color=#6666FF>Инициализация заражения: 0</color>");
        }

        ContagionLevel = GameSessionInfoHandler.GetDataCollection(contagion_collection_key)[0];
        Debug.Log($"<color=#6666FF>Инициализирован уровень заражения: {ContagionLevel}</color>");

        ContagionChanged?.Invoke();
    }

    public static void AddContagion(int volume)
    {
        ContagionLevel += volume;
        SaveContagion();

        ContagionChanged?.Invoke();
        ContagionAdded?.Invoke();
    }

    public static void RemoveContagion(int volume)
    {
        ContagionLevel -= volume;

        if (ContagionLevel < 0)
        {
            ContagionLevel = 0;
        }

        SaveContagion();
        ContagionChanged?.Invoke();
    }

    public static void Clear()
    {
        GameSessionInfoHandler.RemoveDataCollection(contagion_collection_key);
        Debug.Log("<color=#6666FF>Заражение очищено</color>");
        ContagionChanged?.Invoke();
    }

    private static void SaveContagion()
    {
        if (!GameSessionInfoHandler.ExistDataCollection(contagion_collection_key))
        {
            Debug.Log("Коллекция заражения не существует!");
            return;
        }

        GameSessionInfoHandler.ReplaceValueInCollection(contagion_collection_key, 0, ContagionLevel);
        Debug.Log($"<color=#6666FF>Сохранён уровень заражения: {ContagionLevel}</color>");
    }
}
