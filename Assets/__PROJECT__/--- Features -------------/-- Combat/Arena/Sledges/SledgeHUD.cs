using TMPro;
using UnityEngine;

public class SledgeHUD : MonoBehaviour
{
    [SerializeField] private SledgeController sledgeController;
    [SerializeField] private GameObject breakingGroup;
    [SerializeField] private GameObject thrustForwardGroup;
    [SerializeField] private GameObject thrustBackingGroup;
    [SerializeField] private TMP_Text speedLabel;
    [SerializeField] private TMP_Text angleSpeedLabel;
    [SerializeField] private Color canDeploySpeedColor;
    [SerializeField] private Color cannotDeploySpeedColor;
    [SerializeField] private TMP_Text xCoordLabel;
    [SerializeField] private TMP_Text yCoordLabel;
    [SerializeField] private GameObject canDeployObject;

    private void Update()
    {
        if (sledgeController == null)
            return;

        SetActive(thrustForwardGroup, sledgeController.UsingForwardThrust);
        SetActive(thrustBackingGroup, sledgeController.UsingBackingThrust);
        SetActive(breakingGroup, sledgeController.UsingBreaking);
        SetActive(canDeployObject, sledgeController.CanDeployPlayer);

        if (speedLabel != null)
        {
            speedLabel.text = $"{Mathf.RoundToInt(sledgeController.Speed * Meter.SCALE)} м/с";
            speedLabel.color = sledgeController.CanDeployBySpeed 
                ? canDeploySpeedColor 
                : cannotDeploySpeedColor;
        }

        if (angleSpeedLabel != null)
        {
            angleSpeedLabel.text = $"{Mathf.RoundToInt(sledgeController.AngularSpeed)} гр/с";
            angleSpeedLabel.color = sledgeController.CanDeployByAngularSpeed 
                ? canDeploySpeedColor 
                : cannotDeploySpeedColor;
        }

        Vector3 position = Map.CurrentSector == null ? 
            sledgeController.transform.position : 
            Map.CurrentSector.GetLocalCoordinates(sledgeController.transform.position);

        if (xCoordLabel != null)
            xCoordLabel.text = (position.x * Meter.SCALE).ToString("F1");

        if (yCoordLabel != null)
            yCoordLabel.text = (position.y * Meter.SCALE).ToString("F1");
    }

    private void SetActive(GameObject targetObject, bool active)
    {
        if (targetObject != null && targetObject.activeSelf != active)
            targetObject.SetActive(active);
    }
}