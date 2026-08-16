using UnityEngine;

public class _Homing_ : MonoBehaviour
{
    [SerializeField] private string _targetTag;
    [SerializeField] private float _maxHomingDistance = 500f;
    [SerializeField] private float _homingPower;
    [SerializeField] private float _homingBoostOverTime = 0f;
    [SerializeField] private float _maxHomingPower;
    [SerializeField] private float _waitTime = 0f;
    [SerializeField] private float _checkReloadTime = 1f;
    [SerializeField] private bool _playerIfNullTarget = false;

    private Transform _target;
    private float _checkReloadTimer;
    private float _waitTimer;
    private float _homingMultiplier = 1f;
    private float _homingBoost = 0f;

    public string TargetTag => _targetTag;
    public float HomingPower => _homingPower + _homingBoost; 

    private void OnEnable() {
        _checkReloadTimer = 0f;
        _waitTimer = _waitTime;
        _homingBoost = 0f;
        _homingMultiplier = ShipStats.GetValue("HomingEfficiencyMultiplier");
    }

    public void ModParams(string targetTag, float homingPower)
    {
        _targetTag = targetTag;
        _homingPower = homingPower;
    }

    private void FixedUpdate() {
        _waitTimer -= ESTime.worldDeltaTime;

        if (_waitTimer > 0f)
            return;

        _homingBoost = Mathf.Clamp(_homingBoost + _homingBoostOverTime * ESTime.worldFixedDeltaTime, 0f, _maxHomingPower);
            
        _checkReloadTimer -= ESTime.worldDeltaTime;
        if (_checkReloadTimer <= 0)
        {
            _target = GetNearestTransformWithTag(_targetTag);
            _checkReloadTimer = _checkReloadTime;
        }
        if (_target)
            transform.up = SceneStatics.FlatVector(Vector3.RotateTowards(transform.up, _target.position - transform.position, HomingPower * ESTime.worldDeltaTime * _homingMultiplier, 0f));
        else if (_playerIfNullTarget)
        {
            transform.up = SceneStatics.FlatVector(Vector3.RotateTowards(transform.up, Player.PlayerTransform.position - transform.position, HomingPower * ESTime.worldDeltaTime * _homingMultiplier, 0f));
        }
    }

    private Transform GetNearestTransformWithTag(string targetTag)
    {
        GameObject[] allTargets = GameObject.FindGameObjectsWithTag(targetTag);
        Transform nearestTarget = null;
        float minDistance = 10000f;
        bool trapped = false;

        for (int i = 0; i < allTargets.Length; i++)
        {
            float distance = (allTargets[i].transform.position - transform.position).magnitude;
            if (distance > _maxHomingDistance) continue;

            HomingTrap trap = allTargets[i].GetComponent<HomingTrap>();
            if (trap != null && trap.Active)
            {
                if (!trapped)
                {
                    nearestTarget = allTargets[i].transform;
                    continue;
                }
            } else if (trap == null && trapped)
            {
                continue;
            }
            
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = allTargets[i].transform;
            }
        }

        return nearestTarget;
    }
}
