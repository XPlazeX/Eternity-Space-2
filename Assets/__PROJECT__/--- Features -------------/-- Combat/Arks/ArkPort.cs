using TMPro;
using UnityEngine;

public class ArkPort : MonoBehaviour
{
    [SerializeField] private bool isActive = true;
    [SerializeField] private TMP_Text statusLabel;
    [SerializeField] private bool showStatusLabel = true;

    private ArkPortStatus _status = ArkPortStatus.Inactive;
    public bool IsActive => isActive;

    private void Start() {
        if (statusLabel != null)
            statusLabel.gameObject.SetActive(showStatusLabel);

        if (isActive)
            SetStatus(ArkPortStatus.Active);
        else
            SetStatus(ArkPortStatus.Inactive);
    }

    public void SetStatus(ArkPortStatus status)
    {
        _status = status;

        if (statusLabel == null)
            return;

        switch (status)
        {
            case ArkPortStatus.Inactive:
                statusLabel.text = "НЕДОСТУПНО";
                break;
            case ArkPortStatus.Active:
                statusLabel.text = "ДОСТУПНО";
                break;
            case ArkPortStatus.CanConnecting:
                statusLabel.text = "ГОТОВ К ПОДКЛЮЧЕНИЮ";
                break;
            case ArkPortStatus.Connected:
                statusLabel.text = "ПОДКЛЮЧЕНИЕ";
                break;
            default:
                break;
        }
    }
}

public enum ArkPortStatus
{
    Inactive,
    Active,
    CanConnecting,
    Connected
}