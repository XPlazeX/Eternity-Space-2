using UnityEngine;

public class RamAI : EnemyAIRoot
{
    [SerializeField] protected float _ramDistance;
    [SerializeField] protected float _ramReload;

    private float _passedWay = 0f;
    private float _distance = 0f;
    private float _timer = 0f;

    protected override void Start() {
        SetTarget();

        _distance = (_targetPosition - AiPosition).magnitude;
        _timer = 0f;

        base.Start();
    }

    protected virtual void SetTarget()
    {
        _targetPosition = ((GetActualPlayerPosition() - AiPosition).normalized * _ramDistance * Mobility) + AiPosition;

        float secondaryOffset = SceneStatics.MultiplyByChaos(SceneStatics.ChaosMultiplier) * 5f;
        _targetPosition += new Vector3(Random.Range(-secondaryOffset, secondaryOffset), Random.Range(-secondaryOffset, secondaryOffset), 0f);
    }

    protected override Vector2 GetMoveDelta()
    {
        float currentMoving = movingProgression.Evaluate(_passedWay / _distance) * Speed * ESTime.worldFixedDeltaTime * Mobility;
        _passedWay += currentMoving;

        _timer -= ESTime.worldFixedDeltaTime;

        if (_timer <= 0f)
        {
            SetTarget();
            _passedWay = 0f;
            _distance = (_targetPosition - AiPosition).magnitude;
            _timer = SceneStatics.MultiplyByChaos(_ramReload / Mobility);
        }

        return (_targetPosition - AiPosition).normalized * currentMoving;
    }

    protected Vector3 GetActualPlayerPosition()
    {
        return Player.GetPlayerPosition(MovementForesight);
    }
}
