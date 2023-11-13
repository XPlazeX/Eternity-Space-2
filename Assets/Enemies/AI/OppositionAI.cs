using UnityEngine;

public class OppositionAI : EnemyAIRoot
{
    [Header("This AI always use Lerp function to closing the Player (Moving Progression will not using)")]
    [SerializeField] private Vector3 _offset;
    [SerializeField] private bool _autoTargetPlayer = true;
    [SerializeField] private bool _noiseOffset = true;
    [Header("Retreating")]
    [SerializeField] private bool _retreating = false;
    [SerializeField] private float _retreatTime;
    [SerializeField] private Vector2 _retreatOffsetXY;

    private Vector3 _trueOffset;
    private float _timer = 0f;

    protected override void Start() 
    {
        if (!_autoTargetPlayer)
        {
            _player = transform.parent;
            transform.parent = null;
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
            retreatOffset = (_retreatOffsetXY * _mobility) + new Vector2(SceneStatics.MultiplyByChaos(1f), SceneStatics.MultiplyByChaos(1f));
        else
            retreatOffset = (_retreatOffsetXY * _mobility);

        _trueOffset = _offset + new Vector3(Random.Range(-retreatOffset.x, retreatOffset.x), Random.Range(-retreatOffset.y, retreatOffset.y), 0);
    }

    protected override void DoMove()
    {
        if (_player == null)
        {
            _autoTargetPlayer = true;
            base.FindPlayer();
            _offset = new Vector3(_offset.x, -_offset.y);
            _orientation = LookingOrientation.RotateToPlayer;
            
            Start();
            return;
        }
        if (_retreating && _timer <= 0)
        {
            Retreat();
            _timer = _retreatTime / _mobility;
        }

        _targetPosition = _player.position + (_player.rotation * _trueOffset);

        transform.position = Vector3.Lerp(transform.position, _targetPosition, Speed * Time.deltaTime * _mobility);

        _timer -= Time.deltaTime;
    }
}
