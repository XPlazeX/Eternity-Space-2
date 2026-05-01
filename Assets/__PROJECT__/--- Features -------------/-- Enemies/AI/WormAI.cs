using UnityEngine;

public class WormAI : EnemyAIRoot
{
    [SerializeField] private bool _useInnerRotation = false;
    [SerializeField] private float _innerSpeedMultiplier = 1f;
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

    private Vector2 XBorders => new Vector2(ArenaLocal.WNegX, ArenaLocal.WPosX);
    private Vector2 YBorders => new Vector2(ArenaLocal.WNegY + ArenaLocal.Height * _downBorderPercent, -ArenaLocal.WNegY + ArenaLocal.Height * _upperBorderPercent);
    private Vector3 _moveDirection;
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
        //_orientation = LookingOrientation.RotateToTarget;
        _moveDirection = transform.up;
    }

    protected override void Start() {
        // _XBorders = new Vector2 (CameraController.Borders_xXyY.x, CameraController.Borders_xXyY.y);
        // float ySize = -CameraController.Borders_xXyY.z + CameraController.Borders_xXyY.w;
        // _YBorders = new Vector2 ( (ySize * _downBorderPercent - (ySize / 2f)),  (ySize * _upperBorderPercent - (ySize / 2f)));

        _timer = SceneStatics.MultiplyByChaos(_phaseCycleTime * (1f - AgressiveTimePercent));
        SetTarget();
        
        if (_autoStart)
            StartMoving();
    }

    protected override Vector2 GetMoveDelta()
    {
        _timer -= Time.fixedDeltaTime;

        if (_timer < 0f)
        {
            _aggresive = !_aggresive;

            if (_aggresive)
            {
                _timer = SceneStatics.MultiplyByChaos(_phaseCycleTime * AgressiveTimePercent);
                _phaseTime = _timer;
                _normalRotationSpeed = _rotationSpeed * Mobility;
            } else {
                _timer = SceneStatics.MultiplyByChaos(_phaseCycleTime * (1f - AgressiveTimePercent));
                _rotationSpeed = _normalRotationSpeed;
                _targetingTimer = _timeToReloadTarget;
            }
        }

        if (!_aggresive)
        {
            RotateMoveDirection(_targetPosition);
            _targetingTimer -= Time.fixedDeltaTime;
            if (_targetingTimer <= 0)
            {
                SetTarget();
                _targetingTimer = SceneStatics.MultiplyByChaos(_timeToReloadTarget / Mobility);
            }
            return (_useInnerRotation ? _moveDirection : transform.up).normalized * Speed * Time.fixedDeltaTime * Mobility;
        } else {
            RotateMoveDirection(GetActualPlayerPosition());
            _targetPosition = GetActualPlayerPosition();
            _rotationSpeed = _normalRotationSpeed * _rotationSpeedProgression.Evaluate(1f - (_timer / _phaseTime));
            return (_useInnerRotation ? _moveDirection : transform.up).normalized * (Speed + _agressiveSpeedBoost) * Time.fixedDeltaTime * _movingProgression.Evaluate(1f - (_timer / _phaseTime)) * Mobility;
        }
    }

    private void SetTarget()
    {
        _targetPosition = new Vector3 (Random.Range(XBorders.x + level_borders_moving_offset, XBorders.y - level_borders_moving_offset),
            Random.Range(YBorders.x + level_borders_moving_offset, YBorders.y - level_borders_moving_offset), 0f);
    }

    private void RotateMoveDirection(Vector3 toPosition)
    {
        if (_player != null)
            _moveDirection = SceneStatics.FlatVector(Vector3.RotateTowards(_moveDirection, (toPosition - transform.position), _rotationSpeed * Time.fixedDeltaTime * (Speed) * _innerSpeedMultiplier * Mobility, 0f));

        // if (transform.rotation.eulerAngles.y != 180 && transform.rotation .eulerAngles.y != -180)
        //     return;

        // transform.rotation = Quaternion.Euler(0, 0, 180);

        //CorrectRotation();
    }

    protected Vector3 GetActualPlayerPosition()
    {
        return Player.GetPlayerPosition(_foresight + MovementForesight);
    }
}
