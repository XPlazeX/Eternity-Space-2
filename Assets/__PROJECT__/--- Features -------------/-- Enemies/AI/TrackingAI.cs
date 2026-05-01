using UnityEngine;

public class TrackingAI : EnemyAIRoot
{
    [Header("This AI always use Lerp function to closing the Player (Moving Progression will not using)")]
    [SerializeField] private float _closingDistance;
    [SerializeField] private bool _autoTargetPlayer = true;
    [SerializeField] private bool _linearMove = false;
    [Space()]
    [SerializeField] private bool _retreating = false;
    [SerializeField] private float _repositionTime = 1f;
    [SerializeField] private float _retreatPower = 2.5f;

    private bool _reposition = false;
    private float _timer = 0f;
    private float _secondaryOffset = 0f;
    protected bool _neverTargetToPlayer = false;

    protected override void Start() {
        if (!_autoTargetPlayer && !_neverTargetToPlayer)
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
            if (((transform.position - GetActualPlayerPosition()).magnitude < (_closingDistance) && _retreating))
            {
                _reposition = true;
                _timer = SceneStatics.MultiplyByChaos(_repositionTime / Mobility);
                _secondaryOffset = SceneStatics.MultiplyByChaos(SceneStatics.ChaosMultiplier) * 2f;

                _targetPosition = GetActualPlayerPosition() + ((transform.position - GetActualPlayerPosition()).normalized * (_closingDistance / Mobility) * _retreatPower);
                _targetPosition += new Vector3(Random.Range(-_secondaryOffset, _secondaryOffset), Random.Range(-_secondaryOffset, _secondaryOffset), 0f);
                return Vector2.zero;
            }

            _targetPosition = GetActualPlayerPosition() + ((transform.position - GetActualPlayerPosition()).normalized * (_closingDistance / Mobility));

            if (_linearMove && ((_targetPosition - transform.position).magnitude > _closingDistance + 0.2f))
                return (_targetPosition - transform.position).normalized * Speed * Time.fixedDeltaTime * Mobility;
            else
                return Vector3.Lerp(transform.position, _targetPosition, Speed * Time.fixedDeltaTime * Mobility) - transform.position;
        } else 
        {
            if (_timer <= 0)
            {
                _reposition = false;
                return Vector2.zero;
            }

            return Vector3.Lerp(transform.position, _targetPosition, Speed * Time.fixedDeltaTime * Mobility) - transform.position;
        } 
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
