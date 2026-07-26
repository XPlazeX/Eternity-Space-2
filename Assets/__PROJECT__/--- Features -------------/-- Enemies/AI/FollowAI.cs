using UnityEngine;

public class FollowAI : EnemyAIRoot
{
    [SerializeField] private bool useRetargeting = true;
    [SerializeField] private float retargetDelay;
    [SerializeField] private float retargetTime;

    private float _timer = 0f;
    private bool _retargeting = false;

    protected override void Start()
    {
        _timer = SceneStatics.MultiplyByChaos((retargetDelay / Mobility));

        base.Start();
    }

    protected override Vector2 GetMoveDelta()
    {
        if (_player == null)
            return Vector2.zero;

        _timer -= ESTime.worldFixedDeltaTime;

        if (_timer < 0f && useRetargeting)
        {
            _retargeting = !_retargeting;
            _timer = SceneStatics.MultiplyByChaos(retargetDelay / Mobility);
            if (_retargeting)
                _targetPosition = ArenaLocal.GetRandomFieldPosition();
        }

        if (!_retargeting)
        {
            _targetPosition = GetActualPlayerPosition();
            if ((AiPosition - _targetPosition).magnitude > 0.3f)
                return (_targetPosition - AiPosition).normalized * Speed * ESTime.worldFixedDeltaTime * Mobility;
            else
            {
                return Vector3.Lerp(AiPosition, _targetPosition, Speed * ESTime.worldFixedDeltaTime * Mobility) - AiPosition;
            }
        } else 
        {
            return (_targetPosition - AiPosition).normalized * Speed * ESTime.worldFixedDeltaTime * Mobility;
        }
    }

    protected Vector3 GetActualPlayerPosition()
    {
        return Player.GetPlayerPosition(MovementForesight);
    }
}
