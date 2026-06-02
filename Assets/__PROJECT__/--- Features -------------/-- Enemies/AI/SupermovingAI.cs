using UnityEngine;

public class SupermovingAI : EnemyAIRoot
{
    [Range(0, 1f)][SerializeField] private float upperBorderPercent;
    [Range(0, 1f)][SerializeField] private float downBorderPercent;
    [Space()]
    [SerializeField] private float rotationRadius;
    [SerializeField] private float rotationAroundSpeedMultiplier = 1f;
    [SerializeField] private float timeToReloadTarget;

    private Vector2 XBorders => new Vector2(ArenaLocal.WNegX, ArenaLocal.WPosX);
    private Vector2 YBorders => new Vector2(ArenaLocal.WNegY + ArenaLocal.Height * downBorderPercent, ArenaLocal.WNegY + ArenaLocal.Height * upperBorderPercent);
    private float _passedWay = 0f;
    private float _distance = 0f;
    private bool _rotatingAround = false;
    private float _timer = 0f;
    private float _rotationDirection = -1f;

    private void SetTarget()
    {
        _targetPosition = ArenaLocal.ArenaOrientation(new Vector3 (Random.Range(XBorders.x + ARENA_BORDERS_MOVING_OFFSET, XBorders.y - ARENA_BORDERS_MOVING_OFFSET),
            Random.Range(YBorders.x + ARENA_BORDERS_MOVING_OFFSET, YBorders.y - ARENA_BORDERS_MOVING_OFFSET), 0f));
        _passedWay = 0f;
        _distance = (_targetPosition - AiPosition).magnitude;
    }

    protected override void Start() 
    {
        SetTarget();

        base.Start();
    }

    protected override Vector2 GetMoveDelta()
    {
        if (!_rotatingAround)
        {
            if ((AiPosition - _targetPosition).magnitude <= rotationRadius)
            {
                _rotatingAround = true;
                _timer = SceneStatics.MultiplyByChaos(timeToReloadTarget / Mobility);
                _rotationDirection = -1f;

                if (Random.Range(0, 2) == 1)
                    _rotationDirection = 1f;

                _rotationDirection = SceneStatics.MultiplyByChaos(_rotationDirection);
                return Vector2.zero;
            }

            float currentMoving = movingProgression.Evaluate(_passedWay / _distance) * Speed * Time.fixedDeltaTime * Mobility;
            _passedWay += currentMoving;
            return ((Vector2)(_targetPosition - AiPosition).normalized) * currentMoving;
        }
        else
        {
            if (_timer <= 0f)
            {
                _rotatingAround = false;
                SetTarget();
                return Vector2.zero;
            }

            float angleDelta = rotationAroundSpeedMultiplier * Time.fixedDeltaTime * Speed * _rotationDirection * Mobility;

            Vector2 currentPos = AiPosition;
            Vector2 offsetFromTarget = currentPos - (Vector2)_targetPosition;
            Vector2 rotatedOffset = Quaternion.Euler(0f, 0f, angleDelta) * offsetFromTarget;
            Vector2 nextPos = (Vector2)_targetPosition + rotatedOffset;

            _timer -= Time.fixedDeltaTime;
            return nextPos - currentPos;
        }
    }
}
