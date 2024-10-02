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

    protected override void DoMove()
    {
        if (_player == null)
        {
            FindPlayer();
            return;
        }

        if (!_reposition)
        {
            if (((transform.position - GetActualPlayerPosition()).magnitude < (_closingDistance) && _retreating))
            {
                _reposition = true;
                _timer = SceneStatics.MultiplyByChaos(_repositionTime / Mobility);
                _secondaryOffset = SceneStatics.MultiplyByChaos(SceneStatics.ChaosMultiplier) * 2f;

                _targetPosition = GetActualPlayerPosition() + ((transform.position - GetActualPlayerPosition()).normalized * (_closingDistance / Mobility) * _retreatPower);
                _targetPosition += new Vector3(Random.Range(-_secondaryOffset, _secondaryOffset), Random.Range(-_secondaryOffset, _secondaryOffset), 0f);
                return;
            }

            _targetPosition = GetActualPlayerPosition() + ((transform.position - GetActualPlayerPosition()).normalized * (_closingDistance / Mobility));

            if (_linearMove && ((_targetPosition - transform.position).magnitude > _closingDistance + 0.2f))
                transform.position += (_targetPosition - transform.position).normalized * Speed * Time.deltaTime * Mobility;
            else
                transform.position = Vector3.Lerp(transform.position, _targetPosition, Speed * Time.deltaTime * Mobility);
        } else 
        {
            if (_timer <= 0)
            {
                _reposition = false;
                return;
            }

            transform.position = Vector3.Lerp(transform.position, _targetPosition, Speed * Time.deltaTime * Mobility);
        }
        
        _timer -= Time.deltaTime;
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
