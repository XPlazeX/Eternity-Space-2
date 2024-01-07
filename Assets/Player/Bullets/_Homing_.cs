using UnityEngine;

public class _Homing_ : MonoBehaviour
{
    [SerializeField] private string _targetTag;
    [SerializeField] private float _homingPower;
    [SerializeField] private float _waitTime = 0f;

    private Transform _target;
    private float _checkReloadTimer;
    private float _waitTimer;

    public string TargetTag => _targetTag;
    public float HomingPower => _homingPower; 

    private void OnEnable() {
        _checkReloadTimer = 0f;
        _waitTimer = _waitTime;
    }

    public void ModParams(string targetTag, float homingPower)
    {
        _targetTag = targetTag;
        _homingPower = homingPower;
    }

    private void FixedUpdate() {
        _waitTimer -= Time.deltaTime;

        if (_waitTimer > 0f)
            return;
            
        _checkReloadTimer -= Time.deltaTime;
        if (_checkReloadTimer <= 0)
        {
            _target = GetNearestTransformWithTag(_targetTag);
            _checkReloadTimer = 1f;
        }
        if (_target)
            transform.up = SceneStatics.FlatVector(Vector3.RotateTowards(transform.up, _target.position - transform.position, _homingPower * Time.deltaTime, 0f));
    }

    private Transform GetNearestTransformWithTag(string targetTag)
    {
        GameObject[] allTargets = GameObject.FindGameObjectsWithTag(targetTag);
        Transform nearestTarget = null;
        float minDistance = 10000f;

        for (int i = 0; i < allTargets.Length; i++)
        {
            float distance = (allTargets[i].transform.position - transform.position).magnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = allTargets[i].transform;
            }
        }

        return nearestTarget;
    }
}
