using UnityEngine;

public class TrackingAI : EnemyAIRoot
{
    [Header("This AI always use Lerp function to closing the Player (Moving Progression will not using)")]
    [SerializeField] private float _closingDistance;
    [SerializeField] private bool _autoTargetPlayer = true;
    [Space()]
    [SerializeField] private bool _retreating = false;
    [SerializeField] private float _repositionTime = 1f;
    [SerializeField] private float _retreatPower = 2.5f;

    private bool _reposition = false;
    private float _timer = 0f;
    private float _secondaryOffset = 0f;

    protected override void Start() {
        if (!_autoTargetPlayer)
        {
            base._player = transform.parent;
            transform.parent = null;
        }

        base.Start();
    }

    protected override void DoMove()
    {
        if (!_reposition)
        {
            if (((transform.position - _player.position).magnitude < (_closingDistance) && _retreating))
            {
                _reposition = true;
                _timer = SceneStatics.MultiplyByChaos(_repositionTime / _mobility);
                _secondaryOffset = SceneStatics.MultiplyByChaos(SceneStatics.ChaosMultiplier) * 2f;

                _targetPosition = _player.position + ((transform.position - _player.position).normalized * (_closingDistance / _mobility) * _retreatPower);
                _targetPosition += new Vector3(Random.Range(-_secondaryOffset, _secondaryOffset), Random.Range(-_secondaryOffset, _secondaryOffset), 0f);
                return;
            }

            _targetPosition = _player.position + ((transform.position - _player.position).normalized * (_closingDistance / _mobility));
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Speed * Time.deltaTime * _mobility);
        } else 
        {
            if (_timer <= 0)
            {
                _reposition = false;
                return;
            }

            transform.position = Vector3.Lerp(transform.position, _targetPosition, Speed * Time.deltaTime * _mobility);
        }
        
        _timer -= Time.deltaTime;
    }
}
