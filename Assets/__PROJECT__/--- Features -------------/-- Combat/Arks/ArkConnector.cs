using UnityEngine;

public class ArkConnector : MonoBehaviour
{
    public event System.Action ArkConnected;
    public event System.Action ArkDisconnected;

    [SerializeField] private ArkEnvironmentVisualizator arkEnvironmentVisualizator;
    [SerializeField] private float connectDistance = 1f;
    [SerializeField] private float connectCheckFrequency = 0.3f;

    private ArkPort _availiablePort;
    private ArkPort _connectingPort;
    private float _checkTimer;

    public bool ConnectionAvailiable => _availiablePort != null;

    private void OnEnable() {
        arkEnvironmentVisualizator.EnterAnimationFinished += OnEnterAnimationFinished;
        arkEnvironmentVisualizator.ExitAnimationFinished += OnExitAnimationFinished;
    }

    void OnDisable()
    {
        arkEnvironmentVisualizator.EnterAnimationFinished -= OnEnterAnimationFinished;
        arkEnvironmentVisualizator.ExitAnimationFinished -= OnExitAnimationFinished;
    }

    private void Update() {
        _checkTimer -= ESTime.unscaledDeltaTime;
        if (_checkTimer <= 0f)
        {
            _checkTimer = connectCheckFrequency;

            if (_connectingPort == null)
                CheckConnection();
        }
    }

    private void CheckConnection()
    {
        ArkPort[] ports = FindObjectsByType<ArkPort>(FindObjectsSortMode.None);

        ArkPort candidatePort = null;

        float minDistance = Mathf.Infinity;

        foreach (var port in ports)
        {
            if (port == _connectingPort || !port.IsActive)
                continue;

            float distance = Vector3.Distance(port.transform.position, Player.PlayerTransform.position);
            if (distance <= connectDistance && distance < minDistance)
            {
                candidatePort = port;
                minDistance = distance;
            }
        }

        if (candidatePort == null && _availiablePort != null)
        {
            _availiablePort.SetStatus(ArkPortStatus.Active);
        } 
        else if (_availiablePort != candidatePort)
        {
            if (_availiablePort != null)
                _availiablePort.SetStatus(ArkPortStatus.Active);

            candidatePort.SetStatus(ArkPortStatus.CanConnecting);
        }
        _availiablePort = candidatePort;
    }

    public bool TryConnect()
    {
        if (_availiablePort == null)
            return false;

        _connectingPort = _availiablePort;
        _connectingPort.SetStatus(ArkPortStatus.Connected);

        arkEnvironmentVisualizator.EnterAnimation();
        return true;
    }

    public bool TryDisconnect()
    {
        if (_connectingPort == null)
            return false;

        arkEnvironmentVisualizator.ExitAnimation();
        return true;
    }

    private void OnEnterAnimationFinished()
    {
        ArkConnected?.Invoke();
    }
    private void OnExitAnimationFinished()
    {
        _connectingPort.SetStatus(_connectingPort.IsActive ? ArkPortStatus.Active : ArkPortStatus.Inactive);
        _connectingPort = null;
        ArkDisconnected?.Invoke();
    }
}
