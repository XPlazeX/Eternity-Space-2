using UnityEngine;

public class DronePlaneAI : PlaneAI
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
