using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private bool _randomizeDirection = false;
    [SerializeField][Range(1f, 5f)] private float _randomizeSpeed = 1f;
    [Space()]
    [SerializeField] private bool _pingPongRotation;
    [SerializeField] private float _pingPongSpeed;

    private Transform _transform;
    private float _targetRotationSpeed;
    private float _timer;

    private void Start() {
        _transform = transform;

        if (_randomizeDirection && Random.Range(0, 2) == 1)
            _rotationSpeed *= -1f;

        _rotationSpeed *= Random.Range(1f / _randomizeSpeed, _randomizeSpeed);

        _targetRotationSpeed = _rotationSpeed;
    }

    private void FixedUpdate() 
    {
        _transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);

        if (_pingPongRotation)
        {
            _rotationSpeed = _targetRotationSpeed * Mathf.Sin(_timer);

            _timer += Time.deltaTime * _pingPongSpeed;
        }
    }
}
