using UnityEngine;

public class DroneRamAI : RamAI
{
    [Space()]
    [SerializeField] private string _targetTag;

    public override void FindPlayer()
    {
        _player = GetNearestTransformWithTag(_targetTag);
    }

    protected override void CorrectRotation()
    {
        if (_player == null)
            FindPlayer();
    }

    protected override void SetTarget()
    {
        Transform target = GetNearestTransformWithTag(_targetTag);
        if (target == null)
        {
            _targetPosition = ((Player.PlayerTransform.position - transform.position).normalized * _ramDistance * Mobility) + transform.position;
        } else
        {
            _targetPosition = ((target.position - transform.position).normalized * _ramDistance * Mobility) + transform.position;
        }

        float secondaryOffset = SceneStatics.MultiplyByChaos(SceneStatics.ChaosMultiplier) * 5f;
        _targetPosition += new Vector3(Random.Range(-secondaryOffset, secondaryOffset), Random.Range(-secondaryOffset, secondaryOffset), 0f);
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
