using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GateHandler : MonoBehaviour
{
    // const float borderOffset = 3f;
    // public delegate void gateAction();
    // public event gateAction BusConnected;

    // [SerializeField] private Button _endButton;
    // [SerializeField] private GateLocation[] _gates;

    // private Bus _activeBus;
    // private int _preparingLevelID = 0;

    // public void BusConnect(Bus bus)
    // {
    //     _activeBus = bus;
    //     BusConnected?.Invoke();
    // }

    // public void PrepareTransition(int transitionID = 0)
    // {
    //     _preparingLevelID = transitionID;
    //     _endButton.gameObject.SetActive(true);
    // }

    // public void StopTransition()
    // {
    //     _endButton.gameObject.SetActive(false);
    // }

    // public void SpawnGates(int[] IDarray)
    // {
    //     for (int i = 0; i < IDarray.Length; i++)
    //     {
    //         SpawnGate(_gates[IDarray[i]].LevelGate);
    //     }
    // }

    // public void SpawnBeaconGate()
    // {
    //     SpawnGate(Resources.Load<Gate>("Beacon/BeaconGate"));
    // }

    // public void SpawnGate(Gate gate)
    // {
    //     Quaternion borders = CameraController.Borders_xXyY;

    //     Gate spawnedGate = Instantiate(gate, new Vector3(
    //         Random.Range(borders.x + borderOffset, borders.y - borderOffset),
    //         Random.Range(borders.z + borderOffset, borders.w - borderOffset)), Quaternion.identity);
    // }

    // public void Transit()
    // {
    //     _activeBus.StartEngine();
    //     _endButton.gameObject.SetActive(false);

    //     GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
    //     save.PrepareToNewLevel();
    //     save.CurrentLevel ++;
    //     save.LocationID = _preparingLevelID;
    //     //save.LocationGearLevel = 1 + _gates[_preparingLevelID].GearLevelBonus;
    //     GameSessionInfoHandler.RewriteSessionSave(save);
    //     GameSessionInfoHandler.SaveAll();
    // }

    // [System.Serializable]
    // private class GateLocation
    // {
    //     [SerializeField] private Gate _gate;
    //     [SerializeField] private int _gearLevelBonus = 0;

    //     public Gate LevelGate => _gate;
    //     public int GearLevelBonus => _gearLevelBonus;
    // }
}
