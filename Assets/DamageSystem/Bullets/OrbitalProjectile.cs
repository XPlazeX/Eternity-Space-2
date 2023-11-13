using UnityEngine;

public class OrbitalProjectile : MonoBehaviour
{
    [SerializeField] private Transform _parent;
    [SerializeField] private float _rotationSpeed;

    private float _nextRotation;
    private Vector3 _startOrientation;
    private float _parentDistance;
    
    public void Initialize(Transform parent, float rotationSpeed)
    {
        _parent = parent;
        _rotationSpeed = rotationSpeed;
    }

    private void Start() {
        _parentDistance = transform.localPosition.magnitude;
        transform.parent = null;   
        _startOrientation = transform.up; 
    }

    private void FixedUpdate()
    {
        if (_parent == null)
        {
            Death();
            return;
        }

        _nextRotation += _rotationSpeed * Time.deltaTime;
        transform.position = _parent.position + (Quaternion.Euler(0, 0, _nextRotation) * (_startOrientation * _parentDistance));
    }

    private void Death()
    {
        DeathCaller deathCaller = GetComponent<DeathCaller>();
        
        if (deathCaller != null)
            deathCaller.DeathExplosion();

        Destroy(gameObject);
    }
}
