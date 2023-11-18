using UnityEngine;

public class SupermovingAI : EnemyAIRoot
{
    [Range(0, 1f)][SerializeField] private float _upperBorderPercent;
    [Range(0, 1f)][SerializeField] private float _downBorderPercent;
    [Space()]
    [SerializeField] private float _rotationRadius;
    [SerializeField] private float _rotationAroundSpeedMultiplier = 1f;
    [SerializeField] private float _timeToReloadTarget;

    private Vector2 _XBorders;
    private Vector2 _YBorders;
    private float _passedWay = 0f;
    private float _distance = 0f;
    private bool _rotatingAround = false;
    private float _timer = 0f;
    private float _rotationDirection = -1f;

    private void SetTarget()
    {
        _targetPosition = new Vector3 (Random.Range(_XBorders.x + level_borders_moving_offset, _XBorders.y - level_borders_moving_offset),
            Random.Range(_YBorders.x + level_borders_moving_offset, _YBorders.y - level_borders_moving_offset), 0f);
        _passedWay = 0f;
        _distance = (_targetPosition - transform.position).magnitude;
    }

    protected override void Start() {
        _XBorders = new Vector2 (CameraController.Borders_xXyY.x, CameraController.Borders_xXyY.y);
        float ySize = -CameraController.Borders_xXyY.z + CameraController.Borders_xXyY.w;
        _YBorders = new Vector2 ( (ySize * _downBorderPercent - (ySize / 2f)),  (ySize * _upperBorderPercent - (ySize / 2f)));

        SetTarget();

        base.Start();
    }

    protected override void DoMove()
    {
        if (!_rotatingAround)
        {
            if ((transform.position - _targetPosition).magnitude <= _rotationRadius)
            {
                _rotatingAround = true;
                _timer = SceneStatics.MultiplyByChaos(_timeToReloadTarget / _mobility);
                _rotationDirection = -1f;

                if (Random.Range(0, 2) == 1)
                    _rotationDirection = 1f;

                _rotationDirection = SceneStatics.MultiplyByChaos(_rotationDirection);
                return;
            }

            float currentMoving = _movingProgression.Evaluate(_passedWay / _distance) * Speed * Time.deltaTime * _mobility;
            _passedWay += currentMoving;
            transform.position += ((_targetPosition - transform.position).normalized) * currentMoving;
        } else 
        {
            if (_timer <= 0)
            {
                _rotatingAround = false;
                SetTarget();
                return;
            }

            transform.RotateAround(_targetPosition, Vector3.forward, _rotationAroundSpeedMultiplier * Time.deltaTime * Speed * _rotationDirection * _mobility);
            transform.Rotate(0, 0, _rotationAroundSpeedMultiplier * Time.deltaTime * Speed * -_rotationDirection * _mobility);
            _timer -= Time.deltaTime;
        }
    }
}
