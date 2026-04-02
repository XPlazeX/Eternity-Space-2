using UnityEngine;

public class SupermovingAI : EnemyAIRoot
{
    [Range(0, 1f)][SerializeField] private float _upperBorderPercent;
    [Range(0, 1f)][SerializeField] private float _downBorderPercent;
    [Space()]
    [SerializeField] private float _rotationRadius;
    [SerializeField] private float _rotationAroundSpeedMultiplier = 1f;
    [SerializeField] private float _timeToReloadTarget;

    private Vector2 XBorders => new Vector2(ArenaLocal.WNegX, ArenaLocal.WPosX);
    private Vector2 YBorders => new Vector2(ArenaLocal.WNegY + ArenaLocal.Height * _downBorderPercent, ArenaLocal.WNegY + ArenaLocal.Height * _upperBorderPercent);
    private float _passedWay = 0f;
    private float _distance = 0f;
    private bool _rotatingAround = false;
    private float _timer = 0f;
    private float _rotationDirection = -1f;

    private void SetTarget()
    {
        _targetPosition = new Vector3 (Random.Range(XBorders.x + level_borders_moving_offset, XBorders.y - level_borders_moving_offset),
            Random.Range(YBorders.x + level_borders_moving_offset, YBorders.y - level_borders_moving_offset), 0f);
        _passedWay = 0f;
        _distance = (_targetPosition - transform.position).magnitude;
    }

    protected override void Start() {
        // _XBorders = new Vector2 (ArenaLocal.W, CameraController.Borders_xXyY.y);
        // float ySize = -CameraController.Borders_xXyY.z + CameraController.Borders_xXyY.w;
        // _YBorders = new Vector2 ( (ySize * _downBorderPercent - (ySize / 2f)),  (ySize * _upperBorderPercent - (ySize / 2f)));

        SetTarget();

        base.Start();
    }

    protected override Vector2 GetMoveDelta()
    {
        if (!_rotatingAround)
        {
            if ((transform.position - _targetPosition).magnitude <= _rotationRadius)
            {
                _rotatingAround = true;
                _timer = SceneStatics.MultiplyByChaos(_timeToReloadTarget / Mobility);
                _rotationDirection = -1f;

                if (Random.Range(0, 2) == 1)
                    _rotationDirection = 1f;

                _rotationDirection = SceneStatics.MultiplyByChaos(_rotationDirection);
                return Vector2.zero;
            }

            float currentMoving = _movingProgression.Evaluate(_passedWay / _distance) * Speed * Time.fixedDeltaTime * Mobility;
            _passedWay += currentMoving;
            return ((Vector2)(_targetPosition - transform.position).normalized) * currentMoving;
        }
        else
        {
            if (_timer <= 0f)
            {
                _rotatingAround = false;
                SetTarget();
                return Vector2.zero;
            }

            float angleDelta = _rotationAroundSpeedMultiplier * Time.fixedDeltaTime * Speed * _rotationDirection * Mobility;

            Vector2 currentPos = transform.position;
            Vector2 offsetFromTarget = currentPos - (Vector2)_targetPosition;
            Vector2 rotatedOffset = Quaternion.Euler(0f, 0f, angleDelta) * offsetFromTarget;
            Vector2 nextPos = (Vector2)_targetPosition + rotatedOffset;

            _timer -= Time.fixedDeltaTime;
            return nextPos - currentPos;
        }
    }
}
