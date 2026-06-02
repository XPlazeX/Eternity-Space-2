using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FabricatorDroneMonitor : MonoBehaviour
{
    [SerializeField] private TMP_Text stateLabel;
    [SerializeField] private Image stateFillLabel;
    [SerializeField] private GameObject commonSynthesisIndicator;
    [SerializeField] private GameObject rawSynthesisIndicator;
    [SerializeField] private TMP_Text synthesisProgressLabel;

    private PowerupFabricDrone _drone;

    private void Start() 
    {
        _drone = FindAnyObjectByType<PowerupFabricDrone>();
    }

    private void Update() 
    {
        if (_drone == null)
        {
            _drone = FindAnyObjectByType<PowerupFabricDrone>();
            return;
        }
        commonSynthesisIndicator.SetActive(!_drone.RawSynthesis);
        rawSynthesisIndicator.SetActive(_drone.RawSynthesis);
        stateFillLabel.fillAmount = _drone.DrillProgress01;
        synthesisProgressLabel.text = $"{Mathf.RoundToInt(_drone.BudgetProgress01 * 100f)}%";

        switch (_drone.Mode)
        {
            case PowerupFabricDrone.DroneMode.Parked:
                stateLabel.text = ParkingStateText;
                break;
            case PowerupFabricDrone.DroneMode.Working:
                if (_drone.Finding)
                    stateLabel.text = WaitStateText;
                else if (_drone.HasPowerup)
                    stateLabel.text = DeliveryStateText;
                else
                    stateLabel.text = WorkingStateText;
                break;
            case PowerupFabricDrone.DroneMode.Parking:
                stateLabel.text = DockingStateText;
                break;
            default:
                break;
        }
    }

    private string ParkingStateText => "ПАРКИНГ";
    private string WaitStateText => "ОЖИДАЕТ";
    private string WorkingStateText => "РАБОТАЕТ";
    private string DeliveryStateText => "ДОСТАВКА";
    private string DockingStateText => "СТЫКОВКА";
}
