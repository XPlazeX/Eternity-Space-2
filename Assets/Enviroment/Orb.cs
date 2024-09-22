using UnityEngine;

public class Orb : PullableObject
{
    [SerializeField] private float _speed;
    [SerializeField] private float _distanceDisable = 0.05f;

    private Transform _target;
    private TrailRenderer _trailRenderer;
    private bool _deathed;
    private float _disableTimer;

    private void Awake() {
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    public void Set(Vector3 startPosition, Transform _targetTransform)
    {
        _target = _targetTransform;
        transform.position = startPosition;
        _deathed = false;
        _disableTimer = 0.4f;
    }

    private void Update() {
        if (_target == null)
        {
            if (!_deathed)
                Death();
            return;
        }

        if ((transform.position - _target.position).magnitude <= _distanceDisable)
        {
            _disableTimer -= Time.deltaTime;

            if (_disableTimer <= 0f)
                Death();
                
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, _target.position, _speed * Time.deltaTime);
    }

    public void Death()
    {
        gameObject.SetActive(false);
        _deathed = true;
    }

    protected override void OnDisable() {
        base.OnDisable();
        if (_trailRenderer != null)
            _trailRenderer.Clear();
    }
}
