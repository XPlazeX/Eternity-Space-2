using UnityEngine;

public class EnemyHQ : MonoBehaviour
{
    public event System.Action HQCalmed;
    public event System.Action<float> HQWarningUpdated;
    public event System.Action HQAlarmed;
    public event System.Action FarDoomActivated;

    [SerializeField] private FarDoomSystem farDoomSystem;
    [SerializeField] private float maxWarningCapacity = 100f;
    [SerializeField] private float calmingSpeed = 10f;

    private float _warningLevel;
    private EnemyHQState _state;
    private bool _farDoomActivated = false;
    private float _timeSinceAlarm = 0f;

    public float WarningLevel => _warningLevel;
    public float WarningLevel01 => Mathf.Clamp01(_warningLevel / maxWarningCapacity);
    public EnemyHQState State => _state;
    public bool IsFarDoomActivated => _farDoomActivated;
    public float TimeSinceAlarm => _timeSinceAlarm;

    private void Start() {
        SetCalm();
    }

    void Update()
    {
        switch (_state)
        {
            case EnemyHQState.Calm:

            break;
            case EnemyHQState.WarningFill:
                UpdateWarning(ESTime.worldDeltaTime);
            break;
            case EnemyHQState.Alarm:
                _timeSinceAlarm += ESTime.worldDeltaTime;
            break;
            default:
                break;
        }

        if (!_farDoomActivated && Map.CurrentSector != null)
        {
            if (Map.CurrentSector.PlayerInSectorSafetyStatus == PlayerInSectorSafetyStatus.ImpendingDoom)
            {
                ActivateFarDoom();
            }
        }
    }

    private void SetCalm()
    {
        if (_state == EnemyHQState.Alarm) return;

        _state = EnemyHQState.Calm;
        _warningLevel = 0f;

        HQCalmed?.Invoke();
    }

    private void UpdateWarning(float dt)
    {
        if (_state == EnemyHQState.Alarm) return;

        _warningLevel -= calmingSpeed * ESTime.worldDeltaTime;

        HQWarningUpdated?.Invoke(_warningLevel);

        if (_warningLevel <= 0f)
        {
            SetCalm();
        }
    }

    public void AddWarning(float w)
    {
        if (_state == EnemyHQState.Alarm) return;

        _state = EnemyHQState.WarningFill;
        _warningLevel += w;

        HQWarningUpdated?.Invoke(_warningLevel);

        if (_warningLevel >= maxWarningCapacity)
        {
            Alarm();
        }
    }

    private void Alarm()
    {
        _state = EnemyHQState.Alarm;

        HQAlarmed?.Invoke();
    }

    private void ActivateFarDoom()
    {
        if (_farDoomActivated) return;

        // farDoomSystem...

        _farDoomActivated = true;

        FarDoomActivated?.Invoke();
    }
}

public enum EnemyHQState
{
    Calm,
    WarningFill,
    Alarm
}