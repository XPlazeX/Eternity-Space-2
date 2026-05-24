using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CatapultEscapeAI : MonoBehaviour
{
    [Header("Catapult")]
    [SerializeField] private float catapultingImpulse = 5f;
    [SerializeField] private float activationTime = 2f;
    [SerializeField][Range(0, 1f)] private float engineSuccesChance = 0.9f;
    [SerializeField] private bool colorSpriteOnFailure = true;
    [SerializeField] private Color failureColor = Color.gray;

    [Header("Engine")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float acceleration = 4f;

    [Tooltip("Градусов в секунду. Чем меньше, тем шире дуга.")]
    [SerializeField] private float turnSpeedDegrees = 90f;

    [Header("Inertia")]
    [SerializeField] private float drag = 0.5f;

    [Header("Escape")]
    [SerializeField] private float distanceToEscape = 100f;

    private Rigidbody2D _rb;
    private Vector2 _inertia;
    private Vector2 _targetDirection;

    private float _activationTimer = 0f;
    private bool _catapulted = false;
    private bool _engineActive = false;
    private bool _unhappy;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (_rb != null)
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Start()
    {
        _inertia = transform.up.normalized * catapultingImpulse;
        _targetDirection = ((Vector2)(transform.position - ArenaLocal.Pivot.position)).normalized;
        _catapulted = true;

        if (Random.value > engineSuccesChance)
        {
            _unhappy = true;
            if (colorSpriteOnFailure && GetComponent<SpriteRenderer>() != null)
            {
                GetComponent<SpriteRenderer>().color = failureColor;
            }
            return;
        }

        _activationTimer = activationTime;
    }

    private void FixedUpdate()
    {
        if (!_catapulted)
            return;

        float dt = Time.fixedDeltaTime;

        _activationTimer -= dt;

        if (_activationTimer <= 0f && !_unhappy)
            _engineActive = true;

        if (_engineActive)
            UpdateEngineMovement(dt);
        else
            UpdateCatapultInertia(dt);

        _rb.MovePosition(_rb.position + _inertia * dt);

        if (_inertia.sqrMagnitude > 0.000001f)
            transform.up = _inertia.normalized;

        if ((transform.position - Player.PlayerTransform.position).magnitude > distanceToEscape)
            Escape();
    }

    private void UpdateCatapultInertia(float dt)
    {
        _inertia = Vector2.MoveTowards(
            _inertia,
            Vector2.zero,
            drag * dt
        );

        if (_inertia.magnitude <= 0.001f)
            _inertia = Vector2.zero;
    }

    private void UpdateEngineMovement(float dt)
    {
        Vector2 currentDirection;

        if (_inertia.sqrMagnitude > 0.000001f)
            currentDirection = _inertia.normalized;
        else
            currentDirection = transform.up.normalized;

        float currentSpeed = _inertia.magnitude;

        float maxRadiansDelta = turnSpeedDegrees * Mathf.Deg2Rad * dt;

        Vector2 turnedDirection = Vector3.RotateTowards(
            currentDirection,
            _targetDirection,
            maxRadiansDelta,
            0f
        );

        float newSpeed = Mathf.MoveTowards(
            currentSpeed,
            speed,
            acceleration * dt
        );

        _inertia = turnedDirection.normalized * newSpeed;
    }

    private void Escape()
    {
        Destroy(gameObject);
    }
}