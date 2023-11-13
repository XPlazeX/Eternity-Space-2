using UnityEngine;

public class LineAI : EnemyAIRoot
{
    [Header("Default orientation on camera size")]
    [SerializeField] private float _circleAroundPlayerSizeBoost;
    [SerializeField] private Vector2 _randomCircleSizeMultiplierBorders = Vector2.one;
    [SerializeField] private DeathCaller _callerOnSpawn;
    [SerializeField] private DeathCaller _callerOnDestroy;
    [SerializeField] private bool _handRunValue;
    [SerializeField] private float _run_Distance;

    private float _circleSize;
    private float _passedWay = 0f;
    private float _distance = 0f;

    protected override void Start() {
        Camera.main.GetComponent<CameraController>().BordersChange += OnCameraSizeChange;
        OnCameraSizeChange(0);

        ReloadTarget();

        base.Start();
    }

    private void OnCameraSizeChange(float nul)
    {
        _circleSize = (CameraController.Size * 2f) + (_circleAroundPlayerSizeBoost / _mobility);
    }

    protected override void DoMove()
    {
        float currentMoving = _movingProgression.Evaluate(_passedWay / _distance) * Speed * Time.deltaTime * _mobility;
        _passedWay += currentMoving;
        transform.position += ((_targetPosition - transform.position).normalized) * currentMoving;

        if (_distance - _passedWay <= 0.1f)
        {
            ReloadTarget();
        }
    }

    private void ReloadTarget()
    {
        Vector3 _startPosition = (((Vector3)Random.insideUnitCircle.normalized) * Random.Range(_circleSize * _randomCircleSizeMultiplierBorders.x, _circleSize * _randomCircleSizeMultiplierBorders.y));

        _targetPosition = new Vector3(-_startPosition.x, -_startPosition.y, 0f);

        _startPosition += Player.PlayerTransform.position;
        _targetPosition += Player.PlayerTransform.position;

        if (_callerOnDestroy != null)
            _callerOnDestroy.DeathExplosion();

        transform.position = _startPosition;

        if (_callerOnSpawn != null)
            _callerOnSpawn.DeathExplosion();

        if (_handRunValue)
            _distance = SceneStatics.MultiplyByChaos(_run_Distance);
        else
            _distance = (_targetPosition - transform.position).magnitude;

        _passedWay = 0f;
    }
}
