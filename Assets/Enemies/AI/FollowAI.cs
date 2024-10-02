using System.Collections;
using UnityEngine;

public class FollowAI : EnemyAIRoot
{
    [SerializeField] private bool _useRetargeting = true;
    [SerializeField] private float _retargetDelay;
    [SerializeField] private float _retargetTime;

    private Vector2 _XBorders;
    private Vector2 _YBorders;
    private float _timer = 0f;
    private bool _retargeting = false;

    protected override void Start()
    {
        _XBorders = new Vector2 (CameraController.Borders_xXyY.x, CameraController.Borders_xXyY.y);
        float ySize = -CameraController.Borders_xXyY.z + CameraController.Borders_xXyY.w;
        _YBorders = new Vector2 ( -ySize,  ySize);

        _timer = SceneStatics.MultiplyByChaos((_retargetDelay / Mobility));

        base.Start();
    }

    protected override void DoMove()
    {
        if (_player == null)
            return;
        if (!_retargeting)
        {
            _targetPosition = GetActualPlayerPosition();
            if ((transform.position - _targetPosition).magnitude > 0.3f)
                transform.position += (_targetPosition - transform.position).normalized * Speed * Time.deltaTime * Mobility;
            else
            {
                transform.position = Vector3.Lerp(transform.position, _targetPosition, Speed * Time.deltaTime * Mobility);
            }
        } else 
        {
            transform.position += (_targetPosition - transform.position).normalized * Speed * Time.deltaTime * Mobility;
        }

        _timer -= Time.deltaTime;

        if (_timer < 0f && _useRetargeting)
        {
            _retargeting = !_retargeting;
            _timer = SceneStatics.MultiplyByChaos((_retargetDelay / Mobility));
            if (_retargeting)
                _targetPosition = new Vector3 (Random.Range(_XBorders.x, _XBorders.y), Random.Range(_YBorders.x, _YBorders.y), 0f);
        }
    }

    protected Vector3 GetActualPlayerPosition()
    {
        return Player.GetPlayerPosition(_foresight + MovementForesight);
    }
}
