using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PowerupFabricDrone : MonoBehaviour
{
    public enum DroneMode
    {
        Working,
        Parking,
        Parked
    }

    [Header("Fabric")]
    [SerializeField] private float wreckPriceToPowerup = 100f;
    [SerializeField] private float drillSpeed = 1f;
    [SerializeField] private GameObject[] fabricatingObjects;
    [SerializeField] private GameObject rawFabricatingObject;
    [SerializeField] private Transform powerupPivot;
    [SerializeField] private float maxFindingRadius = 100f;
    [SerializeField] private float findingUpdateTime = 1f;

    [Header("Engine")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float acceleration = 4f;

    [Tooltip("Градусов в секунду. Чем меньше, тем шире дуга.")]
    [SerializeField] private float turnSpeedDegrees = 90f;

    [SerializeField] private float drag = 0.5f;
    [SerializeField] private float arrivalSlowDistance = 2.5f;
    [SerializeField] private float minVelocityToStop = 0.01f;

    [Header("Player follow")]
    [SerializeField] private float playerFollowDistance = 3f;
    [SerializeField] private Vector2 playerFollowOffset = new Vector2(0f, -2f);

    [Header("Parking")]
    [SerializeField] private float parkingArriveTolerance = 0.05f;

    [Header("Wreck approach")]
    [SerializeField] private float targetDistance = 1.5f;
    [SerializeField] private float wreckArriveTolerance = 0.15f;

    [Header("Hover near fixed drill point")]
    [SerializeField] private float hoverAmplitude = 0.2f;
    [SerializeField] private float hoverFrequency = 1.5f;
    [SerializeField] private float hoverCorrectionAcceleration = 6f;

    [Header("Visual")]
    [SerializeField] private float visualRotationSpeedDegrees = 360f;
    [SerializeField] private Transform movementTransform;
    [SerializeField] private Transform turretTransform;
    [SerializeField] private LineRenderer drillBeam;
    [SerializeField] private Transform beamPivot;
    [SerializeField] private ExplosionObject fabricatingExplosion;
    [SerializeField] private float scale = 1f;
    [SerializeField] private bool overrideColor = false;
    [SerializeField] private Color color = Color.wheat;
    [SerializeField] private GameObject[] disabledOnParkedObjects = new GameObject[0];

    [Tooltip("Объект на конце луча бурения: искры, маленькая вспышка, сверло и т.п.")]
    [SerializeField] private GameObject drillObject;

    [SerializeField] private bool rotateDrillObjectAlongBeam = true;

    [Header("Integrated Parallax")]
    [SerializeField] private bool useIntegratedParallax = false;

    [Tooltip("Ближе - Основной слой - Дальше")]
    [Range(-1f, 1f)]
    [SerializeField] private float parallaxSpeed = 0f;

    [SerializeField] private bool notChangeScale = false;

    private Rigidbody2D _rb;
    private Vector2 _inertia;
    private DroneMode _mode = DroneMode.Working;
    private Transform _parkingTransform;

    private Wreck _targetWreck;

    private float _currentBudget = 0f;
    private bool _hasPowerup = false;
    private bool _rawFabricateRequested = false;
    private bool _deliveringRawFabricatedObject = false;
    private float _findingTimer;

    private GameObject _fabricatedObject;

    private float _hoverTimer;
    private float _hoverSeed;

    private bool _hasDrillAnchor;
    private Vector2 _drillAnchorDirectionFromWreck;

    private Transform _cameraTransform;
    private Vector3 _lastCameraPosition;
    private Vector3 _defaultScale;
    private float _cameraScale = 1f;

    // Parallax больше не применяется напрямую к transform.
    // Он копится здесь и применяется только через _rb.MovePosition.
    private Vector2 _pendingParallaxDelta;
    private Vector2 _fixedStepParallaxDelta;

    private bool _motionAppliedThisFixedStep;

    public DroneMode Mode => _mode;
    public bool HasPowerup => _hasPowerup;
    public bool Finding => (_mode == DroneMode.Working) && _targetWreck == null && !_hasPowerup;
    public bool RawSynthesis => _rawFabricateRequested;
    public float BudgetProgress01 => Mathf.Clamp01(_currentBudget / wreckPriceToPowerup);
    public float DrillProgress01 => _targetWreck != null ? (1f - _targetWreck.DrilledAmount01) : 0f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (_rb != null)
        {
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        _hoverSeed = Random.Range(0f, 1000f);

        SetupParallax();
        ClearDrillVisuals();
    }

    private void OnEnable()
    {
        if (useIntegratedParallax)
            CameraController.Moved += OnCameraMoved;
    }

    private void OnDisable()
    {
        if (useIntegratedParallax)
            CameraController.Moved -= OnCameraMoved;
    }

    private void FixedUpdate()
    {
        float dt = ESTime.worldFixedDeltaTime;

        if (_mode == DroneMode.Parked)
            return;

        BeginFixedStep();

        if (_mode == DroneMode.Parking)
        {
            TickParking(dt);
            EndFixedStep();
            RotateTowardsPoint(_parkingTransform.position, dt);
            return;
        }

        _findingTimer -= dt;

        if (_hasPowerup && _fabricatedObject == null)
        {
            _hasPowerup = false;

            if (_deliveringRawFabricatedObject)
            {
                _deliveringRawFabricatedObject = false;
                ParkAt(GetPlayerParkingTransform());
                RotateTowardsPoint(_parkingTransform.position, dt);
                EndFixedStep();
                return;
            }
        }

        if (_targetWreck == null && _findingTimer <= 0f && !_hasPowerup)
        {
            FindTargetWreck();
            _findingTimer = findingUpdateTime;
        }

        if (_targetWreck == null || _hasPowerup)
        {
            ClearDrillVisuals();
            ResetDrillAnchor();
            FollowPlayer(dt);
            RotateTowardsPoint(movementTransform.position + movementTransform.up * 3f, dt);
            EndFixedStep();
            return;
        }

        EnsureDrillAnchor();

        Vector2 anchor = GetCurrentDrillAnchorWorld();
        float distanceToAnchor = Vector2.Distance(GetSteeringPosition(), anchor);

        if (distanceToAnchor > wreckArriveTolerance)
        {
            ClearDrillVisuals();
            FollowWreckAnchor(dt);
            RotateTowardsPoint(movementTransform.position + movementTransform.up * 3f, dt);
            EndFixedStep();
            return;
        }

        HoverNearDrillAnchor(dt);
        RotateTowardsPoint(_targetWreck.transform.position, dt);
        Fabricating(dt);

        EndFixedStep();
    }

    private void BeginFixedStep()
    {
        _fixedStepParallaxDelta = _pendingParallaxDelta;
        _pendingParallaxDelta = Vector2.zero;
        _motionAppliedThisFixedStep = false;
    }

    public void StartWorking()
    {
        if (_mode == DroneMode.Working)
            return;

        transform.SetParent(null, true);
        SyncRigidbodyWithTransform();

        _mode = DroneMode.Working;
        _pendingParallaxDelta = Vector2.zero;
        _fixedStepParallaxDelta = Vector2.zero;

        foreach (GameObject obj in disabledOnParkedObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

    public void ParkAt(Transform parkingTransform, bool attachImmediately = false)
    {
        if (parkingTransform == null)
            return;

        _parkingTransform = parkingTransform;

        StopCurrentProcess();

        if (attachImmediately)
        {
            AttachToParkingTransform();
            return;
        }

        transform.SetParent(null, true);
        SyncRigidbodyWithTransform();
        _mode = DroneMode.Parking;
    }

    public bool RequestRawFabricate()
    {
        if (rawFabricatingObject == null)
            return false;

        if (powerupPivot == null)
            return false;

        _rawFabricateRequested = true;

        if (_mode == DroneMode.Parked)
            StartWorking();

        return true;
    }

    public bool RequestRawFabricate(GameObject prefab)
    {
        if (prefab == null)
            return false;

        rawFabricatingObject = prefab;
        return RequestRawFabricate();
    }

    private void TickParking(float dt)
    {
        if (_parkingTransform == null)
        {
            _mode = DroneMode.Working;
            return;
        }

        Vector2 targetPoint = _parkingTransform.position;
        float distanceToParking = Vector2.Distance(GetSteeringPosition(), targetPoint);

        if (distanceToParking <= parkingArriveTolerance)
        {
            AttachToParkingTransform();
            return;
        }

        MoveToPoint(
            targetPoint,
            0f,
            dt,
            rotateToMovement: true
        );
    }

    private void StopCurrentProcess()
    {
        _targetWreck = null;
        _rawFabricateRequested = false;
        _deliveringRawFabricatedObject = false;
        _findingTimer = findingUpdateTime;

        ClearDrillVisuals();
        ResetDrillAnchor();

        _inertia = Vector2.zero;
        _pendingParallaxDelta = Vector2.zero;
        _fixedStepParallaxDelta = Vector2.zero;
        _motionAppliedThisFixedStep = false;
    }

    private void AttachToParkingTransform()
    {
        if (_parkingTransform == null)
            return;

        StopCurrentProcess();

        _mode = DroneMode.Parked;

        transform.SetParent(_parkingTransform, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        foreach (GameObject obj in disabledOnParkedObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        SyncRigidbodyWithTransform();
    }

    private void SyncRigidbodyWithTransform()
    {
        if (_rb == null)
            return;

        _rb.position = transform.position;
        _rb.rotation = transform.eulerAngles.z;
    }

    private void EndFixedStep()
    {
        // На случай, если в каком-то состоянии не было gameplay movement,
        // но камера сдвинулась. Параллакс всё равно должен примениться через MovePosition.
        if (!_motionAppliedThisFixedStep && _fixedStepParallaxDelta.sqrMagnitude > 0f)
        {
            ApplyMotion(Vector2.zero);
        }

        _fixedStepParallaxDelta = Vector2.zero;
    }

    private Vector2 GetSteeringPosition()
    {
        // Для расчётов считаем, что parallax уже будет применён в этом FixedUpdate.
        // Это убирает лишнее подруливание к целям, которые уже сдвинулись параллаксом.
        return _rb.position + _fixedStepParallaxDelta;
    }

    private void ApplyMotion(Vector2 gameplayDelta)
    {
        Vector2 finalDelta = gameplayDelta + _fixedStepParallaxDelta;

        _fixedStepParallaxDelta = Vector2.zero;
        _motionAppliedThisFixedStep = true;

        if (finalDelta.sqrMagnitude <= 0f)
            return;

        _rb.MovePosition(_rb.position + finalDelta);
    }

    private void FollowPlayer(float dt)
    {
        if (Player.PlayerTransform == null)
        {
            ApplyDragOnly(dt);
            return;
        }

        Vector2 targetPoint = (Vector2)Player.PlayerTransform.position + playerFollowOffset;

        MoveToPoint(
            targetPoint,
            playerFollowDistance,
            dt,
            rotateToMovement: true
        );
    }

    private void FollowWreckAnchor(float dt)
    {
        if (_targetWreck == null)
        {
            ApplyDragOnly(dt);
            return;
        }

        Vector2 anchor = GetCurrentDrillAnchorWorld();

        MoveToPoint(
            anchor,
            0f,
            dt,
            rotateToMovement: true
        );
    }

    private void HoverNearDrillAnchor(float dt)
    {
        if (_targetWreck == null)
            return;

        _hoverTimer += dt;

        Vector2 anchor = GetCurrentDrillAnchorWorld();

        Vector2 radial = _drillAnchorDirectionFromWreck;

        if (radial.sqrMagnitude < 0.0001f)
            radial = -GetMovementUp();

        radial.Normalize();

        Vector2 tangent = new Vector2(-radial.y, radial.x);

        float waveA = Mathf.Sin((_hoverTimer + _hoverSeed) * hoverFrequency);
        float waveB = Mathf.Sin((_hoverTimer + _hoverSeed) * hoverFrequency * 0.73f + 1.7f);

        Vector2 hoverOffset =
            tangent * (waveA * hoverAmplitude) +
            radial * (waveB * hoverAmplitude * 0.25f);

        Vector2 desiredPoint = anchor + hoverOffset;
        Vector2 toDesired = desiredPoint - GetSteeringPosition();

        Vector2 desiredVelocity = toDesired / Mathf.Max(dt, 0.0001f);
        desiredVelocity = Vector2.ClampMagnitude(desiredVelocity, speed * 0.35f);

        _inertia = Vector2.MoveTowards(
            _inertia,
            desiredVelocity,
            hoverCorrectionAcceleration * dt
        );

        if (_inertia.sqrMagnitude <= minVelocityToStop * minVelocityToStop)
            _inertia = Vector2.zero;

        ApplyMotion(_inertia * dt);
    }

    private void MoveToPoint(Vector2 targetPoint, float stopDistance, float dt, bool rotateToMovement)
    {
        Vector2 currentPosition = GetSteeringPosition();

        Vector2 toTarget = targetPoint - currentPosition;
        float distance = toTarget.magnitude;

        if (distance <= stopDistance)
        {
            ApplyDragOnly(dt);
            return;
        }

        Vector2 desiredDirection = toTarget / distance;

        Vector2 currentDirection;

        if (_inertia.sqrMagnitude > 0.0001f)
            currentDirection = _inertia.normalized;
        else
            currentDirection = GetMovementUp();

        float maxRadiansDelta = turnSpeedDegrees * Mathf.Deg2Rad * dt;

        Vector2 turnedDirection = Vector3.RotateTowards(
            currentDirection,
            desiredDirection,
            maxRadiansDelta,
            0f
        );

        float currentSpeed = _inertia.magnitude;

        float distanceAfterStopZone = Mathf.Max(0f, distance - stopDistance);
        float speedMultiplier = Mathf.Clamp01(distanceAfterStopZone / Mathf.Max(0.01f, arrivalSlowDistance));

        float targetSpeed = speed * speedMultiplier;

        float newSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            acceleration * dt
        );

        _inertia = turnedDirection.normalized * newSpeed;

        if (_inertia.sqrMagnitude <= minVelocityToStop * minVelocityToStop)
            _inertia = Vector2.zero;

        ApplyMotion(_inertia * dt);

        if (rotateToMovement && _inertia.sqrMagnitude > 0.0001f)
            RotateMovementTowardsDirection(_inertia.normalized);
    }

    private void ApplyDragOnly(float dt)
    {
        _inertia = Vector2.MoveTowards(
            _inertia,
            Vector2.zero,
            drag * dt
        );

        if (_inertia.sqrMagnitude <= minVelocityToStop * minVelocityToStop)
        {
            _inertia = Vector2.zero;
            ApplyMotion(Vector2.zero);
            return;
        }

        ApplyMotion(_inertia * dt);

        if (_inertia.sqrMagnitude > 0.0001f)
            RotateMovementTowardsDirection(_inertia.normalized);
    }

    private void Fabricating(float dt)
    {
        if (_targetWreck == null)
        {
            ClearDrillVisuals();
            return;
        }

        Vector3 start = beamPivot != null ? beamPivot.position : transform.position;
        Vector3 end = _targetWreck.transform.position;

        SetDrillVisuals(start, end);

        float price = _targetWreck.Price;

        if (_targetWreck.IsDrilled(drillSpeed * dt))
        {
            ClearDrillVisuals();

            _currentBudget += price;
            _targetWreck = null;
            ResetDrillAnchor();

            if (_rawFabricateRequested)
            {
                FabricateRawObject();
                return;
            }

            if (_currentBudget >= wreckPriceToPowerup)
                FabricatePowerup();
        }
    }

    private void SetDrillVisuals(Vector3 start, Vector3 end)
    {
        if (drillBeam != null)
        {
            drillBeam.positionCount = 2;
            drillBeam.SetPosition(0, start);
            drillBeam.SetPosition(1, end);
        }

        if (drillObject != null)
        {
            if (!drillObject.activeSelf)
                drillObject.SetActive(true);

            drillObject.transform.position = end;

            if (rotateDrillObjectAlongBeam)
            {
                Vector3 direction = end - start;

                if (direction.sqrMagnitude > 0.0001f)
                    drillObject.transform.up = direction.normalized;
            }
        }
    }

    private void ClearDrillVisuals()
    {
        if (drillBeam != null)
            drillBeam.positionCount = 0;

        if (drillObject != null && drillObject.activeSelf)
            drillObject.SetActive(false);
    }

    private void FabricatePowerup()
    {
        if (fabricatingObjects == null || fabricatingObjects.Length == 0)
            return;

        GameObject prefab = fabricatingObjects[Random.Range(0, fabricatingObjects.Length)];

        TryFabricate(prefab, consumeBudget: true);
    }

    private void FabricateRawObject()
    {
        if (TryFabricate(rawFabricatingObject, consumeBudget: true))
        {
            _rawFabricateRequested = false;
            _deliveringRawFabricatedObject = true;
        }
    }

    private bool TryFabricate(GameObject prefab, bool consumeBudget)
    {
        if (prefab == null)
            return false;

        if (powerupPivot == null)
            return false;

        if (_hasPowerup && _fabricatedObject != null)
            return false;

        if (consumeBudget)
            _currentBudget -= wreckPriceToPowerup;

        _hasPowerup = true;

        _fabricatedObject = Instantiate(
            prefab,
            powerupPivot.position,
            Quaternion.identity
        );

        _fabricatedObject.transform.SetParent(powerupPivot, true);

        ExplosionObject explosion = Pool.Spawn(fabricatingExplosion, powerupPivot.position, transform.rotation);
        explosion.SetScale(scale);
        if (overrideColor)
            explosion.SetColor(color);

        return true;
    }

    private void FindTargetWreck()
    {
        Wreck[] candidates = FindObjectsByType<Wreck>(FindObjectsSortMode.None);

        if (candidates.Length == 0)
            return;

        Wreck bestCandidate = null;
        Vector2 currentPosition = GetSteeringPosition();

        for (int i = 0; i < candidates.Length; i++)
        {
            Wreck candidate = candidates[i];

            if (candidate == null)
                continue;

            if (((Vector2)(candidate.transform.position) - currentPosition).sqrMagnitude > maxFindingRadius * maxFindingRadius)
                continue;

            if (bestCandidate == null)
            {
                bestCandidate = candidate;
                continue;
            }

            if (candidate.Price > bestCandidate.Price)
            {
                bestCandidate = candidate;
                continue;
            }

            if (Mathf.Approximately(candidate.Price, bestCandidate.Price))
            {
                float candidateDistance = Vector2.SqrMagnitude((Vector2)candidate.transform.position - currentPosition);
                float bestDistance = Vector2.SqrMagnitude((Vector2)bestCandidate.transform.position - currentPosition);

                if (candidateDistance < bestDistance)
                    bestCandidate = candidate;
            }
        }

        if (bestCandidate != null)
            SetTargetWreck(bestCandidate);
    }

    private void SetTargetWreck(Wreck wreck)
    {
        _targetWreck = wreck;
        ResetDrillAnchor();

        if (_targetWreck != null)
            EnsureDrillAnchor();
    }

    private void EnsureDrillAnchor()
    {
        if (_hasDrillAnchor || _targetWreck == null)
            return;

        Vector2 wreckPosition = _targetWreck.transform.position;
        Vector2 fromWreckToDrone = GetSteeringPosition() - wreckPosition;

        if (fromWreckToDrone.sqrMagnitude < 0.0001f)
            fromWreckToDrone = -GetMovementUp();

        _drillAnchorDirectionFromWreck = fromWreckToDrone.normalized;
        _hasDrillAnchor = true;
    }

    private Vector2 GetCurrentDrillAnchorWorld()
    {
        if (_targetWreck == null)
            return GetSteeringPosition();

        if (!_hasDrillAnchor)
            EnsureDrillAnchor();

        return (Vector2)_targetWreck.transform.position + _drillAnchorDirectionFromWreck * targetDistance;
    }

    private void ResetDrillAnchor()
    {
        _hasDrillAnchor = false;
        _drillAnchorDirectionFromWreck = Vector2.zero;
        _hoverTimer = 0f;
    }

    private void RotateTowardsPoint(Vector3 worldPoint, float dt)
    {
        Vector2 direction = worldPoint - transform.position;

        if (direction.sqrMagnitude <= 0.0001f)
            return;

        RotateTurretTowardsDirection(direction.normalized, dt);
    }

    private void RotateMovementTowardsDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude <= 0.0001f)
            return;

        if (movementTransform != null && movementTransform != transform)
        {
            movementTransform.up = direction.normalized;
            return;
        }

        if (_rb != null)
            _rb.MoveRotation(DirectionToAngle(direction));
        else
            transform.up = direction.normalized;
    }

    private void RotateTurretTowardsDirection(Vector2 direction, float dt)
    {
        if (direction.sqrMagnitude <= 0.0001f)
            return;

        Transform targetTransform = turretTransform != null ? turretTransform : transform;
        float targetAngle = DirectionToAngle(direction);

        float newAngle = Mathf.MoveTowardsAngle(
            targetTransform.eulerAngles.z,
            targetAngle,
            visualRotationSpeedDegrees * dt
        );

        targetTransform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }

    private float DirectionToAngle(Vector2 direction)
    {
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
    }

    private Vector2 GetMovementUp()
    {
        Transform targetTransform = movementTransform != null ? movementTransform : transform;
        return targetTransform.up;
    }

    private Transform GetPlayerParkingTransform()
    {
        if (Player.PlayerTransform != null)
            return Player.PlayerTransform;

        return _parkingTransform;
    }

    private void SetupParallax()
    {
        if (!useIntegratedParallax)
            return;

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        _cameraTransform = mainCamera.transform;
        _lastCameraPosition = _cameraTransform.position;
        _defaultScale = transform.localScale;
    }

    private void OnCameraMoved()
    {
        if (!useIntegratedParallax)
            return;

        if (_cameraTransform == null)
            SetupParallax();

        if (_cameraTransform == null)
            return;

        Vector3 cameraPosition = _cameraTransform.position;

        if (_mode == DroneMode.Parked)
        {
            _lastCameraPosition = cameraPosition;
            return;
        }

        float deltaX = cameraPosition.x - _lastCameraPosition.x;
        float deltaY = cameraPosition.y - _lastCameraPosition.y;

        float factor = GetParallaxFactor();

        _pendingParallaxDelta += new Vector2(
            deltaX * factor,
            deltaY * factor
        );

        _lastCameraPosition = cameraPosition;
    }

    private float GetParallaxFactor()
    {
        return parallaxSpeed + ((1f - parallaxSpeed) - (1f - parallaxSpeed) / _cameraScale);
    }

    private void ChangeScale(float newScale)
    {
        if (!useIntegratedParallax)
            return;

        if (notChangeScale)
            return;

        if (_cameraTransform == null)
            SetupParallax();

        if (_cameraTransform == null)
            return;

        float oldScale = _cameraScale;
        _cameraScale = newScale;

        transform.localScale = _defaultScale * (1f + ((newScale - 1f) * (parallaxSpeed * parallaxSpeed)));

        if (oldScale <= 0.0001f)
            return;

        Vector2 currentPredictedPosition =
            _rb != null
                ? _rb.position + _pendingParallaxDelta
                : (Vector2)transform.position;

        Vector2 byCameraPosition = currentPredictedPosition - (Vector2)_cameraTransform.position;
        Vector2 newPosition = (Vector2)_cameraTransform.position + byCameraPosition / (newScale / oldScale);

        Vector2 correctionDelta = newPosition - currentPredictedPosition;

        // Scale-parallax correction тоже не применяем напрямую.
        // Копим и применяем в FixedUpdate через MovePosition.
        _pendingParallaxDelta += correctionDelta;
    }
}
