using System.Collections;
using UnityEngine;

public class StaticGravityApporaching : MonoBehaviour
{
    public static float GravitationalConstant = 0.003f;

    [SerializeField] private float _gravityScale;
    [SerializeField] private string _targetTag;

    private Transform _target;

    private void FixedUpdate() {
        if (_target == null)
            _target = GetNearestTransformWithTag(_targetTag);

        if (_target == null || (_target.position - transform.position).magnitude <= 0.2f)
            return;

        transform.position += (_target.position - transform.position).normalized * (GravitationalConstant * _gravityScale) / Mathf.Pow((_target.position - transform.position).magnitude, 2);
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
