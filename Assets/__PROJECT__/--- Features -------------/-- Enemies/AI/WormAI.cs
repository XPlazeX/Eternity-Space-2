using UnityEngine;

public class WormAI : EnemyAIRoot
{
    [SerializeField] private bool useInnerRotation = false;
    [SerializeField] private float innerSpeedMultiplier = 1f;
    [SerializeField] private AnimationCurve rotationSpeedProgression;
    [Header("This AI always use RotateToTarget orientation")]
    [SerializeField] private float phaseCycleTime;
    [SerializeField][Range(0, 1f)] private float agressivePhasePercent;
    [SerializeField] private float agressiveSpeedBoost;
    [Space()]
    [Range(0, 1f)][SerializeField] private float upperBorderPercent;
    [Range(0, 1f)][SerializeField] private float downBorderPercent;
    [Space()]
    [SerializeField] private float timeToReloadTarget;

    private Vector2 XBorders => new Vector2(ArenaLocal.WNegX, ArenaLocal.WPosX);
    private Vector2 YBorders => new Vector2(ArenaLocal.WNegY + ArenaLocal.Height * downBorderPercent, -ArenaLocal.WNegY + ArenaLocal.Height * upperBorderPercent);
    private Vector3 _moveDirection;
    private bool _aggresive = false;
    private float _timer = 0f;
    private float _targetingTimer = 0f;
    private float _phaseTime = 0f;
    private float _normalRotationSpeed = 0f;

    public float AgressiveTimePercent
    {
        get {
            return agressivePhasePercent;
        }
        set{
            agressivePhasePercent = Mathf.Clamp01(value);
        }
    }

    protected override void Awake() 
    {
        base.Awake();
        _moveDirection = transform.up;
    }

    protected override void Start() 
    {
        _timer = SceneStatics.MultiplyByChaos(phaseCycleTime * (1f - AgressiveTimePercent));
        SetTarget();
        
        if (autoStart)
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
                _timer = SceneStatics.MultiplyByChaos(phaseCycleTime * AgressiveTimePercent);
                _phaseTime = _timer;
                _normalRotationSpeed = rotationSpeed * Mobility;
            } else {
                _timer = SceneStatics.MultiplyByChaos(phaseCycleTime * (1f - AgressiveTimePercent));
                rotationSpeed = _normalRotationSpeed;
                _targetingTimer = timeToReloadTarget;
            }
        }

        if (!_aggresive)
        {
            RotateMoveDirection(_targetPosition);
            _targetingTimer -= Time.fixedDeltaTime;
            if (_targetingTimer <= 0)
            {
                SetTarget();
                _targetingTimer = SceneStatics.MultiplyByChaos(timeToReloadTarget / Mobility);
            }
            return (useInnerRotation ? _moveDirection : transform.up).normalized * Speed * Time.fixedDeltaTime * Mobility;
        } else {
            RotateMoveDirection(GetActualPlayerPosition());
            _targetPosition = GetActualPlayerPosition();
            rotationSpeed = _normalRotationSpeed * rotationSpeedProgression.Evaluate(1f - (_timer / _phaseTime));
            return (useInnerRotation ? _moveDirection : transform.up).normalized * (Speed + agressiveSpeedBoost) * Time.fixedDeltaTime * movingProgression.Evaluate(1f - (_timer / _phaseTime)) * Mobility;
        }
    }

    private void SetTarget()
    {
        _targetPosition = new Vector3 (Random.Range(XBorders.x + ARENA_BORDERS_MOVING_OFFSET, XBorders.y - ARENA_BORDERS_MOVING_OFFSET),
            Random.Range(YBorders.x + ARENA_BORDERS_MOVING_OFFSET, YBorders.y - ARENA_BORDERS_MOVING_OFFSET), 0f);
    }

    private void RotateMoveDirection(Vector3 toPosition)
    {
        if (_player != null)
            _moveDirection = SceneStatics.FlatVector(Vector3.RotateTowards(_moveDirection, toPosition - AiPosition, rotationSpeed * Time.fixedDeltaTime * (Speed) * innerSpeedMultiplier * Mobility, 0f));
    }

    protected Vector3 GetActualPlayerPosition()
    {
        return Player.GetPlayerPosition(MovementForesight);
    }
}
