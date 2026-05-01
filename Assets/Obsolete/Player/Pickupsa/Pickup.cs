using UnityEngine;

[RequireComponent(typeof(DeathCaller))]
public class Pickup : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _lifeTime;
    [SerializeField] private int _playingEffectID = -1;
    [SerializeField] protected float _effectDuration;
    [SerializeField] protected string _timeScaleByStat;

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
        _windAngle = GameObject.FindWithTag("Level core")
            .GetComponent<EnvironmentSpawner>().WindAngle + Random.Range(150f, 210f);

        _moveDirection = (Quaternion.Euler(0f, 0f, _windAngle) * Vector3.up).normalized;
    }

    private void FixedUpdate()
    {
        Vector2 moveDelta = _moveDirection * _speed * Time.fixedDeltaTime;

        if (_rb != null)
            _rb.MovePosition(_rb.position + moveDelta);
        else
            transform.position += (Vector3)moveDelta;

        _lifeTimer += Time.fixedDeltaTime;

        if (_lifeTimer > _lifeTime)
            Destroying();
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
        GetComponent<DeathCaller>().DeathExplosion();
        Destroy(gameObject);
    }
}