using UnityEngine;

public class DroneTrackingAI : TrackingAI
{
    [Space()]
    [SerializeField] private string targetTag;
    [SerializeField] private bool targetRefreshing = false;
    [SerializeField] private float targetReload;

    private float _timerRefresh;

    protected override void Start()
    {
        // base.Start();
        StartMoving();
        _neverTargetToPlayer = true;
        _timerRefresh = targetReload;
        // _trueOffset = _offset;
    }

    protected override Vector2 GetMoveDelta()
    {
        if (!targetRefreshing)
            return base.GetMoveDelta();
            
        _timerRefresh -= ESTime.worldDeltaTime;

        if (_timerRefresh <= 0f)
        {
            _timerRefresh = targetReload;
            FindPlayer();
        }

        return base.GetMoveDelta();
    }

    public override void FindPlayer()
    {
        _player = GetNearestTransformWithTag(targetTag);
    }

    protected override void RotateToPlayer()
    {
        if (_player == null)
        {
            FindPlayer();
            return;
        }

        transform.up = SceneStatics.FlatVector(Vector3.RotateTowards(transform.up, (_player.position - transform.position), rotationSpeed * ESTime.worldFixedDeltaTime * (Speed / _startSpeed) * Mobility, 0f));

        CorrectRotation();
    }

    // protected override void CorrectRotation()
    // {
    //     if (_player == null)
    //         FindPlayer();
    // }

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
