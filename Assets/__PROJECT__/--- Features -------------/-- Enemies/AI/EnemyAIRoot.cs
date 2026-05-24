using UnityEngine;

public class EnemyAIRoot : MonoBehaviour
{
    public const float ARENA_BORDERS_MOVING_OFFSET = 3f;

    [Header("General")]
    [SerializeField] protected bool autoStart = true;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float inertionSurmount = 10f;
    [Tooltip("How fast current movement inertia rotates toward desired movement direction, degrees per second.")]
    [SerializeField] private float inertionRotateSpeed = 360f;
    [SerializeField] protected LookingOrientation lookingOrientation = LookingOrientation.Fixed;
    [SerializeField] protected float rotationSpeed = 2f;
    [SerializeField] protected AnimationCurve movingProgression;
    [SerializeField] private float movementForesight = -15f;
    [Space()]
    [SerializeField] private bool stunnable = true;
    [SerializeField][Range(0, 1f)] private float relativityFactor = 1f; 
    [Header("Floating")]
    [SerializeField] private float floatingAmplitude = 1f;
    [SerializeField] private float floatingFrequency = 0.01f; 
    [Header("External Acceleration")]
    [SerializeField] private float externalAccelerationDamping = 12f;
    [SerializeField] private float externalAccelerationStopThreshold = 0.05f;

    protected float _startSpeed;
    private float _localMobility = 1f;
    private Vector3 _inertion = Vector3.zero;
    private Vector3 _floatingOffset = Vector3.zero;
    private Vector3 _aiPosition;
    private float _floatingPhase;
    private Vector2 _floatingDirectionA;
    private Vector2 _floatingDirectionB;
    private Vector2 _externalVelocity;
    private Vector2 _externalForceDirection;
    private float _externalForce;
    private float _externalForceTimer;

    protected Vector3 _targetPosition;
    protected Vector3 _bufferDirection;
    protected Transform _player;
    protected Rigidbody2D _rb;

    public Vector3 AiPosition => _aiPosition;
    public Vector3 FloatingOffset => _floatingOffset;
    public Vector3 RealPosition => _aiPosition + _floatingOffset;
    public Vector3 Inertion => _inertion;
    public float Speed => speed;
    public float Mobility => RR.Get(RuntimeStat.EnemyMobilityMultiplier) * _localMobility;
    public bool Active { get; private set; } = false;
    public bool Stunned { get; private set; } = false;
    public float MovementForesight => movementForesight + RR.Get(RuntimeStat.EnemyForesightAddition);
    public bool ExternalAccelerationActive =>
        _externalForceTimer > 0f || _externalVelocity.sqrMagnitude > externalAccelerationStopThreshold * externalAccelerationStopThreshold;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (_rb != null)
        {
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _aiPosition = _rb.position;
        }
        else
        {
            _aiPosition = transform.position;
        }

        _floatingPhase = Random.Range(0f, Mathf.PI * 2f);

        float angleA = Random.Range(0f, Mathf.PI * 2f);
        float angleB = angleA + Mathf.PI * 0.5f;

        _floatingDirectionA = new Vector2(Mathf.Cos(angleA), Mathf.Sin(angleA));
        _floatingDirectionB = new Vector2(Mathf.Cos(angleB), Mathf.Sin(angleB));
    }

    private void OnEnable()
    {
        _startSpeed = speed;
        speed *= Random.Range(0.9f, 1.15f);

        Player.PlayerChanged += FindPlayer;
        FindPlayer();

        if (stunnable)
        {
            GetComponent<DamageBody>().Stunned += OnStunned;
            GetComponent<DamageBody>().Unstunned += OnUnstunned;
        }
    }

    protected virtual void Start()
    {
        if (autoStart)
            StartMoving();
    }

    protected virtual void FixedUpdate()
    {
        if (!Active)
        {
            return;
        }

        UpdateFloatingOffset();

        if (Stunned)
        {
            ClearExternalAcceleration();

            Vector2 stunMoveDelta = _bufferDirection * (1f - Time.fixedDeltaTime);
            ApplyMovement(stunMoveDelta);
            return;
        }

        if (lookingOrientation == LookingOrientation.RotateToPlayer)
            RotateToPlayer();
        else if (lookingOrientation == LookingOrientation.RotateToTarget)
            RotateToTarget();

        if (TryApplyExternalAcceleration())
        {
            return;
        }

        Vector2 moveDelta = GetMoveDelta();
        ApplyMovement(moveDelta);
    }

    protected virtual Vector2 GetMoveDelta() // Основной метод наследования движения
    {
        return Vector2.zero;
    }

    protected virtual void ApplyMovement(Vector2 moveDelta) // fixed
    {
        Vector2 finalMoveDelta = UpdateInertionTowards(moveDelta);

        ApplyMoveDelta(finalMoveDelta);
    }

    public bool Accelerate(
        Vector2 direction,
        float force,
        float duration,
        bool resetCurrentAcceleration = true,
        bool resetInertion = false)
    {
        if (Stunned)
            return false;

        if (!Active)
            return false;

        if (direction.sqrMagnitude <= 0.0001f)
            return false;

        direction.Normalize();

        if (resetCurrentAcceleration)
            _externalVelocity = Vector2.zero;

        if (resetInertion)
            _inertion = Vector3.zero;

        _externalForceDirection = direction;
        _externalForce = Mathf.Max(0f, force);
        _externalForceTimer = Mathf.Max(0f, duration);

        return true;
    }

    public void ClearExternalAcceleration()
    {
        _externalVelocity = Vector2.zero;
        _externalForceDirection = Vector2.zero;
        _externalForce = 0f;
        _externalForceTimer = 0f;
    }

    private bool TryApplyExternalAcceleration()
    {
        float dt = Time.fixedDeltaTime;

        if (_externalForceTimer > 0f)
        {
            _externalForceTimer -= dt;
            _externalVelocity += _externalForceDirection * (_externalForce * dt);
        }
        else
        {
            _externalVelocity = Vector2.MoveTowards(
                _externalVelocity,
                Vector2.zero,
                externalAccelerationDamping * dt
            );
        }

        bool externalStillActive =
            _externalForceTimer > 0f ||
            _externalVelocity.sqrMagnitude > externalAccelerationStopThreshold * externalAccelerationStopThreshold;

        if (!externalStillActive)
        {
            _externalVelocity = Vector2.zero;
            _externalForceDirection = Vector2.zero;
            _externalForce = 0f;
            _externalForceTimer = 0f;

            // ВАЖНО:
            // _inertion не сбрасываем.
            // Она остаётся как текущая скорость движения и дальше плавно повернётся к GetMoveDelta().
            return false;
        }

        Vector2 targetMoveDelta = _externalVelocity * dt;
        Vector2 finalMoveDelta = UpdateInertionTowards(targetMoveDelta);

        ApplyMoveDelta(finalMoveDelta);

        return true;
    }

    public virtual void StartMoving() => Active = true;

    public virtual void StopMoving() => Active = false;

    private void OnDisable()
    {
        Player.PlayerChanged -= FindPlayer;

        if (stunnable)
        {
            GetComponent<DamageBody>().Stunned -= OnStunned;
            GetComponent<DamageBody>().Unstunned -= OnUnstunned;
        }
    }

    protected void RotateToTarget()
    {
        transform.up = SceneStatics.FlatVector(
            Vector3.RotateTowards(
                transform.up,
                _targetPosition - RealPosition,
                rotationSpeed * Time.fixedDeltaTime * (Speed / _startSpeed) * Mobility,
                0f));

        CorrectRotation();
    }

    protected virtual void RotateToPlayer()
    {
        transform.up = SceneStatics.FlatVector(
            Vector3.RotateTowards(
                transform.up,
                Player.GetPlayerPosition(MovementForesight) - RealPosition,
                rotationSpeed * Time.fixedDeltaTime * (Speed / _startSpeed) * Mobility,
                0f));

        CorrectRotation();
    }

    private void ApplyRealPosition()
    {
        if (_rb != null)
        {
            _rb.MovePosition(RealPosition);
        }
        else
        {
            transform.position = RealPosition;
        }
    }

    private void ApplyMoveDelta(Vector2 moveDelta)
    {
        if (moveDelta.sqrMagnitude > 0f)
        {
            moveDelta = ArenaLocal.RelativeVectorAtPoint(
                moveDelta,
                _aiPosition,
                relativityFactor
            );

            _aiPosition += (Vector3)moveDelta;
            _bufferDirection = moveDelta;
        }

        ApplyRealPosition();
    }

    private Vector2 UpdateInertionTowards(Vector2 targetMoveDelta)
    {
        float dt = Time.fixedDeltaTime;

        Vector2 current = _inertion;
        float currentMagnitude = current.magnitude;
        float targetMagnitude = targetMoveDelta.magnitude;

        // Если почти стоим — просто начинаем движение в нужную сторону,
        // но magnitude всё равно набираем через inertionSurmount.
        if (currentMagnitude <= 0.0001f)
        {
            float newMagnitude = Mathf.MoveTowards(
                0f,
                targetMagnitude,
                inertionSurmount * dt
            );

            _inertion = targetMoveDelta.sqrMagnitude > 0.0001f
                ? (Vector3)(targetMoveDelta.normalized * newMagnitude)
                : Vector3.zero;

            return _inertion;
        }

        // Если новая цель почти нулевая — тормозим без попытки повернуть вектор.
        if (targetMagnitude <= 0.0001f)
        {
            float newMagnitude = Mathf.MoveTowards(
                currentMagnitude,
                0f,
                inertionSurmount * dt
            );

            _inertion = current.normalized * newMagnitude;
            return _inertion;
        }

        Vector2 currentDirection = current.normalized;
        Vector2 targetDirection = targetMoveDelta.normalized;

        float maxRadiansDelta = inertionRotateSpeed * Mathf.Deg2Rad * dt;

        Vector2 newDirection = Vector3.RotateTowards(
            currentDirection,
            targetDirection,
            maxRadiansDelta,
            0f
        );

        float newMagnitude2 = Mathf.MoveTowards(
            currentMagnitude,
            targetMagnitude,
            inertionSurmount * dt
        );

        _inertion = newDirection.normalized * newMagnitude2;
        return _inertion;
    }

    private void UpdateFloatingOffset()
    {
        if (floatingAmplitude <= 0f || floatingFrequency <= 0f)
        {
            _floatingOffset = Vector3.zero;
            return;
        }

        float time = Time.time * floatingFrequency * Mathf.PI * 2f + _floatingPhase;

        Vector2 offset =
            _floatingDirectionA * Mathf.Sin(time) +
            _floatingDirectionB * Mathf.Sin(time * 0.73f + 1.37f) * 0.5f;

        offset *= floatingAmplitude;

        _floatingOffset = new Vector3(offset.x, offset.y, 0f);
    }

    protected virtual void CorrectRotation()
    {
        if (transform.rotation.eulerAngles.y != 180 && transform.rotation.eulerAngles.y != -180)
            return;

        transform.rotation = Quaternion.Euler(0, 0, 180);
    }

    public void LocalMultiplyMobility(float multiplier)
    {
        _localMobility *= multiplier;
    }

    public void Reload()
    {
        OnDisable();
        OnEnable();
    }

    private void OnStunned()
    {
        Stunned = true;
    }

    private void OnUnstunned()
    {
        Stunned = false;
    }

    protected float GetStepSpeed()
    {
        return Speed * Mobility * Time.fixedDeltaTime;
    }

    public virtual void FindPlayer() => _player = Player.PlayerTransform;

    protected enum LookingOrientation
    {
        Fixed,
        RotateToPlayer,
        RotateToTarget
    }
}