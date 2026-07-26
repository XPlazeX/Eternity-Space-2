using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private bool moving = false;
    [SerializeField] private float _speed;
    [SerializeField] private bool hasLifetime = false;
    [SerializeField] private float _lifeTime;
    [SerializeField] private int _playingEffectID = -1;
    [SerializeField] protected float _effectDuration;
    [SerializeField] protected string _timeScaleByStat;
    [Header("Explosion")]
    [SerializeField] private bool explodeOnPickup;
    [SerializeField] private ExplosionObject explosionObject;
    [SerializeField] private float scale = 1f;
    [SerializeField] private bool overrideColor = false;
    [SerializeField] private Color color = Color.wheat;

    private float _windAngle;
    private float _lifeTimer = 0f;
    private Rigidbody2D _rb;
    private Vector2 _moveDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (_rb != null)
        {
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rb.gravityScale = 0f;
        }
    }

    private void Start()
    {
        // _windAngle = GameObject.FindWithTag("Level core")
        //     .GetComponent<EnvironmentSpawner>().WindAngle + Random.Range(150f, 210f);

        // _moveDirection = (Quaternion.Euler(0f, 0f, _windAngle) * Vector3.up).normalized;
    }

    private void FixedUpdate()
    {
        if (moving)
        {
            Vector2 moveDelta = _moveDirection * _speed * ESTime.worldFixedDeltaTime;

            if (_rb != null)
                _rb.MovePosition(_rb.position + moveDelta);
            else
                transform.position += (Vector3)moveDelta;

        }
        
        if (hasLifetime)
        {
            _lifeTimer += ESTime.worldFixedDeltaTime;

            if (_lifeTimer > _lifeTime)
                Destroying();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Picked();
    }

    protected virtual void Picked()
    {
        if (_playingEffectID > -1)
        {
            SceneStatics.UICore.GetComponent<PlayerUI>().PlayEffect(
                (PlayerUI.Effect)_playingEffectID,
                _effectDuration + (string.IsNullOrEmpty(_timeScaleByStat) ? 0f : ShipStats.GetValue(_timeScaleByStat)));
        }

        Destroying();
    }

    protected virtual void Destroying()
    {
        if (explodeOnPickup)
        {
            ExplosionObject explosion = Pool.Spawn(explosionObject, transform.position, transform.rotation);
            explosion.SetScale(scale);
            if (overrideColor)
                explosion.SetColor(color);
        }
        Destroy(gameObject);
    }
}