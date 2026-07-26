using System.Collections.Generic;
using UnityEngine;

public class SectorRoot : MonoBehaviour
{
    public static event System.Action<string> NewSectorZoneVisited;
    public static event System.Action<string> SectorZoneEntered;
    public static event System.Action<string> SectorZoneExited;
    public static event System.Action<string> SectorZoneUpdated;

    [Header("Global")]
    [SerializeField] private Transform center;
    [SerializeField] private Vector3 siriusDirection = new Vector3(-1f, 1f, 0f);
    [Header("Sector Zones")]
    [SerializeField] private List<SectorZone> sectorZones = new List<SectorZone>();
    [SerializeField] private float sectorZoneStatusUpdateTime = 0.5f;
    [SerializeField] private float sectorZoneObjectsActivationUpdateTime = 1f;
    [Header("Sector Borders")]
    [SerializeField] private float safeRadius = 5000f;
    [SerializeField] private float warningRadius = 6000f;
    [SerializeField] private float arriveRadius = 4000f;

    private SectorRuntimeState _sectorRuntimeState = new SectorRuntimeState();
    private PlayerInSectorSafetyStatus _playerInSectorStatus = PlayerInSectorSafetyStatus.Safe;
    private float _statusUpdateTimer = 0f;
    private float _pendingStatusTime = 0f;
    private float _activationUpdateTimer = 0f;

    public Vector3 Center => center == null ? Vector3.zero : center.position;
    public Vector3 Up => siriusDirection.normalized;
    public Vector3 Down => Quaternion.Euler(0f, 0f, 180f) * siriusDirection.normalized;
    public Vector3 Left => Quaternion.Euler(0f, 0f, 90f) * siriusDirection.normalized;
    public Vector3 Right => Quaternion.Euler(0f, 0f, 270f) * siriusDirection.normalized;
    public float ArriveRadius => arriveRadius;

    public PlayerInSectorSafetyStatus PlayerInSectorSafetyStatus
    {
        get
        {
            if (_playerInSectorStatus == PlayerInSectorSafetyStatus.ImpendingDoom)
                return PlayerInSectorSafetyStatus.ImpendingDoom;

            else if ((Player.Position - center.position).magnitude <= safeRadius)
            {
                _playerInSectorStatus = PlayerInSectorSafetyStatus.Safe;
                return PlayerInSectorSafetyStatus.Safe;
            } else if ((Player.Position - center.position).magnitude <= warningRadius)
            {
                _playerInSectorStatus = PlayerInSectorSafetyStatus.Warning;
                return PlayerInSectorSafetyStatus.Warning;
            } else
            {
                _playerInSectorStatus = PlayerInSectorSafetyStatus.ImpendingDoom;
                return PlayerInSectorSafetyStatus.ImpendingDoom;
            }
        }
    }

    void OnEnable()
    {
        Map.SetCurrentSector(this);
    }

    public bool IsSectorZoneVizited(string zoneID)
    {
        return _sectorRuntimeState.visitedSectors.Contains(zoneID);
    }

    void Update()
    {
        _statusUpdateTimer -= ESTime.worldDeltaTime;
        _pendingStatusTime += ESTime.worldDeltaTime;

        if (_statusUpdateTimer < 0f)
        {
            UpdateRuntimeState(_pendingStatusTime);
            _pendingStatusTime = 0f;
            _statusUpdateTimer = sectorZoneStatusUpdateTime;
        }

        _activationUpdateTimer -= ESTime.worldDeltaTime;

        if (_activationUpdateTimer < 0f)
        {
            UpdateSectorZonesActivationRules();
            _activationUpdateTimer = sectorZoneObjectsActivationUpdateTime;
        }

        _sectorRuntimeState.sectorTime += ESTime.worldDeltaTime;
    }

    public void SetDirectionToSirius(Vector3 direction)
    {
        siriusDirection = direction.normalized;
    }

    public SectorStateSnapshot GetSectorStateSnapshot() => new(_sectorRuntimeState);
    public List<SectorZone> GetSectorZonesList() => new(sectorZones);

    public SectorZone GetSectorZone(string id)
    {
        for (int i = 0; i < sectorZones.Count; i++)
        {
            if (sectorZones[i].ZoneId == id)
            {
                return sectorZones[i];
            }
        }

        throw new System.Exception($"Попытка получить несуществующий сектор: {id}");
    }

    public Vector3 GetLocalCoordinates(Vector3 worldPosition)
    {
        Vector3 offset = worldPosition - Center;

        return new Vector3(
            Vector3.Dot(offset, Right),
            Vector3.Dot(offset, Up),
            offset.z);
    }

    private void UpdateRuntimeState(float dt)
    {
        List<string> currentSectorIds = new List<string>(_sectorRuntimeState.playerCurrentSectorsTimes.Keys);

        for (int i = 0; i < currentSectorIds.Count; i++)
        {
            SectorZone sectorZone = GetSectorZone(currentSectorIds[i]);
            if (!sectorZone.CheckPlayerInZone())
            {
                _sectorRuntimeState.playerCurrentSectorsTimes.Remove(currentSectorIds[i]);
                SectorZoneExited?.Invoke(currentSectorIds[i]);
            } else
            {
                _sectorRuntimeState.playerCurrentSectorsTimes[currentSectorIds[i]] += dt;
            }
        }

        for (int i = 0; i < sectorZones.Count; i++)
        {
            if (sectorZones[i].CheckPlayerInZone() && !_sectorRuntimeState.playerCurrentSectorsTimes.ContainsKey(sectorZones[i].ZoneId))
            {
                _sectorRuntimeState.playerCurrentSectorsTimes[sectorZones[i].ZoneId] = 0f;
                SectorZoneEntered?.Invoke(sectorZones[i].ZoneId);

                if (!_sectorRuntimeState.visitedSectors.Contains(sectorZones[i].ZoneId))
                {
                    _sectorRuntimeState.visitedSectors.Add(sectorZones[i].ZoneId);
                    NewSectorZoneVisited?.Invoke(sectorZones[i].ZoneId);
                }
            }
        }
    }

    private void UpdateSectorZonesActivationRules()
    {
        for (int i = 0; i < sectorZones.Count; i++)
        {
            sectorZones[i].UpdateActivationRules();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Center, arriveRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Center, safeRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(Center, warningRadius);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(Center, Up * warningRadius * 1.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Center, Down * warningRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(Center, Left * warningRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Center, Right * warningRadius);
    }
}

public class SectorRuntimeState
{
    public float sectorTime = 0f;
    public Dictionary<string, float> playerCurrentSectorsTimes = new Dictionary<string, float>();
    public HashSet<string> visitedSectors = new HashSet<string>();
}

public struct SectorStateSnapshot
{
    public float sectorTime;
    public Dictionary<string, float> playerCurrentSectorsTimes;
    public HashSet<string> visitedSectors;

    public SectorStateSnapshot(SectorRuntimeState sectorRuntimeState)
    {
        sectorTime = sectorRuntimeState.sectorTime;
        playerCurrentSectorsTimes = new Dictionary<string, float>(sectorRuntimeState.playerCurrentSectorsTimes);
        visitedSectors = new HashSet<string>(sectorRuntimeState.visitedSectors);
    }
}

public enum PlayerInSectorSafetyStatus
{
    Safe,
    Warning,
    ImpendingDoom
}
