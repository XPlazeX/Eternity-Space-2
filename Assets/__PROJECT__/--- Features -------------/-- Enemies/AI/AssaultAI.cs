using UnityEngine;

public class AssaultAI : EnemyAIRoot
{
    [Header("This AI always use Lerp function to closing the Player (Moving Progression will not using)")]
    [SerializeField] private float _offsetDistance;
    [SerializeField] private float _timeToReloadCirclePoint;
    [SerializeField] private bool _autoTargetPlayer = true;

    private Vector3 _offset;
    private float _secondaryOffset = 0f;
    private float _timer = 0f;

    protected override void Start() 
    {
        if (!_autoTargetPlayer)
        {
            base._player = transform.parent;
            transform.parent = null;
        }

        SetOffset();
        
        _secondaryOffset = SceneStatics.MultiplyByChaos(SceneStatics.ChaosMultiplier);
        _timer = _timeToReloadCirclePoint;
        _offset += new Vector3(Random.Range(-_secondaryOffset, _secondaryOffset), Random.Range(-_secondaryOffset, _secondaryOffset), 0f);

        base.Start();
    }

    private void SetOffset()
    {
        Vector2 temp = Random.insideUnitCircle.normalized;
        _offset = new Vector3(temp.x, temp.y, 0f) * (_offsetDistance / Mobility);
    }

    protected override Vector2 GetMoveDelta()
    {
        if (_player == null)
        {
            _autoTargetPlayer = true;
            base.FindPlayer();
            return Vector2.zero;
        }

        _timer -= Time.fixedDeltaTime;

        if (_timer <= 0f)
        {
            SetOffset();
            _timer = SceneStatics.MultiplyByChaos((_timeToReloadCirclePoint / Mobility));
        }

        _targetPosition = GetActualPlayerPosition() + _offset;

        return Vector3.Lerp(transform.position, _targetPosition, Speed * Time.fixedDeltaTime * Mobility) - transform.position;
    }


    protected Vector3 GetActualPlayerPosition()
    {
        if (!_autoTargetPlayer)
        {
            return _player.position;
        } else
        {
            return Player.GetPlayerPosition(_foresight + MovementForesight);
        }
    }
}
