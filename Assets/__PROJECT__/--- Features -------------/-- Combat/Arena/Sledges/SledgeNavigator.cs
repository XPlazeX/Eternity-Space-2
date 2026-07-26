using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SledgeNavigator : MonoBehaviour
{
    [SerializeField] private Transform navigatorHandsRoot;
    [SerializeField] private SledgeNavigatorHand sledgeNavigatorHandPrefab;
    [SerializeField] private SledgeNavigatorHand circleRadarNavigatorHandPrefab;
    [SerializeField] private TMP_Text currentZonesListLabel;
    [SerializeField] private float objectHandsUpdateFrequency = 3f;

    private SectorRoot _currentSector;
    private List<SledgeNavigatorHand> _activeNavigatorHands = new List<SledgeNavigatorHand>();
    private float _objectsUpdateTimer = 0f;

    private void OnEnable() {
        Map.CurrentSectorUpdated += OnCurrentSectorUpdated;
        OnCurrentSectorUpdated();
    }

    void OnDisable()
    {
        Map.CurrentSectorUpdated -= OnCurrentSectorUpdated;
    }

    void Update()
    {
        _objectsUpdateTimer -= ESTime.worldDeltaTime;

        if (_objectsUpdateTimer <= 0f)
        {
            UpdateObjectHands();
            _objectsUpdateTimer = 1f / objectHandsUpdateFrequency;
        }
    }

    private void UpdateObjectHands()
    {
        // circle radars
        EnemyCircleDetector[] circleDetectors = GameObject.FindObjectsByType<EnemyCircleDetector>(FindObjectsSortMode.None);
        for (int i = 0; i < circleDetectors.Length; i++)
        {
            if (!CheckExistHandTransform(circleDetectors[i].transform))
            {
                SpawnCircleRadarNavigationHand(circleDetectors[i]);
            }
        }
    }

    private bool CheckExistHandTransform(Transform t)
    {
        for (int i = 0; i < _activeNavigatorHands.Count; i++)
        {
            if (_activeNavigatorHands[i].ObservingZone.center == t)
                return true;
        }

        return false;
    }

    public void LoadSector(SectorRoot sectorRoot)
    {
        if (_currentSector != null)
        {
            UnloadSector();
        }

        _currentSector = sectorRoot;

        List<SectorZone> sectorZones = _currentSector.GetSectorZonesList();

        for (int i = 0; i < sectorZones.Count; i++)
        {
            SpawnNavigationHand(sectorZones[i]);
        }

        SectorRoot.SectorZoneEntered += OnSectorZoneEntered;
        SectorRoot.SectorZoneExited += OnSectorZoneExited;
        SectorRoot.SectorZoneUpdated += OnSectorZoneUpdated;

        RecalculateCurrentZones();
    }

    public void UnloadSector()
    {
        if (_currentSector == null) return;

        for (int i = 0; i < _activeNavigatorHands.Count; i++)
        {
            Destroy(_activeNavigatorHands[i].gameObject);
        }

        _activeNavigatorHands.Clear();

        _currentSector = null;

        SectorRoot.SectorZoneEntered -= OnSectorZoneEntered;
        SectorRoot.SectorZoneExited -= OnSectorZoneExited;
        SectorRoot.SectorZoneUpdated -= OnSectorZoneUpdated;
    }

    private void OnSectorZoneEntered(string zoneID)
    {
        for (int i = 0; i < _activeNavigatorHands.Count; i++)
        {
            if (_activeNavigatorHands[i].ObservingZone.zoneID == zoneID)
            {
                _activeNavigatorHands[i].UpdateSectorVisitStatus();
                break;
            }
        }

        RecalculateCurrentZones();
    }

    private void OnSectorZoneExited(string zoneID)
    {
        for (int i = 0; i < _activeNavigatorHands.Count; i++)
        {
            if (_activeNavigatorHands[i].ObservingZone.zoneID == zoneID)
            {
                _activeNavigatorHands[i].UpdateSectorVisitStatus();
                break;
            }
        }

        RecalculateCurrentZones();
    }

    private void OnSectorZoneUpdated(string zoneID)
    {
        bool isNewZone = true;

        for (int i = 0; i < _activeNavigatorHands.Count; i++)
        {
            if (_activeNavigatorHands[i].ObservingZone.zoneID == zoneID)
            {
                isNewZone = false;
                _activeNavigatorHands[i].UpdateSectorZoneHeaders();
                break;
            }
        }

        if (isNewZone)
        {
            SpawnNavigationHand(_currentSector.GetSectorZone(zoneID));
            return;
        }
    }

    private void OnCurrentSectorUpdated()
    {
        UnloadSector();

        if (Map.CurrentSector != null)
        {
            LoadSector(Map.CurrentSector);
        }
    }

    private void RecalculateCurrentZones()
    {
        List<string> currentZonesIDs = new(_currentSector.GetSectorStateSnapshot().playerCurrentSectorsTimes.Keys);

        if (currentZonesIDs.Count <= 0)
        {
            currentZonesListLabel.text = "ОТКРЫТЫЙ КОСМОС";
            return;
        }

        string resultString = "";
        for (int i = 0; i < currentZonesIDs.Count; i++)
        {
            string zoneName = _currentSector.GetSectorZone(currentZonesIDs[i]).DisplayName;
            resultString += zoneName.ToUpper() + "\n";
        }

        currentZonesListLabel.text = resultString;
    }

    private void SpawnNavigationHand(SectorZone observingZone)
    {
        SledgeNavigatorHand navigatorHand = Instantiate(sledgeNavigatorHandPrefab, navigatorHandsRoot.position, Quaternion.identity);

        navigatorHand.transform.SetParent(navigatorHandsRoot);
        navigatorHand.SetObservingSectorZone(observingZone);
        _activeNavigatorHands.Add(navigatorHand);
    }

    private void SpawnCircleRadarNavigationHand(EnemyCircleDetector observingDetector)
    {
        SledgeNavigatorHand navigatorHand = Instantiate(circleRadarNavigatorHandPrefab, navigatorHandsRoot.position, Quaternion.identity);

        navigatorHand.transform.SetParent(navigatorHandsRoot);
        navigatorHand.SetObservingCircleRadar(observingDetector);
        _activeNavigatorHands.Add(navigatorHand);
    }
}
