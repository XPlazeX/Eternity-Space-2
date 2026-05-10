using UnityEngine;

[RequireComponent(typeof(DeathCaller))]
public class AsteroidBody : DamageBody
{
    [Tooltip("Если меньше 0, то не уничтожается через время")]
    [SerializeField] protected float _lifetime = -1f;
    [SerializeField] private Vector2 _minMaxSpeed;
    [SerializeField] private bool _explodeOnTimer;

    private float _lifeTimer;
    private float _selectedSpeed;
    private Vector3 _direction;
    private Rigidbody2D _rb;

    public override bool RamReady => false;

    protected override void Awake()
    {
        base.Awake();
        OneShotProtection = false;

        _rb = GetComponent<Rigidbody2D>();
        if (_rb != null)
        {
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rb.gravityScale = 0f;
        }
    }

    private void OnEnable()
    {
        _lifeTimer = _lifetime * ((Mathf.Abs(ArenaLocal.WNegY) + ArenaLocal.WPosY + ArenaLocal.Height) / ArenaLocal.Height);
        _selectedSpeed = Random.Range(_minMaxSpeed.x, _minMaxSpeed.y);
        HitPoints = _startHP;
        _direction = transform.up;
    }

    private void FixedUpdate()
    {
        Vector2 moveDelta = (Vector2)(_direction * _selectedSpeed * Time.fixedDeltaTime);

        if (_rb != null)
            _rb.MovePosition(_rb.position + moveDelta);
        else
            transform.position += (Vector3)moveDelta;

        if (_lifetime >= 0f)
        {
            _lifeTimer -= Time.fixedDeltaTime;
            if (_lifeTimer <= 0f)
            {
                if (_explodeOnTimer)
                    Death();
                    // TakeDamage(999);

                gameObject.SetActive(false);
            }
        }
    }

    protected override void Stun(){}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_damageKey == DamageSystem.DamageKey.Enemy || other.GetComponent<Hover>() != null)
            return;

        DamageBody damageBody = other.GetComponentInParent<DamageBody>();

        if (damageBody == null)
            return;

        // if (damageBody.KeyDamage == DamageSystem.DamageKey.Enemy || damageBody.KeyDamage == DamageSystem.DamageKey.Everything)
        // {
        //     int otherHP = damageBody.HitPoints;
        //     damageBody.TakeDamage(HitPoints);
        //     TakeDamage(otherHP);
        // }
    }
}