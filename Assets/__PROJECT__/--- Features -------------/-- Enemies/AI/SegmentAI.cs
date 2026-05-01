using UnityEngine;

[RequireComponent(typeof(DeathCaller))]
public class SegmentAI : MonoBehaviour
{
    [SerializeField] private Transform _targetFollow;
    [SerializeField] private bool _destroyWithTarget = true;
    [SerializeField] private float _followSpeed;
    [SerializeField] private float _closingDistance;
    [SerializeField] private bool _rotateToTarget;
    [Space()]
    [SerializeField] private bool _offsetBinding;
    [SerializeField] private Vector3 _targetOffset;

    private Transform _target;
    private DeathCaller _deathCaller;

    private void Start() {
        BindTarget(_targetFollow);
        transform.parent = null;
        _deathCaller = GetComponent<DeathCaller>();
    }

    public void BindTarget(Transform newTarget)
    {
        _target = newTarget;
    }
    
    private void Update() {
        if (_target == null)
        {
            Stop();
            return;
        }

        if (_offsetBinding)
        {
            transform.position = Vector3.Lerp(transform.position, _target.position + (_target.rotation * _targetOffset), _followSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _target.position + (transform.position - _target.position).normalized * _closingDistance, _followSpeed * Time.deltaTime);
        }

        if (_rotateToTarget)
            transform.up = _target.position - transform.position;
    }

    private void Stop()
    {
        if (_destroyWithTarget)
        {
            _deathCaller.DeathExplosion();
            Destroy(gameObject);
        }
    }
}
