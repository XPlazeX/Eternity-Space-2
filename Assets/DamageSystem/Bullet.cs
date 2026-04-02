using UnityEngine;
using DamageSystem;

[RequireComponent(typeof(_ExplosionBullet))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : AttackObject
{
    public const int parryExplosionID = 11;

    public delegate void stateLife();
    public event stateLife Deathed;
    public event stateLife Hitted;

    [SerializeField] private bool _otherDeathResource = false; // если истинно - подразумевается другой источник смерти, lifetime не применяется
    [SerializeField] private float _lifetime;
    [SerializeField] protected float _speed;
    [SerializeField] protected float _acceleration;
    [SerializeField] private bool _accelerateToZero = false;
    [Space()]
    [SerializeField] private int _piercingTargets = 0;
    [SerializeField] private bool _explodeOnTimer;
    [Space()]
    [SerializeField][Range(0, 1f)] private float relativeFactor = 0.66f;

    public float Lifetime => _lifetime;
    public float Lifetimer => _lifeTimer;

    public int Pierces
    {
        get { return _piercingTargets; }
        set { _piercingTargets = value; }
    }

    public float Acceleration
    {
        get { return _acceleration; }
        set { _acceleration = value; }
    }

    private TrailRenderer _trailRenderer;
    private ExplosionHandler _explosionHandler;
    private Rigidbody2D _rb;

    private float _lifeTimer;
    private float _startSpeed;
    private float _startAcceleration;
    private float _curPierces = 0;
    protected bool _ignoreForce = false;

    private void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        _rb = GetComponent<Rigidbody2D>();

        if (_rb != null)
        {
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rb.gravityScale = 0f;
        }

        if (!_otherDeathResource)
        {
            _explosionHandler = SceneStatics.CoresFinded ? SceneStatics.SceneCore.GetComponent<ExplosionHandler>() : null;
            if (_explosionHandler == null)
                SceneStatics.CoresLoaded += Initialize;
        }

        _explosionHandler = SceneStatics.SceneCore.GetComponent<ExplosionHandler>();
    }

    private void OnDestroy()
    {
        SceneStatics.CoresLoaded -= Initialize;
    }

    public override void Initialize()
    {
        base.Initialize();

        _startSpeed = _speed;
        _startAcceleration = _acceleration;

        _speed = _startSpeed;
    }

    private void FixedUpdate()
    {
        TickSpeed();
        TickMovement();

        if (_otherDeathResource)
            return;

        TickLifetime();
    }

    private void TickSpeed()
    {
        if (_accelerateToZero && (Mathf.Abs(_speed) < 0.01f * Mathf.Abs(Acceleration)))
            _speed = 0f;
        else
            _speed += Acceleration * Time.fixedDeltaTime;
    }

    private void TickMovement()
    {
        Vector2 movement =
            ((Vector2)transform.up * _speed + (_ignoreForce ? Vector2.zero : (Vector2)PlayerController.DefaultForce))
            * Time.fixedDeltaTime;

        movement = PlayerController.RelativeVectorAtPoint(movement, _rb.position, relativeFactor);

        if (_rb != null)
            _rb.MovePosition(_rb.position + movement);
        else
            transform.position += (Vector3)movement;
    }

    private void TickLifetime()
    {
        _lifeTimer -= Time.fixedDeltaTime;
        if (_lifeTimer <= 0f)
            Deactivate();
    }

    public virtual void MultiplySpeedParams(float multiplier = 1f)
    {
        _speed *= multiplier;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<__HardShield>() != null)
        {
            // Pierce();
            return;
        }
        
        DamageBody damageBody = other.GetComponentInParent<DamageBody>();

        if (damageBody == null || !InflictDamage(damageBody))
            return;

        Pierce();
    }

    protected override void SetDefaultStats()
    {
        _lifeTimer = _lifetime;

        if (KeyDamage == DamageKey.Enemy)
            _lifeTimer *= ShipStats.GetValue("PlayerShotLifetimeMultiplier");

        _curPierces = Pierces + ShipStats.GetIntValue("PiercesBoost", ShipStats.RoundMode.Floor);

        if (!_initialized)
            return;

        if (KeyDamage == DamageKey.Enemy)
        {
            _speed = _startSpeed * ShipStats.GetValue("PlayerShotSpeedMultiplier");
            _acceleration = _startAcceleration + ShipStats.GetValue("FlatPlayerBulletAcceleration");
        }
        else
        {
            _speed = _startSpeed * ShipStats.GetValue("EnemyBulletSpeedMultiplier");
        }
    }

    public virtual void Parrying()
    {
        gameObject.SetActive(false);

        if (_explosionHandler == null)
            _explosionHandler = SceneStatics.SceneCore.GetComponent<ExplosionHandler>();

        _explosionHandler.SpawnExplosion(transform.position, parryExplosionID);
    }

    protected virtual void Death()
    {
        gameObject.SetActive(false);
        Deathed?.Invoke();
    }

    public virtual void DetonateImmediately()
    {
        Death();
    }

    public virtual void Pierce(int times = 1)
    {
        _curPierces -= times;
        Hitted?.Invoke();

        if (_curPierces < 0)
            Death();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (_trailRenderer != null)
            _trailRenderer.Clear();
    }

    protected virtual void Deactivate()
    {
        if (!_explodeOnTimer)
            gameObject.SetActive(false);
        else
            Death();
    }
}