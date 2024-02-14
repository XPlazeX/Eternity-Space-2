using UnityEngine;

public class RamAI : EnemyAIRoot
{
    [SerializeField] private float _ramDistance;
    [SerializeField] private float _ramReload;

    private float _passedWay = 0f;
    private float _distance = 0f;
    private float _timer = 0f;

    protected override void Start() {
        SetTarget();

        _distance = (_targetPosition - transform.position).magnitude;
        _timer = 0f;

        base.Start();
    }

    private void SetTarget()
    {
        _targetPosition = ((_player.position - transform.position).normalized * _ramDistance * Mobility) + transform.position;

        float secondaryOffset = SceneStatics.MultiplyByChaos(SceneStatics.ChaosMultiplier) * 5f;
        _targetPosition += new Vector3(Random.Range(-secondaryOffset, secondaryOffset), Random.Range(-secondaryOffset, secondaryOffset), 0f);
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
            _timer = SceneStatics.MultiplyByChaos(_ramReload / Mobility);
        }
    }
}
