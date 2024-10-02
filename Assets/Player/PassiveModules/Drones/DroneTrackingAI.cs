using UnityEngine;

public class DroneTrackingAI : TrackingAI
{
    [Space()]
    [SerializeField] private string _targetTag;
    [SerializeField] private bool _targetRefreshing = false;
    [SerializeField] private float _targetReload;

    private float _timerRefresh;

    protected override void Start()
    {
        // base.Start();
        StartMoving();
        _neverTargetToPlayer = true;
        _timerRefresh = _targetReload;
        // _trueOffset = _offset;
    }

    protected override void DoMove()
    {
        base.DoMove();

        if (!_targetRefreshing)
            return;
            
        _timerRefresh -= Time.deltaTime;

        if (_timerRefresh <= 0f)
        {
            _timerRefresh = _targetReload;
            FindPlayer();
        }
    }

    public override void FindPlayer()
    {
        _player = GetNearestTransformWithTag(_targetTag);
    }

    protected override void RotateToPlayer()
    {
        if (_player == null)
        {
            FindPlayer();
            return;
        }

        transform.up = SceneStatics.FlatVector(Vector3.RotateTowards(transform.up, (_player.position - transform.position), _rotationSpeed * Time.deltaTime * (Speed / _startSpeed) * Mobility, 0f));

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
