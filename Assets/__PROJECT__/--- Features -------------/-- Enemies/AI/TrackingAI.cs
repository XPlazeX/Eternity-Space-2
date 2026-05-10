using UnityEngine;

public class TrackingAI : EnemyAIRoot
{
    [Header("This AI always use Lerp function to closing the Player (Moving Progression will not using)")]
    [SerializeField] private float closingDistance;
    [SerializeField] private bool autoTargetPlayer = true;
    [SerializeField] private bool linearMove = false;
    [Space()]
    [SerializeField] private bool retreating = false;
    [SerializeField] private float repositionTime = 1f;
    [SerializeField] private float retreatPower = 2.5f;

    private bool _reposition = false;
    private float _timer = 0f;
    private float _secondaryOffset = 0f;
    protected bool _neverTargetToPlayer = false;

    protected override void Start() 
    {
        if (!autoTargetPlayer && !_neverTargetToPlayer)
        {
            base._player = transform.parent;
            transform.parent = null;
        }

        base.Start();
    }

    protected override Vector2 GetMoveDelta()
    {
        if (_player == null)
        {
            FindPlayer();
            return Vector2.zero;
        }

        _timer -= Time.fixedDeltaTime;

        if (!_reposition)
        {
            if ((AiPosition - GetActualPlayerPosition()).magnitude < (closingDistance) && retreating)
            {
                _reposition = true;
                _timer = SceneStatics.MultiplyByChaos(repositionTime / Mobility);
                _secondaryOffset = SceneStatics.MultiplyByChaos(SceneStatics.ChaosMultiplier) * 2f;

                _targetPosition = GetActualPlayerPosition() + ((AiPosition - GetActualPlayerPosition()).normalized * (closingDistance / Mobility) * retreatPower);
                _targetPosition += new Vector3(Random.Range(-_secondaryOffset, _secondaryOffset), Random.Range(-_secondaryOffset, _secondaryOffset), 0f);
                return Vector2.zero;
            }

            _targetPosition = GetActualPlayerPosition() + ((AiPosition - GetActualPlayerPosition()).normalized * (closingDistance / Mobility));

            if (linearMove && ((_targetPosition - AiPosition).magnitude > closingDistance + 0.2f))
                return (_targetPosition - AiPosition).normalized * Speed * Time.fixedDeltaTime * Mobility;
            else
                return Vector3.Lerp(AiPosition, _targetPosition, Speed * Time.fixedDeltaTime * Mobility) - AiPosition;
        } else 
        {
            if (_timer <= 0)
            {
                _reposition = false;
                return Vector2.zero;
            }

            return Vector3.Lerp(AiPosition, _targetPosition, Speed * Time.fixedDeltaTime * Mobility) - AiPosition;
        } 
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
