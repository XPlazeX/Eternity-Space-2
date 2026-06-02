using UnityEngine;

[RequireComponent(typeof(PowerupFabricDrone))]
public class FabricDroneController : MonoBehaviour
{
    [SerializeField] private PowerupFabricDrone drone;
    [SerializeField] private Transform parkingTransform;
    [SerializeField] private bool parkOnStart = true;
    [Header("Encounter Commands")]
    [SerializeField] private bool startWorkOnEncounterStart = true;
    [SerializeField] private float wrecksFindingRadiusToStart = 200f;
    [SerializeField] private float findingWrecksUpdateTime = 1f;
    [SerializeField] private bool rawOnEncounterStop = true;

    private bool _initFindingWrecks = false;
    private float _findingTimer = 0f;
    private EncounterDirector _encounterDirector;

    private void Awake()
    {
        if (drone == null)
            drone = GetComponent<PowerupFabricDrone>();
    }

    void OnEnable()
    {
        _encounterDirector = FindAnyObjectByType<EncounterDirector>();

        if (_encounterDirector != null)
        {
            _encounterDirector.EncounterStarted += OnEncounterStart;
            _encounterDirector.EncounterStopped += OnEncounterStop;
        }       
    }

    void OnDisable()
    {
        if (_encounterDirector != null)
        {
            _encounterDirector.EncounterStarted -= OnEncounterStart;
            _encounterDirector.EncounterStopped -= OnEncounterStop;
        }
    }

    private void Start()
    {
        if (parkOnStart)
            ParkImmediately();
    }

    void FixedUpdate()
    {
        if (_initFindingWrecks)
        {
            _findingTimer += Time.fixedDeltaTime;

            if (_findingTimer >= findingWrecksUpdateTime)
            {
                _findingTimer = 0f;

                if (AnyWreckInRadius())
                {
                    StartWorking();
                    _initFindingWrecks = false;
                }
            }
        }       
    }

    public void Park()
    {
        if (drone == null)
            return;

        drone.ParkAt(GetParkingTransform());
    }

    public void ParkImmediately()
    {
        if (drone == null)
            return;

        drone.ParkAt(GetParkingTransform(), attachImmediately: true);
    }

    public void SetParkingTransform(Transform newParkingTransform)
    {
        parkingTransform = newParkingTransform;
    }

    public void StartWorking()
    {
        if (drone == null)
            return;

        drone.StartWorking();
    }

    public bool RawFabricate()
    {
        if (drone == null)
            return false;

        return drone.RequestRawFabricate();
    }

    public bool RawFabricate(GameObject prefab)
    {
        if (drone == null)
            return false;

        return drone.RequestRawFabricate(prefab);
    }

    private Transform GetParkingTransform()
    {
        if (parkingTransform != null)
            return parkingTransform;

        return Player.PlayerTransform;
    }

    private bool AnyWreckInRadius()
    {
        bool result = false;
        Wreck[] candidates = FindObjectsByType<Wreck>(FindObjectsSortMode.None);

        if (candidates.Length == 0)
            return false;

        for (int i = 0; i < candidates.Length; i++)
        {
            Wreck candidate = candidates[i];

            if (candidate == null)
                continue;

            if ((candidate.transform.position - transform.position).magnitude > wrecksFindingRadiusToStart)
                continue;

            result = true;
            break;
        }
        return result;
    }

    private void OnEncounterStart(Encounter encounter)
    {
        Debug.Log("on encounter start: ");
        if (startWorkOnEncounterStart)
        {
            _initFindingWrecks = true;
        }
    }

    private void OnEncounterStop(Encounter encounter)
    {
        Debug.Log("on encounter stop: ");
        if (_encounterDirector == null || _encounterDirector.ActiveEncountersCount > 0)
            return;

        if (rawOnEncounterStop)
        {
            RawFabricate();
        }
        
        _initFindingWrecks = false;
    }
}
