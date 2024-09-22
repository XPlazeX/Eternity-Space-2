using UnityEngine;

public class OppositionAI : EnemyAIRoot
{
    [Header("This AI always use Lerp function to closing the Player (Moving Progression will not using)")]
    [SerializeField] protected Vector3 _offset;
    [SerializeField] private bool _autoTargetPlayer = true;
    [SerializeField] private bool _noiseOffset = true;
    [SerializeField] private bool _copyTargetRotation = true;
    [Header("Retreating")]
    [SerializeField] private bool _retreating = false;
    [SerializeField] private float _retreatTime;
    [SerializeField] private Vector2 _retreatOffsetXY;
    [Header("Latitude использует отступ Y от верхней границы и отменяет retreating")]
    [SerializeField] private bool _useLatitude;

    protected Vector3 _trueOffset;
    private float _timer = 0f;
    private float _latitudeY;
    protected bool _neverTargetToPlayer = false;

    protected override void Start() 
    {
        if (!_autoTargetPlayer && !_neverTargetToPlayer)
        {
            _player = transform.parent;
            transform.parent = null;
        }

        if (_useLatitude)
        {
            _retreating = false;
            _latitudeY = CameraController.Borders_xXyY.w - _offset.y;
        }
        
        if ((!_retreating) && (Random.Range(0, 2) == 1) && (_autoTargetPlayer))
            _offset = new Vector3(-_offset.x, _offset.y, 0f);

        Retreat();

        base.Start();
    }

    private void Retreat()
    {
        Vector2 retreatOffset = new Vector2();

        if (_noiseOffset)
            retreatOffset = (_retreatOffsetXY * Mobility) + new Vector2(SceneStatics.MultiplyByChaos(1f), SceneStatics.MultiplyByChaos(1f));
        else
            retreatOffset = (_retreatOffsetXY * Mobility);

        _trueOffset = _offset + new Vector3(Random.Range(-retreatOffset.x, retreatOffset.x), Random.Range(-retreatOffset.y, retreatOffset.y), 0);
    }

    protected override void DoMove()
    {
        if (_player == null)
        {
            if (!_neverTargetToPlayer)
            {
                _autoTargetPlayer = true;
                _orientation = LookingOrientation.RotateToPlayer;
                _offset = new Vector3(_offset.x, -_offset.y);
            }
            print("tfp");
            FindPlayer();
            
            Start();
            return;
        }
        if (_retreating && _timer <= 0)
        {
            Retreat();
            _timer = _retreatTime / Mobility;
        }

        if (!_useLatitude)
            _targetPosition = GetActualPlayerPosition() + ((_copyTargetRotation ? _player.rotation : Quaternion.identity) * _trueOffset);
        else
            _targetPosition = new Vector3(GetActualPlayerPosition().x, _latitudeY, 0f);

        transform.position = Vector3.Lerp(transform.position, _targetPosition, Speed * Time.deltaTime * Mobility);

        _timer -= Time.deltaTime;
    }

    protected Vector3 GetActualPlayerPosition()
    {
        if (!_autoTargetPlayer)
        {
            return _player.position;
        } else
        {
            return Player.GetPlayerPosition(_foresight);
        }
    }
}
