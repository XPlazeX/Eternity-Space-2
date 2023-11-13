using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private bool _randomizeDirection = false;

    private Transform _transform;

    private void Start() {
        _transform = transform;
        if (!_randomizeDirection)
            return;
        if (Random.Range(0, 2) == 1)
            _rotationSpeed *= -1f;
    }

    private void FixedUpdate() {
        _transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
    }
}
