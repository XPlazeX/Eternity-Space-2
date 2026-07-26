using UnityEngine;

public class ArkDirector : MonoBehaviour
{
    public const string ARK_DIRECTOR_PAUSE_CODE = "ARK_DIRECTOR";

    public static event System.Action ArkEntered;
    public static event System.Action ArkExited;

    [SerializeField] private ArkConnector arkConnector;
    [SerializeField] private ArkCameraController arkCameraController;
    [SerializeField] private ArkPlayerController arkPlayerController;

    public static bool IsArked {get; private set;} = false;

    private PauseHandle _worldPauseHandle;
    // private PauseHandle _arkPauseHandle;
    private bool _isPlayerControlling = false;
    private float _waitTimer;

    private void OnEnable() {
        arkConnector.ArkConnected += OnArkConnected;
        arkConnector.ArkDisconnected += OnArkDisconnected;
    }

    void OnDisable()
    {
        arkConnector.ArkConnected -= OnArkConnected;
        arkConnector.ArkDisconnected -= OnArkDisconnected;
    }

    private void Start() {
        // _arkPauseHandle = ESTime.AcquirePause(new PauseRequest(TimeDomain.Ark, ARK_DIRECTOR_PAUSE_CODE));
        // _worldPauseHandle = null;
    }

    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (IsArked)
                arkConnector.TryConnect();
            else
                arkConnector.TryDisconnect();
        }
        #endif

        if (arkConnector.ConnectionAvailiable && !IsArked && PlayerInput.SelectionDown)
        {
            arkConnector.TryConnect();
        } else if (IsArked && PlayerInput.NextReleased)
        {
            arkConnector.TryDisconnect();
        }
    }


    public void EnterArk()
    {
        // _worldPauseHandle = ESTime.AcquirePause(new PauseRequest(TimeDomain.World, ARK_DIRECTOR_PAUSE_CODE));
        // ESTime.Release(_arkPauseHandle);
        // _arkPauseHandle = null;
        // arkEnvironmentVisualizator.Show();
        arkPlayerController.StartControlling(Vector3.zero);
        // arkCameraController.Activate();

        IsArked = true;
        // _waitTimer = arkEnvironmentVisualizator.AnimationTime;
        ArkEntered?.Invoke();
        
        // debug
        InteriorSoundController isc = FindAnyObjectByType<InteriorSoundController>();
        isc.GoInteriorOST();
    }

    public void ExitArk()
    {
        // _arkPauseHandle = ESTime.AcquirePause(new PauseRequest(TimeDomain.Ark, ARK_DIRECTOR_PAUSE_CODE));
        // ESTime.Release(_worldPauseHandle);
        // _worldPauseHandle = null;
        // arkEnvironmentVisualizator.Hide();
        arkPlayerController.StopControlling();
        // arkCameraController.Deactivate();

        IsArked = false;
        // _waitTimer = arkEnvironmentVisualizator.AnimationTime;
        ArkExited?.Invoke();
        
        // debug
        InteriorSoundController isc = FindAnyObjectByType<InteriorSoundController>();
        isc.ContinueMainOST();
    }

    private void OnArkConnected()
    {
        EnterArk();
    }

    private void OnArkDisconnected()
    {
        ExitArk();
    }
}
