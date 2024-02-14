using UnityEngine;

public class OnePointAI : EnemyAIRoot
{
    [Space()]
    [Range(0f, 40f)][SerializeField] private float _startAngleDeviation;
    [SerializeField] private float _jumpPower;
    [SerializeField] private bool _noSpreadJumpPower = false;

    private Vector3 _selectedDirection;
    private float _passedWay = 0f;

    protected override void Start() 
    {
        _selectedDirection = Quaternion.Euler(0, 0, Random.Range(-_startAngleDeviation, _startAngleDeviation)) * transform.up;
        if (!_noSpreadJumpPower)
            _jumpPower = SceneStatics.MultiplyByChaos(_jumpPower);
        _targetPosition = transform.position + (_selectedDirection.normalized * _jumpPower);

        base.Start();
    }

    protected override void DoMove()
    {
        if ((_targetPosition - transform.position).magnitude < 0.05f)
        {
            return;
        }

        float currentMoving = _movingProgression.Evaluate(_passedWay / _jumpPower) * Speed * Time.deltaTime * Mobility;
        transform.position += _selectedDirection * currentMoving;
        _passedWay += currentMoving;
    }
}
