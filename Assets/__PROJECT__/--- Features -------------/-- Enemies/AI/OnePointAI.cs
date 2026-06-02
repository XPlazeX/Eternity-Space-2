using UnityEngine;

public class OnePointAI : EnemyAIRoot
{
    [Space()]
    [Range(0f, 40f)][SerializeField] private float startAngleDeviation;
    [SerializeField] private float jumpPower;
    [SerializeField] private bool noSpreadJumpPower = false;

    private Vector3 _selectedDirection;
    private float _passedWay = 0f;

    protected override void Start() 
    {
        _selectedDirection = Quaternion.Euler(0, 0, Random.Range(-startAngleDeviation, startAngleDeviation)) * (ArenaLocal.Pivot.position - transform.position);
        if (!noSpreadJumpPower)
            jumpPower = SceneStatics.MultiplyByChaos(jumpPower);
        _targetPosition = AiPosition + (_selectedDirection.normalized * jumpPower);

        base.Start();
    }

    protected override Vector2 GetMoveDelta()
    {
        if ((_targetPosition - AiPosition).magnitude < 0.05f)
        {
            return Vector2.zero;
        }

        float currentMoving = movingProgression.Evaluate(_passedWay / jumpPower) * Speed * Time.fixedDeltaTime * Mobility;
        _passedWay += currentMoving;
        return _selectedDirection * currentMoving;
    }
}
