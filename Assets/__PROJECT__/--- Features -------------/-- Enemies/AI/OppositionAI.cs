using UnityEngine;

public class OppositionAI : EnemyAIRoot
{
    [Header("This AI always use Lerp function to closing the Player (Moving Progression will not using)")]
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] protected Vector3 offset;
    [SerializeField] private bool autoTargetPlayer = true;
    [SerializeField] private bool noiseOffset = true;
    [SerializeField] private bool copyTargetRotation = true;
    [Header("Retreating")]
    [SerializeField] private bool retreating = false;
    [SerializeField] private float retreatTime;
    [SerializeField] private Vector2 retreatOffsetXY;
    [Header("Latitude использует отступ Y от верхней границы и отменяет retreating")]
    [SerializeField] private bool useLatitude;

    protected Vector3 _trueOffset;
    private float _timer = 0f;
    private float LatitudeY => ArenaLocal.WPosY - offset.y;
    protected bool _neverTargetToPlayer = false;

    protected override void Start() 
    {
        if (!autoTargetPlayer && !_neverTargetToPlayer)
        {
            _player = transform.parent;
            transform.parent = null;
        }

        if (useLatitude)
        {
            retreating = false;
        }
        
        if ((!retreating) && (Random.Range(0, 2) == 1) && (autoTargetPlayer))
            offset = new Vector3(-offset.x, offset.y, 0f);

        Retreat();

        base.Start();
    }

    private void Retreat()
    {
        Vector2 retreatOffset = new Vector2();

        if (noiseOffset)
            retreatOffset = (retreatOffsetXY * Mobility) + new Vector2(SceneStatics.MultiplyByChaos(1f), SceneStatics.MultiplyByChaos(1f));
        else
            retreatOffset = retreatOffsetXY * Mobility;

        _trueOffset = offset + new Vector3(Random.Range(-retreatOffset.x, retreatOffset.x), Random.Range(-retreatOffset.y, retreatOffset.y), 0);
    }

    protected override Vector2 GetMoveDelta()
    {
        if (_player == null)
        {
            if (!_neverTargetToPlayer)
            {
                autoTargetPlayer = true;
                lookingOrientation = LookingOrientation.RotateToPlayer;
                offset = new Vector3(offset.x, offset.y);
            }
            FindPlayer();
            
            Start();
            return Vector2.zero;
        }
        if (retreating && _timer <= 0)
        {
            Retreat();
            _timer = retreatTime / Mobility;
        }

        if (!useLatitude)
            _targetPosition = GetActualPlayerPosition() + ((copyTargetRotation ? _player.rotation : Quaternion.identity) * _trueOffset);
        else
            _targetPosition = new Vector3(GetActualPlayerPosition().x, LatitudeY, 0f);

        _timer -= Time.deltaTime;

        Vector3 result = (Vector3.Lerp(AiPosition, _targetPosition, Speed * Time.deltaTime * Mobility) - AiPosition);
        if (result.magnitude > maxSpeed * Time.deltaTime)
        {
            result = result.normalized * (maxSpeed * Time.deltaTime);
        }

        return result;
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
