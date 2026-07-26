using UnityEngine;

public class LineAI : EnemyAIRoot
{
    [Header("Default orientation on camera size")]
    [SerializeField] private float circleAroundPlayerSizeBoost;
    [SerializeField] private Vector2 randomCircleSizeMultiplierBorders = Vector2.one;
    [SerializeField] private DeathCaller callerOnSpawn;
    [SerializeField] private DeathCaller callerOnDestroy;
    [SerializeField] private bool handRunValue;
    [SerializeField] private float runDistance;

    private float CircleSize => ArenaLocal.Height + (circleAroundPlayerSizeBoost / Mobility);
    private float _passedWay = 0f;
    private float _distance = 0f;

    protected override void Start() 
    {
        ReloadTarget();

        base.Start();
    }

    protected override Vector2 GetMoveDelta()
    {
        float currentMoving = movingProgression.Evaluate(_passedWay / _distance) * Speed * ESTime.worldFixedDeltaTime * Mobility;
        _passedWay += currentMoving;

        if (_distance - _passedWay <= 0.1f)
        {
            ReloadTarget();
        }

        return (_targetPosition - AiPosition).normalized * currentMoving;
    }

    private void ReloadTarget()
    {
        Vector3 _startPosition = ((Vector3)Random.insideUnitCircle.normalized) * Random.Range(CircleSize * randomCircleSizeMultiplierBorders.x, CircleSize * randomCircleSizeMultiplierBorders.y);

        _targetPosition = new Vector3(-_startPosition.x, -_startPosition.y, 0f);

        _startPosition += Player.PlayerTransform.position;
        _targetPosition += Player.PlayerTransform.position;

        // if (callerOnDestroy != null)
        //     callerOnDestroy.DeathExplosion();

        // if (callerOnSpawn != null)
        //     callerOnSpawn.DeathExplosion();

        if (handRunValue)
            _distance = SceneStatics.MultiplyByChaos(runDistance);
        else
            _distance = (_targetPosition - _startPosition).magnitude;

        _passedWay = 0f;
    }
}
