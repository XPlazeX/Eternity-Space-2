using UnityEngine;

public class RotateAroundAI : EnemyAIRoot
{
    [Header("This AI always use Lerp function to closing the Player (Moving Progression will not using)")]
    [SerializeField] private float _closingDistance;
    [SerializeField] private float _rotationAroundSpeedMultiplier = 1f;

    private Transform _emptyTarget;
    private float _rotationDirection = -1f;

    protected override void Start() {
        _emptyTarget = Instantiate(new GameObject(), _player.position + Vector3.up * (_closingDistance / _mobility), Quaternion.identity, _player).transform;
        
        if (Random.Range(0, 2) == 1)
            _rotationDirection = 1f;
        
        _rotationDirection = SceneStatics.MultiplyByChaos(_rotationDirection);

        base.Start();
    }

    protected override void DoMove()
    {
        if (_emptyTarget != null)
        {
            _emptyTarget.RotateAround(_player.position, Vector3.forward, Speed * _rotationAroundSpeedMultiplier * Time.deltaTime * _rotationDirection * _mobility);
            _targetPosition = _emptyTarget.position;
        }

        transform.position = Vector3.Lerp(transform.position, _targetPosition, Speed * Time.deltaTime * _mobility);
    }

    private void OnDestroy() {
        if (_emptyTarget != null)
            Destroy(_emptyTarget.gameObject);
    }
}
