using UnityEngine;

public class RotateAroundAI : EnemyAIRoot
{
    [Header("This AI always use Lerp function to closing the Player (Moving Progression will not using)")]
    [SerializeField] private float closingDistance;
    [SerializeField] private float rotationAroundSpeedMultiplier = 1f;

    private Transform _emptyTarget;
    private float _rotationDirection = -1f;

    protected override void Start() 
    {
        _emptyTarget = Instantiate(new GameObject(), _player.position + Vector3.up * closingDistance, Quaternion.identity, _player).transform;
        
        if (Random.Range(0, 2) == 1)
            _rotationDirection = 1f;
        
        _rotationDirection = SceneStatics.MultiplyByChaos(_rotationDirection);

        base.Start();
    }

    protected override Vector2 GetMoveDelta()
    {
        if (_emptyTarget != null)
        {
            _emptyTarget.RotateAround(GetActualPlayerPosition(), Vector3.forward, Speed * rotationAroundSpeedMultiplier * Time.fixedDeltaTime * _rotationDirection * Mobility);
            _targetPosition = _emptyTarget.position;
        }

        return Vector3.Lerp(AiPosition, _targetPosition, Speed * Time.fixedDeltaTime * Mobility) - AiPosition;
    }

    private void OnDestroy() {
        if (_emptyTarget != null)
            Destroy(_emptyTarget.gameObject);
    }

    protected Vector3 GetActualPlayerPosition()
    {
        return Player.GetPlayerPosition(MovementForesight);
    }
}
