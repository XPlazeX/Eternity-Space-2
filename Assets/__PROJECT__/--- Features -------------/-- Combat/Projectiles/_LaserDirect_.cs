using UnityEngine;

public class _LaserDirect_ : MonoBehaviour
{
    private enum AxisMode
    {
        Yself_Xplayer = 0,
        Xself_Yplayer = 1,
        Xplayer_Yplayer = 2
    }

    [Header("Player Direct only")]
    [SerializeField] private float _directionFollowSpeed;
    [SerializeField] private AxisMode _axisMode;

    private float _homingMultiplier = 1f;

    private void OnEnable() {
        _homingMultiplier = ShipStats.GetValue("HomingEfficiencyMultiplier");
    }

    private void FixedUpdate() 
    {
        Vector3 targetPosition = Player.PlayerTransform.position;

        if (_axisMode == AxisMode.Yself_Xplayer)
        {
            targetPosition = new Vector3(targetPosition.x, transform.position.y, 0f);
        } 
        else if (_axisMode == AxisMode.Xself_Yplayer)
        {
            targetPosition = new Vector3(transform.position.x, targetPosition.y, 0f);
        }

        transform.position += (targetPosition - transform.position) * _directionFollowSpeed * _homingMultiplier * ESTime.worldDeltaTime;
    }
}
