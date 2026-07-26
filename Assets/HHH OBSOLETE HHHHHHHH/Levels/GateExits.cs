using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateExits : MonoBehaviour
{
    // [SerializeField] private int[] _exitsID;
    // //[SerializeField] private int _choicePower;

    // private void Start() {
    //     SceneStatics.SceneCore.GetComponent<GateHandler>().BusConnected += OnBusConnected;
    // }

    // private void OnBusConnected()
    // {
    //     if (!GameSessionInfoHandler.GetSessionSave().BeaconLevel)
    //     {
    //         SceneStatics.SceneCore.GetComponent<GateHandler>().SpawnBeaconGate();
    //         return;
    //     }

    //     List<int> choiceIndexes = GameSessionInfoHandler.GetDataCollection(GetGateLevelCode());
    //     int choicePower = ShipStats.GetIntValue("CurrentBusGateChoice");

    //     if (choiceIndexes.Count > 0)  
    //     {
    //         SceneStatics.SceneCore.GetComponent<GateHandler>().SpawnGates(choiceIndexes.ToArray());
    //         print("GettingFromGameSessionInfo");
    //     }

    //     else
    //     {
    //         List<int> aviableExits = new List<int>(_exitsID);

    //         for (int i = 0; i < choicePower; i++)
    //         {
    //             choiceIndexes.Add(aviableExits[Random.Range(0, aviableExits.Count)]);
    //             aviableExits.Remove(choiceIndexes[choiceIndexes.Count - 1]);
    //         }

    //         SceneStatics.SceneCore.GetComponent<GateHandler>().SpawnGates(choiceIndexes.ToArray());
    //         GameSessionInfoHandler.AddDataCollection(GetGateLevelCode(), choiceIndexes);
    //         print("SetFirstTimeToGameSessionInfo");
    //     }
    // }

    // private string GetGateLevelCode()
    // {
    //     return "Level" + GameSessionInfoHandler.GetSessionSave().CurrentLevel.ToString() + "Gates";
    // }
}
