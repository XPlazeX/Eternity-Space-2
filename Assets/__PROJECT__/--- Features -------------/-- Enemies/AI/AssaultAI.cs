using UnityEngine;

public class AssaultAI : EnemyAIRoot
{
    [Header("This AI always use Lerp function to closing the Player (Moving Progression will not using)")]
    [SerializeField] private float offsetDistance;
    [SerializeField] private float timeToReloadCirclePoint;
    [SerializeField] private bool autoTargetPlayer = true;

    private Vector3 _offset;
    private float _secondaryOffset = 0f;
    private float _timer = 0f;

    protected override void Start() 
    {
        if (!autoTargetPlayer)
        {
            base._player = transform.parent;
            transform.parent = null;
        }

        SetOffset();
        
        _secondaryOffset = SceneStatics.MultiplyByChaos(SceneStatics.ChaosMultiplier);
        _timer = timeToReloadCirclePoint;
        _offset += new Vector3(Random.Range(-_secondaryOffset, _secondaryOffset), Random.Range(-_secondaryOffset, _secondaryOffset), 0f);

        base.Start();
    }

    private void SetOffset()
    {
        Vector2 temp = Random.insideUnitCircle.normalized;
        _offset = new Vector3(temp.x, temp.y, 0f) * (offsetDistance / Mobility);
    }

    protected override Vector2 GetMoveDelta()
    {
        if (_player == null)
        {
            autoTargetPlayer = true;
            base.FindPlayer();
            return Vector2.zero;
        }

        _timer -= ESTime.worldFixedDeltaTime;

        if (_timer <= 0f)
        {
            SetOffset();
            _timer = SceneStatics.MultiplyByChaos((timeToReloadCirclePoint / Mobility));
        }

        _targetPosition = GetActualPlayerPosition() + _offset;

        return Vector3.Lerp(AiPosition, _targetPosition, Speed * ESTime.worldFixedDeltaTime * Mobility) - AiPosition;
    }


    protected Vector3 GetActualPlayerPosition()
    {
        if (!autoTargetPlayer)
        {
            return _player.position;
        } else
        {
            return Player.GetPlayerPosition(MovementForesight);
        }
    }
}
