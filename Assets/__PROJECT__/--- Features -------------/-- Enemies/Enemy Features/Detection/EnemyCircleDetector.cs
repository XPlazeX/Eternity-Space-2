using UnityEngine;

public class EnemyCircleDetector : MonoBehaviour
{
    [Header("Отслеживает Sledge Position")]
    [SerializeField] private float detectionRadius = 100f;
    [SerializeField] private float detectionSpeed = 20f;
    [SerializeField] private float detectionFrequency = 4f;
    [SerializeField] private LineRenderer lineVisualizator;

    public Vector3 Center => transform.position;
    public float DetectionRadius => detectionRadius;

    private EnemyHQ _hq;
    private float _detectionTimer;
    private Transform _detectionTarget;

    private void Start() {
        _hq = FindAnyObjectByType<EnemyHQ>();

        if (lineVisualizator != null)
            lineVisualizator.SetPositions(new Vector3[2] {Vector3.zero, Vector3.up * detectionRadius});
    }

    void Update()
    {
        _detectionTimer -= ESTime.worldDeltaTime;

        if (_detectionTimer <= 0f)
        {
            UpdateDetectionTarget();
            _detectionTimer = 1f / detectionFrequency;
        }

        if (_detectionTarget != null)
        {
            _hq.AddWarning(detectionSpeed * ESTime.worldDeltaTime);
        }

        if (lineVisualizator != null)
            lineVisualizator.transform.up = SceneStatics.FlatVector((SledgeDirector.SledgePosition - transform.position).normalized);
    }

    private void UpdateDetectionTarget()
    {
        if ((SledgeDirector.SledgePosition - transform.position).magnitude <= detectionRadius)
        {
            _detectionTarget = SledgeDirector.SledgeTransform;
        }
        else
        {
            _detectionTarget = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
