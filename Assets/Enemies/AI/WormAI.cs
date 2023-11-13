using UnityEngine;

public class WormAI : EnemyAIRoot
{
    [SerializeField] private AnimationCurve _rotationSpeedProgression;
    [Header("This AI always use RotateToTarget orientation")]
    [SerializeField] private float _phaseCycleTime;
    [SerializeField][Range(0, 1f)] private float _agressivePhasePercent;
    [SerializeField] private float _agressiveSpeedBoost;
    [Space()]
    [Range(0, 1f)][SerializeField] private float _upperBorderPercent;
    [Range(0, 1f)][SerializeField] private float _downBorderPercent;
    [Space()]
    [SerializeField] private float _timeToReloadTarget;

    private Vector2 _XBorders;
    private Vector2 _YBorders;
    private bool _aggresive = false;
    private float _timer = 0f;
    private float _targetingTimer = 0f;
    private float _phaseTime = 0f;
    private float _normalRotationSpeed = 0f;

    public float AgressiveTimePercent
    {
        get {
            return _agressivePhasePercent;
        }
        set{
            _agressivePhasePercent = Mathf.Clamp01(value);
        }
    }

    private void Awake() {
        _orientation = LookingOrientation.RotateToTarget;
    }

    protected override void Start() {
        _XBorders = new Vector2 (CameraController.Borders_xXyY.x, CameraController.Borders_xXyY.y);
        float ySize = -CameraController.Borders_xXyY.z + CameraController.Borders_xXyY.w;
        _YBorders = new Vector2 ( (ySize * _downBorderPercent - (ySize / 2f)),  (ySize * _upperBorderPercent - (ySize / 2f)));

        _timer = SceneStatics.MultiplyByChaos(_phaseCycleTime * (1f - AgressiveTimePercent));
        SetTarget();
        
        StartMoving();
    }

    protected override void DoMove()
    {
        if (!_aggresive)
        {
            _targetingTimer -= Time.deltaTime;
            if (_targetingTimer <= 0)
            {
                SetTarget();
                _targetingTimer = SceneStatics.MultiplyByChaos(_timeToReloadTarget / _mobility);
            }
            transform.position += transform.up.normalized * Speed * Time.deltaTime * _mobility;
        } else {
            _targetPosition = _player.position;
            _rotationSpeed = _normalRotationSpeed * _rotationSpeedProgression.Evaluate(1f - (_timer / _phaseTime));
            transform.position += transform.up.normalized * (Speed + _agressiveSpeedBoost) * Time.deltaTime * _movingProgression.Evaluate(1f - (_timer / _phaseTime)) * _mobility;
        }

        _timer -= Time.deltaTime;

        if (_timer < 0f)
        {
            _aggresive = !_aggresive;

            if (_aggresive)
            {
                _timer = SceneStatics.MultiplyByChaos(_phaseCycleTime * AgressiveTimePercent);
                _phaseTime = _timer;
                _normalRotationSpeed = _rotationSpeed * _mobility;
            } else {
                _timer = SceneStatics.MultiplyByChaos(_phaseCycleTime * (1f - AgressiveTimePercent));
                _rotationSpeed = _normalRotationSpeed;
                _targetingTimer = _timeToReloadTarget;
            }
        }
    }

    private void SetTarget()
    {
        _targetPosition = new Vector3 (Random.Range(_XBorders.x, _XBorders.y), Random.Range(_YBorders.x, _YBorders.y), 0f);
    }
}
