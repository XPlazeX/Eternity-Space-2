using UnityEngine;

public class PlaneAI : EnemyAIRoot
{
    [Range(0, 1f)][SerializeField] private float _upperBorderPercent;
    [Range(0, 1f)][SerializeField] private float _downBorderPercent;
    [Space()]
    [SerializeField] private float _timeToReloadTarget;

    private Vector2 _XBorders;
    private Vector2 _YBorders;
    private float _passedWay = 0f;
    private float _distance = 0f;
    private float _timer = 0f;

    protected override void Start() {
        _XBorders = new Vector2 (CameraController.Borders_xXyY.x, CameraController.Borders_xXyY.y);
        float ySize = -CameraController.Borders_xXyY.z + CameraController.Borders_xXyY.w;
        _YBorders = new Vector2 ( (ySize * _downBorderPercent - (ySize / 2f)),  (ySize * _upperBorderPercent - (ySize / 2f)));

        SetTarget();

        _passedWay = 0f;
        _distance = (_targetPosition - transform.position).magnitude;
        _timer = _timeToReloadTarget;

        base.Start();
    }

    public override void StartMoving()
    {
        base.StartMoving();
        if (!_autoStart)
            Start();
    }

    private void SetTarget()
    {
        _targetPosition = new Vector3 (Random.Range(_XBorders.x + level_borders_moving_offset, _XBorders.y - level_borders_moving_offset),
            Random.Range(_YBorders.x + level_borders_moving_offset, _YBorders.y - level_borders_moving_offset), 0f);
    }

    protected override void DoMove()
    {
        float currentMoving = _movingProgression.Evaluate(_passedWay / _distance) * Speed * Time.deltaTime * Mobility;
        _passedWay += currentMoving;
        transform.position += ((_targetPosition - transform.position).normalized) * currentMoving;

        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            SetTarget();
            _passedWay = 0f;
            _distance = (_targetPosition - transform.position).magnitude;
            _timer = SceneStatics.MultiplyByChaos(_timeToReloadTarget / Mobility);
        }
    }
}
