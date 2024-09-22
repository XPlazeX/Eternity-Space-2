using UnityEngine;

public class StaticGravityApporaching : MonoBehaviour
{
    public static float GravitationalConstant = 0.003f;

    [SerializeField] private float _gravityScale;
    [SerializeField] private string _targetTag;
    [SerializeField] private float _deadZone = 0.3f;
    [SerializeField] private float _deadZoneSpeed = 0.5f;
    [SerializeField] private float _dampen = 0.5f;
    [SerializeField] private bool _regularUpdateTarget;
    [SerializeField] private float _updateTime = 0.3f;

    private Transform _target;
    private Vector3 _targetPosition;
    private float _updateTimer;

    private void Update() 
    {
        if (_updateTimer < 0f)
        {
            _target = GetNearestTransformWithTag(_targetTag);
            _updateTimer = _updateTime;
        }

        if (_regularUpdateTarget)
            _updateTimer -= Time.deltaTime;
        
        if (_target == null)
            return;

        _targetPosition = transform.position;

        if ((_target.position - transform.position).magnitude <= _deadZone)
        {
            transform.position = Vector3.Lerp(transform.position, _target.position, _deadZoneSpeed * Time.deltaTime);
        }
        else
        {
            _targetPosition += (_target.position - transform.position).normalized * (GravitationalConstant * _gravityScale) / Mathf.Pow((_target.position - transform.position).magnitude, 2);
            transform.position = Vector3.Lerp(transform.position, _targetPosition, _dampen);
        }
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
