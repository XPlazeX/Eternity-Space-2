using UnityEngine;

public class DroneTurretAI : TurretStaticAI
{
    [Space()]
    [SerializeField] private string _targetTag;

    public override void FindPlayer()
    {
        GameObject[] allTargets = GameObject.FindGameObjectsWithTag(_targetTag);
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

        _player = nearestTarget;
    }
}
