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

        _distance = (_targetPosition - transform.position).magnitude;
        _timer = 0f;

        base.Start();
    }

    protected virtual void SetTarget()
    {
        _targetPosition = ((GetActualPlayerPosition() - transform.position).normalized * _ramDistance * Mobility) + transform.position;

        float secondaryOffset = SceneStatics.MultiplyByChaos(SceneStatics.ChaosMultiplier) * 5f;
        _targetPosition += new Vector3(Random.Range(-secondaryOffset, secondaryOffset), Random.Range(-secondaryOffset, secondaryOffset), 0f);
    }

    protected override Vector2 GetMoveDelta()
    {
        float currentMoving = _movingProgression.Evaluate(_passedWay / _distance) * Speed * Time.fixedDeltaTime * Mobility;
        _passedWay += currentMoving;

        _timer -= Time.fixedDeltaTime;

        if (_timer <= 0f)
        {
            SetTarget();
            _passedWay = 0f;
            _distance = (_targetPosition - transform.position).magnitude;
            _timer = SceneStatics.MultiplyByChaos(_ramReload / Mobility);
        }

        return ((_targetPosition - transform.position).normalized) * currentMoving;
    }

    protected Vector3 GetActualPlayerPosition()
    {
        return Player.GetPlayerPosition(_foresight + MovementForesight);
    }
}
