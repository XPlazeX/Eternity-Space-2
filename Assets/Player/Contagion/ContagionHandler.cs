using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContagionHandler : MonoBehaviour
{
    public const string contagion_collection_key = "Contagion";

    [SerializeField] private int _contagionLevel_RO;

    public static int ContagionLevel {get; private set;}

    public static void Initialize()
    {
        if (!GameSessionInfoHandler.ExistDataCollection(contagion_collection_key))
        {
            GameSessionInfoHandler.AddValueToCollection(contagion_collection_key, 0);
        }
        ContagionLevel = GameSessionInfoHandler.GetDataCollection(contagion_collection_key)[0];
    }
}
