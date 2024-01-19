using UnityEngine;
using DamageSystem;

[RequireComponent(typeof(_ExplosionBullet))]
public class Bullet : AttackObject
{
    const int parryExplosionID = 11;

    public delegate void stateLife();
    public event stateLife Deathed;

    [SerializeField] private bool _otherDeathResource = false; // если истинно - подразумевается другой источник смерти, lifetime не применяется
    [SerializeField] private float _lifetime;
    [SerializeField] protected float _speed;
    [SerializeField] protected float _acceleration;
    [SerializeField] private bool _accelerateToZero = false;
    [Space()]
    [SerializeField] private int _piercingTargets = 0; 
    [SerializeField] private bool _explodeOnTimer;

    public float Lifetime => _lifetime;
    public float Lifetimer => _lifeTimer; 

    public int Pierces
    {
        get {return _piercingTargets;}
        set {_piercingTargets = value;}
    }
    public float Acceleration
    {
        get {return _acceleration;}
        set {_acceleration = value;}
    }

    private TrailRenderer _trailRenderer;
    private ExplosionHandler _explosionHandler;
    private float _lifeTimer;
    private float _startSpeed;
    private float _startAcceleration;
    private float _curPierces = 0;

    private void Awake() {
        _trailRenderer = GetComponent<TrailRenderer>();
        if (!_otherDeathResource)
        {
            _explosionHandler = SceneStatics.CoresFinded ? SceneStatics.SceneCore.GetComponent<ExplosionHandler>() : null;
            if (_explosionHandler == null)
                SceneStatics.CoresLoaded += Initialize;
        }
    }

    private void OnDestroy() {
        SceneStatics.CoresLoaded -= Initialize;
    }

    public override void Initialize()
    {
        base.Initialize();

        _startSpeed = _speed;
        _startAcceleration = _acceleration;

        _speed = _startSpeed;

        _explosionHandler = SceneStatics.SceneCore.GetComponent<ExplosionHandler>();
    }

    private void FixedUpdate() 
    {      
        if (!(_accelerateToZero && (Mathf.Abs(_speed) < 0.1f)))
            _speed += Acceleration * Time.deltaTime;

        transform.position += transform.up * _speed * Time.deltaTime;

        if (_otherDeathResource)
            return;

        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer <= 0)
            Deactivate();
    }

    public virtual void MultiplySpeedParams(float multiplier = 1f)
    {
        _speed *= multiplier;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        DamageBody damageBody = other.GetComponent<DamageBody>();

        if (damageBody == null || !InflictDamage(damageBody))
            return;

        Pierce();
    }

    protected override void SetDefaultStats()
    {
        _lifeTimer = _lifetime;

        if (KeyDamage == DamageKey.Enemy)
            _lifeTimer *= ShipStats.GetValue("PlayerShotLifetimeMultiplier");

        _curPierces = Pierces + ShipStats.GetIntValue("PiercesBoost");

        if (!_initialized)
            return;

        if (KeyDamage == DamageKey.Enemy)
        {
            _speed = _startSpeed * ShipStats.GetValue("PlayerShotSpeedMultiplier");
            _acceleration = _startAcceleration + ShipStats.GetValue("FlatPlayerBulletAcceleration");
        }
        else 
            _speed = _startSpeed * ShipStats.GetValue("EnemyBulletSpeedMultiplier");

    }

    public virtual void Parrying()
    {
        gameObject.SetActive(false);
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
        if (_curPierces < 0)
            Death();
    }

    protected override void OnDisable() {
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
