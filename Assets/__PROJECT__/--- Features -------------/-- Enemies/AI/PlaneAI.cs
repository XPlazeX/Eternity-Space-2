using UnityEngine;

public class PlaneAI : EnemyAIRoot
{
    [Range(0, 1f)][SerializeField] private float upperBorderPercent;
    [Range(0, 1f)][SerializeField] private float downBorderPercent;
    [Space()]
    [SerializeField] private float timeToReloadTarget;

    private Vector2 XBorders => new Vector2(ArenaLocal.WNegX, ArenaLocal.WPosX);
    private Vector2 YBorders => new Vector2(ArenaLocal.WNegY + ArenaLocal.Height * downBorderPercent, ArenaLocal.WNegY + ArenaLocal.Height * upperBorderPercent);
    private float _passedWay = 0f;
    private float _distance = 0f;
    private float _timer = 0f;

    protected override void Start() 
    {
        SetTarget();

        _passedWay = 0f;
        _distance = (_targetPosition - AiPosition).magnitude;
        _timer = timeToReloadTarget;

        base.Start();
    }

    public override void StartMoving()
    {
        base.StartMoving();
        if (!autoStart)
            Start();
    }

    private void SetTarget()
    {
        _targetPosition = ArenaLocal.ArenaOrientation(new Vector3 (Random.Range(XBorders.x + ARENA_BORDERS_MOVING_OFFSET, XBorders.y - ARENA_BORDERS_MOVING_OFFSET),
            Random.Range(YBorders.x + ARENA_BORDERS_MOVING_OFFSET, YBorders.y - ARENA_BORDERS_MOVING_OFFSET), 0f));
    }

    protected override Vector2 GetMoveDelta()
    {
        float currentMoving = movingProgression.Evaluate(_passedWay / _distance) * Speed * Time.fixedDeltaTime * Mobility;
        _passedWay += currentMoving;

        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            SetTarget();
            _passedWay = 0f;
            _distance = (_targetPosition - AiPosition).magnitude;
            _timer = SceneStatics.MultiplyByChaos(timeToReloadTarget / Mobility);
        }

        return (_targetPosition - AiPosition).normalized * currentMoving;
    }
}
