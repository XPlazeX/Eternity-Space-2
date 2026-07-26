using UnityEngine;

public class SledgeTransitor : MonoBehaviour
{
    private const float MIN_TARGET_COORDS_DELTA = 10f;

    public const float TRANSITION_PROGRESS_WAIT_PLAYER = 0.01f;
    public const float TRANSITION_PROGRESS_CATCHING_PLAYER = 0.1f;
    public const float TRANSITION_PROGRESS_ROTATION_TO_DESTINATION = 0.2f;
    public const float TRANSITION_PROGRESS_PARSING_ACCELERATION = 0.3f;
    public const float TRANSITION_PROGRESS_PARSING_BREAKING = 0.8f;
    public const float TRANSITION_PROGRESS_NORMALIZING = 0.9f;

    public static event System.Action StartTransitionCycle;
    public static event System.Action Arrived;

    [Header("Init state")]
    [SerializeField] private Vector2 initStartPosition = Vector2.down * 100f;
    [SerializeField] private Vector2 initTargetPosition = Vector2.zero;

    [Header("Core")]
    [SerializeField] private Transform parsingTransform;
    [SerializeField] private Rigidbody2D parsingRb;

    [Header("Player catch")]
    [SerializeField] private Transform catchPivot;
    [SerializeField] private float catchRadius = 2f;
    [SerializeField] private float catchSpeed = 10f;
    [SerializeField] private float minDistanceToCatch = 0.001f;
    [SerializeField] private SledgeDockingStatuser dockingStatuser;
    [SerializeField] private Transform pointer;
    [SerializeField] private float pointerOffset = 4f;
    [SerializeField] private float pointerHideDistance = 6f;

    [Header("Parsing")]
    [SerializeField] private float rotationSpeed = 4f; // radians/sec
    [SerializeField] private AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private float minRotationDuration = 0.15f;

    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxParsingSpeed = 40f;
    [SerializeField] private float minApproachingDistance = 0.01f;
    [SerializeField] private bool skipNormalizing = true;

    [Header("Visual")]
    [SerializeField] private GameObject[] rotationThrusters;
    [SerializeField] private GameObject[] accelerationThrusters;
    [SerializeField] private TrailRenderer[] accelerationTrails;
    [SerializeField] private float accelerationTrailsFadeDuration = 1f;
    [SerializeField] private GameObject[] breakingThrusters;

    [Header("Player Camera")]
    [SerializeField] private float playerCameraFollowSpeed = 10f;
    [SerializeField] private Vector3 playerCameraFollowOffset = new Vector3(0f, 3f, 0f);

    public static float TransitionProggress01 { get; private set; }
    public bool IsTransitioning => _state != SledgeTransitorState.Free && _state != SledgeTransitorState.WaitPlayer;
    public bool IsParsing => _state == SledgeTransitorState.ParsingToTargetCoords || _state == SledgeTransitorState.RotateTotargetCoords || _state == SledgeTransitorState.NormalizeRotation;

    private SledgeTransitorState _state = SledgeTransitorState.Free;
    private ParsingMovePhase _parsingMovePhase = ParsingMovePhase.None;

    private Vector2 _targetCoordinates;

    private Vector2 _moveStartPosition;
    private Vector2 _moveDirection;
    private float _moveTotalDistance;
    private float _moveTravelledDistance;
    private float _moveSpeed;
    private float _trailFadeTimer;

    private float _rotationStartAngle;
    private float _rotationTargetAngle;
    private float _rotationTimer;
    private float _rotationDuration;

    private enum ParsingMovePhase
    {
        None,
        Accelerating,
        AccelerationTrailsFading,
        Braking
    }

    private void Reset()
    {
        parsingTransform = transform;
        parsingRb = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        if (parsingTransform == null)
            parsingTransform = transform;

        if (parsingRb == null)
            parsingRb = parsingTransform.GetComponent<Rigidbody2D>();

        if (parsingRb == null)
        {
            Debug.LogError($"{nameof(SledgeTransitor)} needs Rigidbody2D on parsingTransform.");
            enabled = false;
            return;
        }

        parsingRb.bodyType = RigidbodyType2D.Kinematic;
        parsingRb.gravityScale = 0f;
        parsingRb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void OnEnable()
    {
        if (Player.PlayerTransform != null)
        {
            Initialize();
        }
        else
        {
            Player.PlayerChanged += OnPlayerLoaded;
        }
    }

    private void OnDisable()
    {
        Player.PlayerChanged -= OnPlayerLoaded;
    }

    private void OnPlayerLoaded()
    {
        Initialize();
        Player.PlayerChanged -= OnPlayerLoaded;
    }

    private void Initialize()
    {
        TransitionProggress01 = 0f;

        if ((initStartPosition - initTargetPosition).magnitude < MIN_TARGET_COORDS_DELTA)
            return;

        TeleportSledge(initStartPosition);

        Player.PlayerTransform.position = catchPivot.position;
        Player.PlayerTransform.SetParent(catchPivot, true);

        PlayerController.CanControl = false;
        Player.CanAttack = false;

        BeginParsingToTarget(initTargetPosition, startWithBrakingPhase: true);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetTargetCoordinates(new Vector2(250f, 300f));
            StartAwaitTransition();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            SetTargetCoordinates(new Vector2(850f, -400f));
            StartAwaitTransition();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            SetTargetCoordinates(Vector2.zero);
            StartAwaitTransition();
        }
#endif
        if (_state == SledgeTransitorState.WaitPlayer)
        {
            WaitingPlayer();
        }
    }

    private void FixedUpdate()
    {
        float dt = ESTime.worldFixedDeltaTime;

        switch (_state)
        {
            case SledgeTransitorState.PlayerCatching:
                CatchingPlayer(dt);
                break;

            case SledgeTransitorState.RotateTotargetCoords:
                RotationToTargetCoords(dt);
                break;

            case SledgeTransitorState.ParsingToTargetCoords:
                ParsingToTargetCoords(dt);
                break;

            case SledgeTransitorState.NormalizeRotation:
                NormalizingRotation(dt);
                break;
        }
    }

    public void SetTargetCoordinates(Vector2 coords)
    {
        if ((parsingRb.position - coords).magnitude < MIN_TARGET_COORDS_DELTA)
        {
            Debug.LogError("Точка назначения слишком близко!");
            return;
        }

        _targetCoordinates = coords;
    }

    public void StartAwaitTransition()
    {
        if (_state != SledgeTransitorState.Free)
        {
            Debug.LogError("Парсер занят!");
            return;
        }

        _state = SledgeTransitorState.WaitPlayer;
        TransitionProggress01 = TRANSITION_PROGRESS_WAIT_PLAYER;
    }

    private void WaitingPlayer()
    {
        Vector3 fromPlayer = catchPivot.position - Player.PlayerTransform.position;

        if (pointer != null)
        {
            pointer.gameObject.SetActive(fromPlayer.magnitude >= pointerHideDistance);

            Vector3 directionFromPlayer = fromPlayer.normalized;
            pointer.position = Player.PlayerTransform.position + directionFromPlayer * pointerOffset;
            pointer.up = directionFromPlayer;
        }

        if (dockingStatuser.UpdateDockingStatus() != SledgeDockingStatus.Ready)
        {
            return;
        }

        if (fromPlayer.magnitude <= catchRadius)
        {
            _state = SledgeTransitorState.PlayerCatching;

            if (pointer != null)
                pointer.gameObject.SetActive(false);

            PlayerController.CanControl = false;
            Player.CanAttack = false;

            StartTransitionCycle?.Invoke();
            TransitionProggress01 = TRANSITION_PROGRESS_CATCHING_PLAYER;
        }
    }

    private void CatchingPlayer(float dt)
    {
        Vector3 targetPosition = catchPivot.position;

        Player.PlayerTransform.position = Vector3.MoveTowards(
            Player.PlayerTransform.position,
            targetPosition,
            catchSpeed * dt
        );

        if ((Player.PlayerTransform.position - targetPosition).magnitude > minDistanceToCatch)
            return;

        Player.PlayerTransform.position = targetPosition;
        Player.PlayerTransform.SetParent(catchPivot, true);

        _state = SledgeTransitorState.RotateTotargetCoords;

        Vector2 targetDirection = (_targetCoordinates - parsingRb.position).normalized;
        BeginSmoothRotation(targetDirection);

        SetObjectsActive(rotationThrusters, true);

        TransitionProggress01 = TRANSITION_PROGRESS_ROTATION_TO_DESTINATION;
    }

    private void RotationToTargetCoords(float dt)
    {
        if (!TickSmoothRotation(dt))
            return;

        SetObjectsActive(rotationThrusters, false);
        BeginParsingToTarget(_targetCoordinates, startWithBrakingPhase: false);
    }

    private void ParsingToTargetCoords(float dt)
    {
        if (_moveTotalDistance <= 0f)
        {
            FinishParsingToTarget();
            return;
        }

        switch (_parsingMovePhase)
        {
            case ParsingMovePhase.Accelerating:
                TickAcceleration(dt);
                break;

            case ParsingMovePhase.AccelerationTrailsFading:
                TickTrailFade(dt);
                break;

            case ParsingMovePhase.Braking:
                TickBraking(dt);
                break;

            default:
                FinishParsingToTarget();
                break;
        }
    }

    private void TickAcceleration(float dt)
    {
        if (ShouldStartAccelerationTrailFade())
        {
            BeginAccelerationTrailFade();
            MoveSledgeForward(_moveSpeed * dt);
            return;
        }

        float previousSpeed = _moveSpeed;

        _moveSpeed += GetSafeAcceleration() * dt;
        _moveSpeed = Mathf.Min(_moveSpeed, GetSafeMaxParsingSpeed());

        float moveDistance = (previousSpeed + _moveSpeed) * 0.5f * dt;

        if (MoveSledgeForward(moveDistance))
            return;

        if (ShouldStartAccelerationTrailFade())
        {
            BeginAccelerationTrailFade();
        }
    }

    private void TickTrailFade(float dt)
    {
        _moveSpeed = Mathf.Min(_moveSpeed, GetSafeMaxParsingSpeed());

        if (MoveSledgeForward(_moveSpeed * dt))
            return;

        _trailFadeTimer -= dt;

        if (_trailFadeTimer <= 0f)
        {
            BeginBraking();
        }
    }

    private void TickBraking(float dt)
    {
        float remainingDistance = GetRemainingDistance();

        if (remainingDistance <= minApproachingDistance)
        {
            FinishParsingToTarget();
            return;
        }

        float safeAcceleration = GetSafeAcceleration();

        float desiredSpeed = Mathf.Sqrt(2f * safeAcceleration * remainingDistance);
        desiredSpeed = Mathf.Min(desiredSpeed, GetSafeMaxParsingSpeed());

        float previousSpeed = _moveSpeed;

        if (_moveSpeed > desiredSpeed)
        {
            _moveSpeed = Mathf.Max(
                desiredSpeed,
                _moveSpeed - safeAcceleration * dt
            );
        }
        else
        {
            _moveSpeed = Mathf.MoveTowards(
                _moveSpeed,
                desiredSpeed,
                safeAcceleration * dt
            );
        }

        float moveDistance = (previousSpeed + _moveSpeed) * 0.5f * dt;

        if (MoveSledgeForward(moveDistance))
            return;

        if (_moveSpeed <= 0.001f && remainingDistance <= minApproachingDistance * 2f)
        {
            FinishParsingToTarget();
        }
    }

    private void NormalizingRotation(float dt)
    {
        if (!TickSmoothRotation(dt))
            return;

        parsingRb.MoveRotation(0f);

        SetObjectsActive(rotationThrusters, false);

        FinishTransitionCycle();
    }

    private void BeginParsingToTarget(Vector2 target, bool startWithBrakingPhase)
    {
        CameraController.instance.StartFollowing(
            catchPivot,
            playerCameraFollowSpeed,
            playerCameraFollowOffset,
            predication: 0f,
            lockFollow: true
        );

        _targetCoordinates = target;

        Vector2 currentPosition = parsingRb.position;
        Vector2 toTarget = _targetCoordinates - currentPosition;

        if (toTarget.magnitude < minApproachingDistance)
        {
            FinishParsingToTarget();
            return;
        }

        _moveStartPosition = currentPosition;
        _moveDirection = toTarget.normalized;
        _moveTotalDistance = toTarget.magnitude;
        _moveTravelledDistance = 0f;
        _trailFadeTimer = 0f;

        parsingRb.MoveRotation(AngleFromDirection(_moveDirection));

        _state = SledgeTransitorState.ParsingToTargetCoords;

        SetObjectsActive(rotationThrusters, false);

        if (startWithBrakingPhase)
        {
            _parsingMovePhase = ParsingMovePhase.Braking;

            float requiredBrakingSpeed = Mathf.Sqrt(2f * GetSafeAcceleration() * _moveTotalDistance);
            _moveSpeed = Mathf.Min(requiredBrakingSpeed, GetSafeMaxParsingSpeed());

            SetObjectsActive(accelerationThrusters, false);
            SetAccelerationTrailsActive(false, false, clear: true);
            SetObjectsActive(breakingThrusters, true);

            TransitionProggress01 = TRANSITION_PROGRESS_PARSING_BREAKING;
        }
        else
        {
            _parsingMovePhase = ParsingMovePhase.Accelerating;
            _moveSpeed = 0f;

            SetObjectsActive(accelerationThrusters, true);
            SetAccelerationTrailsActive(true, true, clear: true);
            SetObjectsActive(breakingThrusters, false);

            TransitionProggress01 = TRANSITION_PROGRESS_PARSING_ACCELERATION;
        }
    }

    private bool MoveSledgeForward(float distance)
    {
        if (distance <= 0f)
            return false;

        _moveTravelledDistance += distance;

        if (_moveTravelledDistance >= _moveTotalDistance - minApproachingDistance)
        {
            FinishParsingToTarget();
            return true;
        }

        Vector2 nextPosition = _moveStartPosition + _moveDirection * _moveTravelledDistance;
        parsingRb.MovePosition(nextPosition);

        return false;
    }

    private void FinishParsingToTarget()
    {
        TeleportSledge(_targetCoordinates);

        _moveSpeed = 0f;
        _trailFadeTimer = 0f;
        _moveTravelledDistance = _moveTotalDistance;
        _parsingMovePhase = ParsingMovePhase.None;

        SetObjectsActive(accelerationThrusters, false);
        SetAccelerationTrailsActive(false, false, clear: false);
        SetObjectsActive(breakingThrusters, false);

        if (skipNormalizing)
        {
            FinishTransitionCycle();
            return;
        }

        _state = SledgeTransitorState.NormalizeRotation;

        BeginSmoothRotation(Vector2.up);
        SetObjectsActive(rotationThrusters, true);

        TransitionProggress01 = TRANSITION_PROGRESS_NORMALIZING;
    }

    private void FinishTransitionCycle()
    {
        CameraController.instance.StartFollowing(
            Player.PlayerTransform,
            playerCameraFollowSpeed,
            playerCameraFollowOffset,
            predication: 0f,
            lockFollow: false
        );

        _state = SledgeTransitorState.Free;

        Player.PlayerTransform.SetParent(null, true);
        PlayerController.CanControl = true;
        Player.CanAttack = true;

        TransitionProggress01 = 1f;

        Debug.Log("ARRIVED");
        Arrived?.Invoke();
    }

    private void BeginSmoothRotation(Vector2 targetDirection)
    {
        if (targetDirection.sqrMagnitude <= 0.0001f)
            targetDirection = Vector2.up;

        _rotationStartAngle = parsingRb.rotation;
        _rotationTargetAngle = AngleFromDirection(targetDirection.normalized);

        float angleDeltaDeg = Mathf.Abs(Mathf.DeltaAngle(_rotationStartAngle, _rotationTargetAngle));
        float angleDeltaRad = angleDeltaDeg * Mathf.Deg2Rad;

        _rotationDuration = Mathf.Max(
            minRotationDuration,
            angleDeltaRad / GetSafeRotationSpeed()
        );

        _rotationTimer = 0f;
    }

    private bool TickSmoothRotation(float dt)
    {
        _rotationTimer += dt;

        float progress01 = Mathf.Clamp01(_rotationTimer / _rotationDuration);

        float curvedProgress = rotationCurve != null
            ? Mathf.Clamp01(rotationCurve.Evaluate(progress01))
            : progress01;

        float angle = Mathf.LerpAngle(
            _rotationStartAngle,
            _rotationTargetAngle,
            curvedProgress
        );

        parsingRb.MoveRotation(angle);

        if (progress01 < 1f)
            return false;

        parsingRb.MoveRotation(_rotationTargetAngle);
        return true;
    }

    private void BeginAccelerationTrailFade()
    {
        _parsingMovePhase = ParsingMovePhase.AccelerationTrailsFading;
        _trailFadeTimer = GetAccelerationTrailsFadeDuration();

        SetObjectsActive(accelerationThrusters, false);

        for (int i = 0; i < accelerationTrails.Length; i++)
        {
            if (accelerationTrails[i] == null)
                continue;

            accelerationTrails[i].emitting = false;
            accelerationTrails[i].gameObject.SetActive(true);
        }
    }

    private void BeginBraking()
    {
        _parsingMovePhase = ParsingMovePhase.Braking;

        SetAccelerationTrailsActive(false, false, clear: false);
        SetObjectsActive(breakingThrusters, true);

        TransitionProggress01 = TRANSITION_PROGRESS_PARSING_BREAKING;
    }

    private bool ShouldStartAccelerationTrailFade()
    {
        float remainingDistance = GetRemainingDistance();
        float safeAcceleration = GetSafeAcceleration();

        float limitedSpeed = Mathf.Min(_moveSpeed, GetSafeMaxParsingSpeed());

        float brakingDistance = (limitedSpeed * limitedSpeed) / (2f * safeAcceleration);
        float trailFadeDistance = limitedSpeed * GetAccelerationTrailsFadeDuration();

        return remainingDistance <= brakingDistance + trailFadeDistance + minApproachingDistance;
    }

    private float GetRemainingDistance()
    {
        return Mathf.Max(0f, _moveTotalDistance - _moveTravelledDistance);
    }

    private void TeleportSledge(Vector2 position)
    {
        parsingRb.position = position;
        parsingTransform.position = new Vector3(position.x, position.y, parsingTransform.position.z);
    }

    private static float AngleFromDirection(Vector2 direction)
    {
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
    }

    private float GetSafeAcceleration()
    {
        return Mathf.Max(0.001f, acceleration);
    }

    private float GetSafeMaxParsingSpeed()
    {
        return Mathf.Max(0.001f, maxParsingSpeed);
    }

    private float GetSafeRotationSpeed()
    {
        return Mathf.Max(0.001f, rotationSpeed);
    }

    private float GetAccelerationTrailsFadeDuration()
    {
        float result = accelerationTrailsFadeDuration;

        for (int i = 0; i < accelerationTrails.Length; i++)
        {
            if (accelerationTrails[i] == null)
                continue;

            result = Mathf.Max(result, accelerationTrails[i].time);
        }

        return result;
    }

    private void SetObjectsActive(GameObject[] objects, bool active)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] == null)
                continue;

            objects[i].SetActive(active);
        }
    }

    private void SetAccelerationTrailsActive(bool active, bool emitting, bool clear)
    {
        for (int i = 0; i < accelerationTrails.Length; i++)
        {
            if (accelerationTrails[i] == null)
                continue;

            accelerationTrails[i].gameObject.SetActive(active);
            accelerationTrails[i].emitting = emitting;

            if (clear)
                accelerationTrails[i].Clear();
        }
    }
}

public enum SledgeTransitorState
{
    Free,
    WaitPlayer,
    PlayerCatching,
    RotateTotargetCoords,
    ParsingToTargetCoords,
    NormalizeRotation
}