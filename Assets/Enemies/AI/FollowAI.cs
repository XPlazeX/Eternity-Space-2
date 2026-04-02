using System.Collections;
using UnityEngine;

public class FollowAI : EnemyAIRoot
{
    [SerializeField] private bool _useRetargeting = true;
    [SerializeField] private float _retargetDelay;
    [SerializeField] private float _retargetTime;

    private float _timer = 0f;
    private bool _retargeting = false;

    protected override void Start()
    {
        _timer = SceneStatics.MultiplyByChaos((_retargetDelay / Mobility));

        base.Start();
    }

    protected override Vector2 GetMoveDelta()
    {
        if (_player == null)
            return Vector2.zero;

        _timer -= Time.fixedDeltaTime;

        if (_timer < 0f && _useRetargeting)
        {
            _retargeting = !_retargeting;
            _timer = SceneStatics.MultiplyByChaos(_retargetDelay / Mobility);
            if (_retargeting)
                _targetPosition = ArenaLocal.GetRandomFieldPosition();
        }

        if (!_retargeting)
        {
            _targetPosition = GetActualPlayerPosition();
            if ((transform.position - _targetPosition).magnitude > 0.3f)
                return (_targetPosition - transform.position).normalized * Speed * Time.fixedDeltaTime * Mobility;
            else
            {
                return Vector3.Lerp(transform.position, _targetPosition, Speed * Time.fixedDeltaTime * Mobility) - transform.position;
            }
        } else 
        {
            return (_targetPosition - transform.position).normalized * Speed * Time.fixedDeltaTime * Mobility;
        }
    }

    protected Vector3 GetActualPlayerPosition()
    {
        return Player.GetPlayerPosition(_foresight + MovementForesight);
    }
}
