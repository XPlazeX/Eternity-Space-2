using TMPro;
using UnityEngine;

public class SledgeNavigatorHand : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] private Transform handTransform;
    [SerializeField] private TMP_Text nameLabel;
    [SerializeField] private TMP_Text distanceLabel;
    [SerializeField] private SpriteRenderer signalIcon;
    [SerializeField] private Color unvisitedIconColor;
    [SerializeField] private Color visitedIconColor;
    [Header("Sliding Mark")]
    [SerializeField] private Transform slidingMarkTransform;
    [SerializeField] private float slidingMinDistance = 6f;
    [SerializeField] private float slidingMaxDistance = 11.45f;
    [SerializeField] private float refMinDistance = 100f;
    [SerializeField] private float refMaxDistance = 4000f;
    [Header("Hiding")]
    [SerializeField] private GameObject[] hidingByDistanceObjects;
    [SerializeField] private float exitThresholdDistance = 50f;
    [SerializeField] private CanvasGroup[] hidingCanvasGroups;
    [SerializeField] private SpriteRenderer hiddenIcon;
    [SerializeField] private LineRenderer hiddenLine;
    [SerializeField] private float hiddenAnimationTime = 1f;
    [SerializeField] private AnimationCurve hiddenProgression;
    [Header("Database")] 
    [SerializeField] private NavigationIconPair[] navigationIconPairs;

    private NavigationZone _observingZone;
    private bool _visible = true;
    private bool _visibilityEnabled = true;
    private float _visibilityAnimationTimer;
    private float _visibilityAnimationStartAlpha = 1f;
    private float _visibilityAnimationTargetAlpha = 1f;
    private float _visibilityAlpha = 1f;
    private bool _sectorZoneLogics = false;

    public NavigationZone ObservingZone => _observingZone;

    public void SetObservingSectorZone(SectorZone sectorZone)
    {
        _observingZone = new NavigationZone(sectorZone);
        _sectorZoneLogics = true;

        UpdateSectorZoneHeaders();
        UpdateSectorVisitStatus();
    }

    public void SetObservingCircleRadar(EnemyCircleDetector circleDetector)
    {
        _observingZone = new NavigationZone(circleDetector);
        _sectorZoneLogics = false;

        UpdateSectorZoneHeaders();
        
    }

    public void UpdateSectorZoneHeaders()
    {
        if (_observingZone == null) return;

        nameLabel.text = _observingZone.displayName;

        for (int i = 0; i < hidingByDistanceObjects.Length; i++)
        {
            hidingByDistanceObjects[i].SetActive(_observingZone.displayDistance);
        }

        for (int i = 0; i < navigationIconPairs.Length; i++)
        {
            if (_observingZone.zoneType == navigationIconPairs[i].sectorZoneType)
            {
                signalIcon.sprite = navigationIconPairs[i].sprite;
                break;
            }
        }

        if (_observingZone.isHidden)
        {
            DisableVisibility();
        } else
        {
            EnableVisibility();
        }
    }

    public void UpdateSectorVisitStatus()
    {
        if (!_sectorZoneLogics) return;

        signalIcon.color = Map.CurrentSector.IsSectorZoneVizited(_observingZone.zoneID) ? visitedIconColor : unvisitedIconColor;
    }

    private void Update() 
    {
        if (_observingZone == null || _observingZone.center == null)
        {
            Destroy(gameObject);
            return;
        }

        if (_visible && _observingZone.CheckPlayerInZone())
        {
            Hide();
        } else if (!_visible && (Player.Position - _observingZone.center.position).magnitude > _observingZone.radius + exitThresholdDistance)
        {
            Show();
        }

        UpdateVisibilityAnimation();

        Vector3 vectorToZone = _observingZone.center.position - Player.Position;

        handTransform.up = vectorToZone.normalized;
        
        distanceLabel.text = Mathf.RoundToInt(vectorToZone.magnitude * Meter.SCALE).ToString() + " м";
        
        float slidingPart = Mathf.Lerp(slidingMinDistance, slidingMaxDistance, vectorToZone.magnitude / (refMaxDistance - refMinDistance));
        slidingPart = Mathf.Clamp(slidingPart, slidingMinDistance, slidingMaxDistance);

        slidingMarkTransform.localPosition = Vector3.up * slidingPart;
    }

    private void DisableVisibility()
    {
        _visibilityEnabled = false;
        _visible = false;
        _visibilityAnimationTimer = hiddenAnimationTime;
        _visibilityAnimationStartAlpha = 0f;
        _visibilityAnimationTargetAlpha = 0f;
        ApplyVisibilityAlpha(0f);
    }

    private void EnableVisibility()
    {
        _visibilityEnabled = true;
    }

    private void Hide()
    {
        _visible = false;
        StartVisibilityAnimation(0f);
    }

    private void Show()
    {
        if (!_visibilityEnabled) return;

        _visible = true;
        StartVisibilityAnimation(1f);
    }

    private void StartVisibilityAnimation(float targetAlpha)
    {
        if (Mathf.Approximately(_visibilityAnimationTargetAlpha, targetAlpha)) return;

        _visibilityAnimationTimer = 0f;
        _visibilityAnimationStartAlpha = _visibilityAlpha;
        _visibilityAnimationTargetAlpha = targetAlpha;

        if (hiddenAnimationTime <= 0f)
        {
            ApplyVisibilityAlpha(targetAlpha);
        }
    }

    private void UpdateVisibilityAnimation()
    {
        if (Mathf.Approximately(_visibilityAlpha, _visibilityAnimationTargetAlpha)) return;

        if (hiddenAnimationTime <= 0f)
        {
            ApplyVisibilityAlpha(_visibilityAnimationTargetAlpha);
            return;
        }

        _visibilityAnimationTimer += ESTime.worldDeltaTime;

        float normalizedTime = Mathf.Clamp01(_visibilityAnimationTimer / hiddenAnimationTime);
        float progression = hiddenProgression == null ? normalizedTime : hiddenProgression.Evaluate(normalizedTime);
        ApplyVisibilityAlpha(Mathf.Lerp(_visibilityAnimationStartAlpha, _visibilityAnimationTargetAlpha, progression));
    }

    private void ApplyVisibilityAlpha(float alpha)
    {
        _visibilityAlpha = Mathf.Clamp01(alpha);

        if (hidingCanvasGroups != null)
        {
            for (int i = 0; i < hidingCanvasGroups.Length; i++)
            {
                if (hidingCanvasGroups[i] != null)
                {
                    hidingCanvasGroups[i].alpha = _visibilityAlpha;
                }
            }
        }

        if (hiddenIcon != null)
        {
            Color color = hiddenIcon.color;
            color.a = _visibilityAlpha;
            hiddenIcon.color = color;
        }

        if (hiddenLine != null)
        {
            Gradient gradient = hiddenLine.colorGradient;
            GradientAlphaKey[] alphaKeys = gradient.alphaKeys;

            if (alphaKeys == null || alphaKeys.Length == 0)
            {
                alphaKeys = new[]
                {
                    new GradientAlphaKey(_visibilityAlpha, 0f),
                    new GradientAlphaKey(_visibilityAlpha, 1f)
                };
            }

            for (int i = 0; i < alphaKeys.Length; i++)
            {
                alphaKeys[i].alpha = _visibilityAlpha;
            }

            gradient.SetKeys(gradient.colorKeys, alphaKeys);
            hiddenLine.colorGradient = gradient;
        }
    }

    [System.Serializable]
    public struct NavigationIconPair
    {
        public NavigationZoneType sectorZoneType;
        public Sprite sprite;
    }
}

public class NavigationZone
{
    private static int Register = 0;

    public string zoneID;
    public string displayName;
    public Transform center;
    public float radius;
    public NavigationZoneType zoneType;
    public bool isHidden;
    public bool displayDistance;

    public bool CheckPlayerInZone()
    {
        return (Player.GetPlayerPosition() - center.position).magnitude <= radius;
    }

    public NavigationZone(SectorZone sectorZone)
    {
        zoneID = sectorZone.ZoneId;
        displayName = sectorZone.DisplayName;
        center = sectorZone.CenterTransform;
        radius = sectorZone.ArriveRadius;
        zoneType = ConvertSectorToNavigationZoneType(sectorZone.ZoneType);
        isHidden = sectorZone.IsHidden;
        displayDistance = sectorZone.ShowDistance;
    }

    public NavigationZone(EnemyCircleDetector enemyCircleDetector)
    {
        Register ++;
        zoneID = "circleDetector_" + Register.ToString();
        displayName = "РАДАР";
        center = enemyCircleDetector.transform;
        radius = enemyCircleDetector.DetectionRadius;
        zoneType = NavigationZoneType.CircleRadar;
        isHidden = false;
        displayDistance = true;
    }

    public NavigationZoneType ConvertSectorToNavigationZoneType(SectorZoneType sectorZoneType)
    {
        return (NavigationZoneType)(int)sectorZoneType;
    }
}

public enum NavigationZoneType
{
    Unknown,
    EnemyBase,
    Signal,
    OtherStation,
    Debris,
    CircleRadar
}
