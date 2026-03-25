using UnityEngine;

public class EnemyAIRoot : MonoBehaviour
{
    public const float level_borders_moving_offset = 3f;

    protected enum LookingOrientation
    {
        Fixed,
        RotateToPlayer,
        RotateToTarget
    }

    [SerializeField] protected bool _autoStart = true;
    [SerializeField] private float _speed;
    [SerializeField] protected LookingOrientation _orientation;
    [SerializeField] protected float _rotationSpeed;
    [SerializeField] protected AnimationCurve _movingProgression;
    [SerializeField] protected float _foresight = 0f;
    [SerializeField] private float _movementForesight = -15f;
    [SerializeField] private bool stunnable = true;

    protected float _startSpeed;
    private float _mobility = 1f;
    private float _localMobility = 1f;
    protected Vector3 _targetPosition;
    protected Vector3 _bufferDirection;
    protected Transform _player;
    protected Rigidbody2D _rb;

    public float Speed => _speed;
    public float Mobility => _mobility * _localMobility;
    public bool Active { get; private set; } = false;
    public bool Stunned { get; private set; } = false;
    public float MovementForesight { get; protected set; }

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (_rb != null)
        {
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }

    private void OnEnable()
    {
        _startSpeed = _speed;
        _speed *= Random.Range(0.9f, 1.15f);

        Player.PlayerChanged += FindPlayer;
        FindPlayer();

        ShipStats.StatChanged += ObserveStat;
        _mobility = ShipStats.GetValue("EnemyMobilityMultiplier");
        MovementForesight = _movementForesight + ShipStats.GetValue("MovementForesightAddition");

        if (stunnable)
        {
            GetComponent<DamageBody>().Stunned += OnStunned;
            GetComponent<DamageBody>().Unstunned += OnUnstunned;
        }
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

    private void ObserveStat(string name, float val)
    {
        if (name == "EnemyMobilityMultiplier")
            _mobility = ShipStats.GetValue("EnemyMobilityMultiplier");

        if (name == "MovementForesightAddition")
            MovementForesight = _movementForesight + ShipStats.GetValue("MovementForesightAddition");
    }

    protected virtual void Start()
    {
        if (_autoStart)
            StartMoving();
    }

    protected virtual void FixedUpdate()
    {
        if (!Active)
            return;

        if (Stunned)
        {
            Vector2 stunMoveDelta = _bufferDirection * (1f - Time.fixedDeltaTime);
            ApplyMovement(stunMoveDelta);
            return;
        }

        if (_orientation == LookingOrientation.RotateToPlayer)
            RotateToPlayer();
        else if (_orientation == LookingOrientation.RotateToTarget)
            RotateToTarget();

        Vector2 moveDelta = GetMoveDelta();
        ApplyMovement(moveDelta);
    }

    protected virtual Vector2 GetMoveDelta()
    {
        return Vector2.zero;
    }

    protected virtual void ApplyMovement(Vector2 moveDelta)
    {
        if (moveDelta.sqrMagnitude <= 0f)
            return;

        if (_rb != null)
        {
            _rb.MovePosition(_rb.position + moveDelta);
        }
        else
        {
            transform.position += (Vector3)moveDelta;
        }

        _bufferDirection = moveDelta;
    }

    public virtual void StartMoving() => Active = true;

    public virtual void StopMoving() => Active = false;

    private void OnDisable()
    {
        Player.PlayerChanged -= FindPlayer;
        ShipStats.StatChanged -= ObserveStat;

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
                _targetPosition - transform.position,
                _rotationSpeed * Time.fixedDeltaTime * (Speed / _startSpeed) * Mobility,
                0f));

        CorrectRotation();
    }

    protected virtual void RotateToPlayer()
    {
        transform.up = SceneStatics.FlatVector(
            Vector3.RotateTowards(
                transform.up,
                Player.GetPlayerPosition(_foresight) - transform.position,
                _rotationSpeed * Time.fixedDeltaTime * (Speed / _startSpeed) * Mobility,
                0f));

        CorrectRotation();
    }

    protected virtual void CorrectRotation()
    {
        if (transform.rotation.eulerAngles.y != 180 && transform.rotation.eulerAngles.y != -180)
            return;

        transform.rotation = Quaternion.Euler(0, 0, 180);
    }

    protected float GetStepSpeed()
    {
        return Speed * Mobility * Time.fixedDeltaTime;
    }

    public virtual void FindPlayer() => _player = Player.PlayerTransform;
}