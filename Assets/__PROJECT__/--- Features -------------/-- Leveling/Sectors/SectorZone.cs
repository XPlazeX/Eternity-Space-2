using UnityEngine;

public class SectorZone : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string zoneId;
    [SerializeField] private string displayName = "Зона 1";
    [Header("Navigation")]
    [SerializeField] private SectorZoneType zoneType = SectorZoneType.EnemyBase;
    [SerializeField] private float arriveRadius = 100f;
    [SerializeField] private bool isHidden = false;
    [SerializeField] private bool showDistance = true;
    [Header("Activation Rules")]
    [SerializeField] private SectorObjetsActivationRules activationRules;

    public string ZoneId { get => zoneId; private set => zoneId = value; }
    public string DisplayName { get => displayName; set => displayName = value; } // все остальное может менятся
    public SectorZoneType ZoneType { get => zoneType; set => zoneType = value; }
    public float ArriveRadius { get => arriveRadius; set => arriveRadius = value; }
    public bool IsHidden { get => isHidden; set => isHidden = value; }
    public bool ShowDistance { get => showDistance; set => showDistance = value; }
    
    public Vector3 Center => transform.position;
    public Transform CenterTransform => transform;

    public bool CheckPlayerInZone()
    {
        return (Player.Position - transform.position).magnitude <= arriveRadius;
    }

    public void UpdateActivationRules()
    {
        activationRules.Evaluate((Player.Position - transform.position).magnitude);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, arriveRadius);
    }
}

public enum SectorZoneType
{
    Unknown,
    EnemyBase,
    Signal,
    OtherStation,
    Debris
}
