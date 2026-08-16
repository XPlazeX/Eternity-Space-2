using UnityEngine;

public class SledgeDirector : MonoBehaviour
{
    public static event System.Action PlayerCatchStarted;
    public static event System.Action PlayerDeployed;
    public static event System.Action PlayerDocked;
    public static event System.Action<bool> LockStateChanged;

    [SerializeField] private SledgeBody sledgeBody;
    [SerializeField] private SledgeDocker sledgeDocker;
    [SerializeField] private SledgeController sledgeController;
    [SerializeField] private SledgeJumpController sledgeJumpController;

    private PlayerSledgeState _playerSledgeState = PlayerSledgeState.SledgeControl;
    private bool _isPlayerControlling = true;
    private bool _isLocked = false;
    private bool _isPlayerUnvulnerableRequest = false;

    public static bool IsPlayerControlling => instance == null ? true : instance._isPlayerControlling;
    public bool IsSledgeControlling => _playerSledgeState == PlayerSledgeState.SledgeControl && !IsPlayerControlling && !IsLocked;
    public bool CanCatching => _playerSledgeState == PlayerSledgeState.PlayerFree && sledgeBody.CatcherAvailable && !IsLocked && !AutoDeployPlayer;
    public bool CanDeployPlayer => _playerSledgeState == PlayerSledgeState.SledgeControl && sledgeController.CanDeployPlayer;
    public bool AutoDeployPlayer => sledgeBody.IsAutoDeployState;
    public bool IsLocked => _isLocked;

    public static bool IsPlayerLosedControl {get; private set;} = false;
    public static Transform SledgeTransform => instance == null ? null : instance.sledgeBody.transform;
    public static Vector3 SledgePosition => SledgeTransform == null ? Vector3.zero : SledgeTransform.position;

    private static SledgeDirector instance;

    private void Awake() {
        instance = this;
    }

    private void OnEnable() 
    {
        sledgeDocker.CatchingStarted += OnPlayerCatchingStarted;
        sledgeDocker.PlayerDocked += OnPlayerDocked;

        if (Player.PlayerTransform != null)
        {
            Initialize();
        }
        else
        {
            Player.PlayerChanged += OnPlayerLoaded;
        }
    }

    private void OnPlayerLoaded()
    {
        Initialize();
        Player.PlayerChanged -= OnPlayerLoaded;
    }

    void OnDisable()
    {
        sledgeDocker.CatchingStarted -= OnPlayerCatchingStarted;
        sledgeDocker.PlayerDocked -= OnPlayerDocked;
        Player.PlayerChanged -= OnPlayerLoaded;
    }

    private void Initialize()
    {
        sledgeController.Initialize();

        sledgeDocker.FreeImmediately();
        PlayerController.CanControl = true;
        _isPlayerControlling = true;
        IsPlayerLosedControl = false;
        _playerSledgeState = PlayerSledgeState.PlayerFree;
    }

    public bool TryDeployPlayer() // вызывается из SledgeController, только при низкой скорости, когда игрок пристыкован
    {
        if (!CanDeployPlayer) return false;

        DeployPlayer();

        return true;
    }

    public bool TryJump()
    {
        return false;
    }

    private void OnPlayerCatchingStarted()
    {
        PlayerController.CanControl = false;
        _isPlayerControlling = false;
        IsPlayerLosedControl = true;

        if (!_isPlayerUnvulnerableRequest)
        {
            PlayerShipData.RequestUnvulnerability();
            _isPlayerUnvulnerableRequest = true;
        }

        PlayerCatchStarted?.Invoke();
    }

    private void OnPlayerDocked()
    {
        PlayerController.CanControl = false;
        _isPlayerControlling = false;
        IsPlayerLosedControl = true;
        StartSledgeControlling();

        PlayerDocked?.Invoke();
    }

    private void OnPlayerDeployed()
    {
        
    }

    /// <summary>
    /// Запускает контроль салазками, активирует системы салазок
    /// </summary>
    private void StartSledgeControlling()
    {
        _playerSledgeState = PlayerSledgeState.SledgeControl;
        _isPlayerControlling = false;
        sledgeController.StartControlling();
    }

    /// <summary>
    /// Деактивирует системы салазок, выключает контроль салазок, активирует системы игрока
    /// </summary>
    private void DeployPlayer()
    {
        sledgeDocker.DeployPlayer();
        PlayerController.CanControl = true;
        _isPlayerControlling = true;
        IsPlayerLosedControl = false;
        _playerSledgeState = PlayerSledgeState.PlayerFree;

        if (_isPlayerUnvulnerableRequest)
        {
            PlayerShipData.ReleaseUnvulnerability();
            _isPlayerUnvulnerableRequest = false;
        }

        PlayerDeployed?.Invoke();
    }

    /// <summary>
    /// Запуск анимации прыжка
    /// </summary>
    private void Jump()
    {
        
    }

    public static void Lock()
    {
        instance._isLocked = true;

        LockStateChanged?.Invoke(true);
    }

    public static void Unlock()
    {
        instance._isLocked = false;

        LockStateChanged?.Invoke(false);
    }
}

public enum PlayerSledgeState
{
    PlayerFree,
    SledgeControl,
    SledgeJumping
}
