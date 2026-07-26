using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Wreck : MonoBehaviour
{
    [SerializeField] private float price = 10f;
    [SerializeField] private Vector2 minMaxDrillCapacity = new Vector2(40f, 120f);
    [Header("Wreck mask")]
    [SerializeField] private bool useWreckMaskController = true;
    [SerializeField] private float wreckMaskTargetBreachRadius = 26f;

    [Header("Particles")]
    [SerializeField] private bool useParticles;
    [SerializeField] private ParticleSystem sharpParticles;

    [Header("Explosions")]
    [SerializeField] private bool useExplosions;
    [SerializeField] private ExplosionRepeater explosionRepeater;

    [Header("Charging")]
    [SerializeField] private bool canBeCharged;
    [SerializeField] private ParticleSystem chargedParticles;

    [Header("Movement")]
    [SerializeField] private float linearDrag = 2.5f;
    [SerializeField] private float minVelocityToStop = 0.01f;

    [Tooltip("Если true, SetVelocity принимает уже готовое смещение за fixed step: velocity * fixedDeltaTime.")]
    [SerializeField] private bool setVelocityReceivesFixedStepDelta = true;

    [Header("Rotation")]
    [SerializeField] private float angularDrag = 3.5f;
    [SerializeField] private float minAngularVelocityToStop = 0.1f;

    [Tooltip("Сколько вращения добавить при Detonate. В градусах/сек.")]
    [SerializeField] private Vector2 detonateAngularVelocityRange = new Vector2(90f, 220f);

    private Rigidbody2D _rb;

    // Всегда храним скорость в units/second.
    private WreckMaskController _wmc;
    private float _wmcStartBreachRadius;
    private Vector2 _velocity;
    private bool _charged = false;
    private float _drillCapacity = 0f;
    private float _startDrillCapacity;

    public bool Charged => _charged;
    public float Price => Charged ? price * 1.5f : price;

    // Градусы/сек.
    private float _angularVelocity;

    public float DrilledAmount01 => _drillCapacity / _startDrillCapacity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        _rb.bodyType = RigidbodyType2D.Kinematic;

        _rb.interpolation = RigidbodyInterpolation2D.None;

        _startDrillCapacity = Random.Range(minMaxDrillCapacity.x, minMaxDrillCapacity.y);
        _drillCapacity = _startDrillCapacity;

        _wmc = GetComponent<WreckMaskController>();
        if (_wmc != null)
        {
            _wmcStartBreachRadius = _wmc.BreachRadius;   
        }
    }

    private void Update()
    {
        float dt = ESTime.worldDeltaTime;

        UpdateMovement(dt);
        UpdateRotation(dt);
    }

    public void SetVelocity(Vector3 velocity)
    {
        Vector2 value = new Vector2(velocity.x, velocity.y);

        if (setVelocityReceivesFixedStepDelta)
        {
            float fixedDt = ESTime.worldFixedDeltaTime;

            if (fixedDt > 0f)
                value /= fixedDt;
        }

        _velocity = value;
    }

    public void SetVelocityUnitsPerSecond(Vector3 velocity)
    {
        _velocity = new Vector2(velocity.x, velocity.y);
    }

    public void SetFixedStepDelta(Vector3 fixedStepDelta)
    {
        float fixedDt = ESTime.worldFixedDeltaTime;

        if (fixedDt <= 0f)
        {
            _velocity = Vector2.zero;
            return;
        }

        _velocity = new Vector2(fixedStepDelta.x, fixedStepDelta.y) / fixedDt;
    }

    public void Detonate()
    {
        Vector3 detonatePosition = transform.position;

        if (useWreckMaskController)
        {
            if (_wmc != null)
                detonatePosition = _wmc.RandomizeBreach();
        }

        if (useParticles && sharpParticles != null)
        {
            sharpParticles.transform.position = detonatePosition;
            sharpParticles.Play();
        }

        if (useExplosions && explosionRepeater != null)
        {
            explosionRepeater.Activate(detonatePosition);
        }

        AddRandomSpin();
    }

    public bool TryCharge()
    {
        if (!canBeCharged || _charged) return false;

        _charged = true;
        chargedParticles.Play();

        return true;
    }

    private void UpdateMovement(float dt)
    {
        if (_velocity.sqrMagnitude <= 0f)
            return;

        transform.position += (Vector3)(_velocity * dt);

        _velocity = ApplyDrag(_velocity, linearDrag, dt);

        if (_velocity.sqrMagnitude <= minVelocityToStop * minVelocityToStop)
            _velocity = Vector2.zero;
    }

    private void UpdateRotation(float dt)
    {
        if (Mathf.Abs(_angularVelocity) <= 0f)
            return;

        transform.Rotate(0f, 0f, _angularVelocity * dt);

        _angularVelocity = ApplyDrag(_angularVelocity, angularDrag, dt);

        if (Mathf.Abs(_angularVelocity) <= minAngularVelocityToStop)
            _angularVelocity = 0f;
    }

    private void AddRandomSpin()
    {
        float min = Mathf.Min(detonateAngularVelocityRange.x, detonateAngularVelocityRange.y);
        float max = Mathf.Max(detonateAngularVelocityRange.x, detonateAngularVelocityRange.y);

        float spin = Random.Range(min, max);

        if (Random.value < 0.5f)
            spin = -spin;

        _angularVelocity += spin;
    }

    private Vector2 ApplyDrag(Vector2 value, float drag, float dt)
    {
        if (drag <= 0f)
            return value;

        return value * Mathf.Exp(-drag * dt);
    }

    private float ApplyDrag(float value, float drag, float dt)
    {
        if (drag <= 0f)
            return value;

        return value * Mathf.Exp(-drag * dt);
    }

    public bool IsDrilled(float drillAmount)
    {
        _drillCapacity -= drillAmount;
        

        if (_wmc != null)
        {
            float elapsed = _drillCapacity / _startDrillCapacity;
            _wmc.SetBreachRadiusPixels(Mathf.Lerp(_wmcStartBreachRadius, wreckMaskTargetBreachRadius, 1f - elapsed));
        }

        if (_drillCapacity <= 0)
        {
            Destroy(gameObject);
            return true;
        }

        return false;
    }
}