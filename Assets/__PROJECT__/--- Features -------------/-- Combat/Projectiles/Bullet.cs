using UnityEngine;
using DamageSystem;

[RequireComponent(typeof(_ExplosionBullet))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : AttackObject
{
    public const float MAX_INERTION_ADDITIVE_SPEED_MULTIPLIER = 1.33f;
    public const float MIN_INERTION_ADDITIVE_SPEED_MULTIPLIER = 1f;
    public const int parryExplosionID = 11;

    public delegate void stateLife();
    public delegate void interaction(GameObject other);
    public event stateLife Deathed;
    public event stateLife Hitted;
    public event interaction Collided;
    public event System.Action<IDamagable> Killed;

    [SerializeField] private bool _otherDeathResource = false; // если истинно - подразумевается другой источник смерти, lifetime не применяется
    [SerializeField] private float _lifetime;
    [SerializeField] protected float _speed;
    [SerializeField] protected float _acceleration;
    [SerializeField] private float _topAcceleratedSpeed;
    [Space()]
    [SerializeField] private int _piercingTargets = 0;
    [SerializeField] private bool _explodeOnTimer;
    [Space()]
    [SerializeField][Range(0, 1f)] private float relativeFactor = 0.66f;

    public float Lifetime => _lifetime;
    public float Lifetimer => _lifeTimer;
    public float LifetimeLeftNormalized => Mathf.Clamp01(_lifeTimer / _lifetime);

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
                SceneStatics.CoresLoaded += ResetState;
        }

        _startSpeed = _speed;
        _startAcceleration = _acceleration;

        _speed = _startSpeed;

        _explosionHandler = SceneStatics.SceneCore.GetComponent<ExplosionHandler>();
    }

    private void OnDestroy()
    {
        SceneStatics.CoresLoaded -= ResetState;
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
        if (Acceleration == 0) return;

        if (_speed < _topAcceleratedSpeed)
        {
            _speed = Mathf.Clamp(_speed + Acceleration * ESTime.worldFixedDeltaTime, _speed, _topAcceleratedSpeed);
        }
        else if (_speed > _topAcceleratedSpeed)
        {
            _speed = Mathf.Clamp(_speed + Acceleration * ESTime.worldFixedDeltaTime, _topAcceleratedSpeed, _speed); // здесь в инспекторе отрицательное ускорение
        }
    }

    private void TickMovement()
    {
        Vector2 movement =
            ((Vector2)transform.up * _speed + (_ignoreForce ? Vector2.zero : (Vector2)PlayerController.DefaultForce))
            * ESTime.worldFixedDeltaTime;

        movement = ArenaLocal.RelativeVectorAtPoint(movement, _rb.position, relativeFactor);

        if (_rb != null)
            _rb.MovePosition(_rb.position + movement);
        else
            transform.position += (Vector3)movement;
    }

    private void TickLifetime()
    {
        _lifeTimer -= ESTime.worldFixedDeltaTime;
        if (_lifeTimer <= 0f)
            Death();
    }

    public virtual void MultiplySpeedParams(float multiplier = 1f, Vector3 inertionFixedDelta = default(Vector3))
    {
        if (_speed == 0f)
            return;

        float inertionMultiplier = MIN_INERTION_ADDITIVE_SPEED_MULTIPLIER;

        if (inertionFixedDelta.sqrMagnitude > 0.000001f && ESTime.worldFixedDeltaTime > Mathf.Epsilon)
        {
            Vector3 inertionVelocity = inertionFixedDelta / ESTime.worldFixedDeltaTime;
            float forwardInertionSpeed = Mathf.Max(0f, Vector3.Dot(transform.up, inertionVelocity));
            float speedRatio = forwardInertionSpeed / Mathf.Abs(_speed);

            inertionMultiplier = Mathf.Clamp(
                MIN_INERTION_ADDITIVE_SPEED_MULTIPLIER + speedRatio,
                MIN_INERTION_ADDITIVE_SPEED_MULTIPLIER,
                MAX_INERTION_ADDITIVE_SPEED_MULTIPLIER
            );
        }

        _speed *= multiplier * inertionMultiplier;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<__IgnoreBulletsCollisions__>() != null)
        {
            return;
        }
        
        IDamagable damagable = other.GetComponentInParent<IDamagable>();

        if (damagable == null || !InflictDamage(damagable, out bool killed))
            return;

        if (killed)
        {
            Killed?.Invoke(damagable);
        }
        Pierce(other.gameObject);
    }

    protected override void ResetState()
    {
        _lifeTimer = _lifetime;

        if (KeyDamage == DamageKey.Enemy)
            _lifeTimer *= ShipStats.GetValue("PlayerShotLifetimeMultiplier");

        _curPierces = Pierces + ShipStats.GetIntValue("PiercesBoost", ShipStats.RoundMode.Floor);

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

    protected virtual void Death(bool silent = false)
    {
        Release();
        if (!silent)
            Deathed?.Invoke();
    }

    public virtual void DetonateImmediately()
    {
        Death();
    }

    public virtual void Pierce(GameObject gameObject, int times = 1)
    {
        _curPierces -= times;
        Hitted?.Invoke();
        Collided?.Invoke(gameObject);

        if (_curPierces < 0)
            Death();
    }

    protected void OnDisable()
    {
        if (_trailRenderer != null)
            _trailRenderer.Clear();
    }
}
