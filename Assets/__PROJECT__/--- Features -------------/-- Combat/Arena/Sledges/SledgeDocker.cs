using UnityEngine;

public class SledgeDocker : MonoBehaviour
{
    private const float PLAYER_CAMERA_SCALE = 11f;
    private const float SLEDGE_CAMERA_SCALE = 17.2f;

    public event System.Action CatchingStarted;
    public event System.Action PlayerDocked;

    [SerializeField] private SledgeDirector sledgeDirector;
    [SerializeField] private Transform catchingPivot;
    [SerializeField] private GameObject leavingZoneObject;
    [SerializeField] private CircleLineRenderer leavingCircleRenderer;
    [SerializeField] private float leavingRadius = 5f;
    [SerializeField] private GameObject catchingZoneObject;
    [SerializeField] private CircleLineRenderer catchingCircleRenderer;
    [SerializeField] private float catchingRadius = 2f;
    [SerializeField] private float catchSpeed = 1f;
    [SerializeField] private float catchThreshold = 0.01f;
    [SerializeField] private SledgeDockingStatuser dockingStatuser;
    [Header("Player Camera")]
    [SerializeField] private float playerCameraFollowSpeed = 10f;
    [SerializeField] private Vector3 playerCameraFollowOffset = new Vector3(0f, 3f, 0f);
    [SerializeField] private Vector3 sledgeCameraFollowOffset = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private float catchCameraSpeed = 2f;
    [SerializeField] private float cameraScaleTime = 2f;

    private DockingState _dockingState;

    private void OnEnable() {
        SledgeDirector.LockStateChanged += OnLockStateChanged;
    }

    void OnDisable()
    {
        SledgeDirector.LockStateChanged -= OnLockStateChanged;
    }

    private void Start() {
        leavingCircleRenderer.SetRadius(leavingRadius);
        catchingCircleRenderer.SetRadius(catchingRadius);
        leavingZoneObject.SetActive(false);
        catchingZoneObject.SetActive(false);
    }

    void Update()
    {
        Vector3 playerPosition = Player.Position;

        switch (_dockingState)
        {
            case DockingState.Docked:
                return;

            case DockingState.FreeLeaveZone:
                if ((catchingPivot.position - playerPosition).magnitude > leavingRadius)
                {
                    PlayerLeaved();
                }
            break;

            case DockingState.FreeWaitCatch:
                if (dockingStatuser.UpdateDockingStatus() != SledgeDockingStatus.Ready)
                    {
                        return;
                    }
                if ((catchingPivot.position - playerPosition).magnitude < catchingRadius && sledgeDirector.CanCatching)
                {
                    StartPlayerCatch();
                }
            break;

            case DockingState.Catching:
                Catching(ESTime.worldDeltaTime);
            break;

            default:
                break;
        }
    }

    public void DeployPlayer() // вызывается из директора
    {
        _dockingState = DockingState.FreeLeaveZone;
        leavingZoneObject.SetActive(true);
        Player.PlayerTransform.SetParent(null);

        CameraController.instance.StartFollowing(
            Player.PlayerTransform,
            playerCameraFollowSpeed,
            playerCameraFollowOffset,
            predication: 0f,
            lockFollow: false
        );

        CameraController.instance.StartScaling(PLAYER_CAMERA_SCALE, cameraScaleTime);
    }

    private void PlayerLeaved()
    {
        _dockingState = DockingState.FreeWaitCatch;
        leavingZoneObject.SetActive(false);
        catchingZoneObject.SetActive(sledgeDirector.CanCatching);
    }

    private void StartPlayerCatch()
    {
        _dockingState = DockingState.Catching;
        catchingZoneObject.SetActive(false);

        CameraController.instance.StartFollowing(
            catchingPivot,
            catchCameraSpeed,
            sledgeCameraFollowOffset,
            predication: 0f,
            lockFollow: false
        );

        CatchingStarted?.Invoke();
    }

    private void Catching(float dt)
    {
        Player.PlayerTransform.position = SceneStatics.FlatVector(Vector3.MoveTowards(
            Player.Position,
            catchingPivot.position,
            catchSpeed * dt
        ));

        if ((Player.Position - catchingPivot.position).magnitude <= catchThreshold)
        {
            Player.PlayerTransform.position = catchingPivot.position;
            Dock();
        }
    }

    public void FreeImmediately()
    {
        DeployPlayer();
    }

    public void DockImmediately()
    {
        Player.PlayerTransform.position = catchingPivot.position;
        Dock();
    }

    private void Dock()
    {
        _dockingState = DockingState.Docked;
        Player.PlayerTransform.SetParent(catchingPivot);

        CameraController.instance.StartFollowing(
            catchingPivot,
            playerCameraFollowSpeed,
            sledgeCameraFollowOffset,
            predication: 0f,
            lockFollow: true
        );

        CameraController.instance.StartScaling(SLEDGE_CAMERA_SCALE, cameraScaleTime);

        PlayerDocked?.Invoke();
    }

    private void OnLockStateChanged(bool isLocked)
    {
        if (_dockingState == DockingState.FreeWaitCatch)
        {
            catchingZoneObject.SetActive(!isLocked);
        }
    }

    public enum DockingState
    {
        Docked,
        FreeLeaveZone,
        FreeWaitCatch,
        Catching
    }
}
