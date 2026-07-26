using System;
using UnityEngine;

public enum GearSlot
{
    Neutral = -1,
    First = 0,
    Second = 1,
    Third = 2,
    Fourth = 3,
    Fifth = 4,
    Sixth = 5
}

[Serializable]
public class GearLane
{
    public float x;

    public bool hasUpper = true;
    public bool hasLower = true;

    public GearSlot upperGear = GearSlot.Neutral;
    public GearSlot lowerGear = GearSlot.Neutral;

    public float upY = 100f;
    public float downY = -100f;
}

public class FGB : MonoBehaviour
{
    public static event Action<GearSlot> GearChanged;

    private enum State
    {
        Neutral,
        Vertical
    }

    [Header("Refs")]
    [SerializeField] private Transform leverVisual;

    [Header("Input")]
    [SerializeField] private float mouseScale = 18f;

    [Header("Neutral")]
    [SerializeField] private float neutralY = 0f;
    [SerializeField] private float neutralMinX = -150f;
    [SerializeField] private float neutralMaxX = 150f;

    [Header("Lanes")]
    [SerializeField] private GearLane[] lanes;

    [Header("Feel")]
    [SerializeField] private float laneEnterXThreshold = 28f;   // как близко к колонке надо быть, чтобы войти
    [SerializeField] private float cornerAssistX = 20f;         // дотягивание X до канавки при входе
    [SerializeField] private float gearSnapRadius = 24f;        // магнит к конечной точке
    [SerializeField] private float gearSnapSpeed = 20f;         // скорость магнита
    [SerializeField] private float neutralExitThreshold = 16f;  // как близко к нейтрали надо быть, чтобы выйти обратно
    [SerializeField] private float minVerticalIntent = 0.2f;    // минимальное вертикальное намерение
    [SerializeField] private float minHorizontalIntent = 0.2f;  // минимальное горизонтальное намерение

    [Header("Visual")]
    [SerializeField] private ParticleSystem leverEffectsPS;
    [SerializeField] private GameObject gearboxVisualObject;

    private bool _open;
    private State _state = State.Neutral;

    private Vector2 _leverPos;
    private int _activeLaneIndex = -1;

    private GearSlot _hoveredGear = GearSlot.Neutral;

    private Vector2 _restingPos;
    private bool _hasRestingPos;
    private GearSlot _committedGear = GearSlot.Neutral;
    // private GearSlot _committedGear = GearSlot.None;
    // private bool _committedUpper = true;

    public bool IsOpen => _open;
    public GearSlot HoveredGear => _hoveredGear;
    public GearSlot CurrentGear => _committedGear;
    public Vector2 LeverPosition => _open ? _leverPos : _restingPos;
    public Vector2 NormalizedLeverPosition => GetNormalizedLeverPosition(LeverPosition);

    private void Start()
    {
        _leverPos = new Vector2(0f, neutralY);
        _restingPos = _leverPos;
        _hasRestingPos = true;
        UpdateVisual();
        gearboxVisualObject.SetActive(false);
    }

    private void Update()
    {
        if (PlayerInput.SelectionDown)
            Open();

        if (!_open)
            return;

        Tick();

        if (PlayerInput.SelectionUp)
            CloseAndCommit();
    }

    private void Open()
    {
        _open = true;

        if (_hasRestingPos)
        {
            _leverPos = _restingPos;
        }
        else
        {
            _leverPos = new Vector2(0f, neutralY);
        }

        // Определяем состояние по позиции, а не по прошлой передаче
        int laneIndex = FindNearestLaneByX(_leverPos.x, 1f);
        bool onNeutral = Mathf.Abs(_leverPos.y - neutralY) <= 0.001f;

        if (!onNeutral && laneIndex >= 0)
        {
            _state = State.Vertical;
            _activeLaneIndex = laneIndex;
            _leverPos.x = lanes[laneIndex].x; // на всякий случай дожимаем в линию
        }
        else
        {
            _state = State.Neutral;
            _activeLaneIndex = -1;
            _leverPos.y = neutralY;
        }

        gearboxVisualObject.SetActive(true);
        if (leverEffectsPS != null)
        {
            leverEffectsPS.Play();
        }

        _hoveredGear = DetectGear();
        UpdateVisual();
    }

    private void CloseAndCommit()
    {
        _hoveredGear = DetectGear();

        if (_hoveredGear != GearSlot.Neutral)
        {
            // Воткнули передачу: сохраняем именно конечную точку передачи
            _committedGear = _hoveredGear;

            if (_activeLaneIndex >= 0 && _activeLaneIndex < lanes.Length)
            {
                var lane = lanes[_activeLaneIndex];

                bool upper = _leverPos.y > neutralY;
                _restingPos = new Vector2(
                    lane.x,
                    upper ? lane.upY : lane.downY
                );
            }
            else
            {
                _restingPos = _leverPos;
            }
        }
        else
        {
            // Не попали в передачу: коммитим нейтраль
            _committedGear = GearSlot.Neutral;
            _restingPos = new Vector2(
                Mathf.Clamp(_leverPos.x, neutralMinX, neutralMaxX),
                neutralY
            );
        }

        GearChanged?.Invoke(_committedGear);

        gearboxVisualObject.SetActive(false);
        if (leverEffectsPS != null)
        {
            leverEffectsPS.Stop();
        }

        _hasRestingPos = true;
        _open = false;
    }

    private void Tick()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseScale;

        switch (_state)
        {
            case State.Neutral:
                TickNeutral(mouseDelta);
                break;

            case State.Vertical:
                TickVertical(mouseDelta);
                break;
        }

        _leverPos = ApplyGearSnap(_leverPos);
        _hoveredGear = DetectGear();
        UpdateVisual();
    }

    private void TickNeutral(Vector2 delta)
    {
        _leverPos.y = neutralY;
        _leverPos.x = Mathf.Clamp(_leverPos.x + delta.x, neutralMinX, neutralMaxX);

        int nearestLane = FindNearestLaneByX(_leverPos.x, laneEnterXThreshold);
        if (nearestLane < 0)
            return;

        float absX = Mathf.Abs(delta.x);
        float absY = Mathf.Abs(delta.y);

        if (absY <= absX || absY < minVerticalIntent)
            return;

        var lane = lanes[nearestLane];

        if (delta.y > 0f && lane.hasUpper)
        {
            _state = State.Vertical;
            _activeLaneIndex = nearestLane;
            _leverPos.x = Mathf.Lerp(_leverPos.x, lane.x, cornerAssistX / Mathf.Max(cornerAssistX, 0.001f));
            _leverPos.x = lane.x;
            _leverPos.y = Mathf.Clamp(neutralY + delta.y, neutralY, lane.upY);
        }
        else if (delta.y < 0f && lane.hasLower)
        {
            _state = State.Vertical;
            _activeLaneIndex = nearestLane;
            _leverPos.x = lane.x;
            _leverPos.y = Mathf.Clamp(neutralY + delta.y, lane.downY, neutralY);
        }
    }

private void TickVertical(Vector2 delta)
{
    if (_activeLaneIndex < 0 || _activeLaneIndex >= lanes.Length)
    {
        _state = State.Neutral;
        _activeLaneIndex = -1;
        _leverPos.y = neutralY;
        return;
    }

    var lane = lanes[_activeLaneIndex];
    _leverPos.x = lane.x;

    float minY = lane.hasLower ? lane.downY : neutralY;
    float maxY = lane.hasUpper ? lane.upY : neutralY;

    // Главное изменение:
    // двигаемся по всей вертикальной канавке сразу,
    // чтобы можно было плавно пройти через нейтраль вниз/вверх
    _leverPos.y = Mathf.Clamp(_leverPos.y + delta.y, minY, maxY);

    float absX = Mathf.Abs(delta.x);
    float absY = Mathf.Abs(delta.y);

    bool nearNeutral = Mathf.Abs(_leverPos.y - neutralY) <= neutralExitThreshold;
    bool wantsHorizontal = absX > absY && absX >= minHorizontalIntent;

    if (nearNeutral && wantsHorizontal)
    {
        _state = State.Neutral;
        _activeLaneIndex = -1;
        _leverPos.y = neutralY;
        _leverPos.x = Mathf.Clamp(_leverPos.x + delta.x, neutralMinX, neutralMaxX);
    }
}

    private Vector2 ApplyGearSnap(Vector2 pos)
    {
        if (_state == State.Vertical && _activeLaneIndex >= 0 && _activeLaneIndex < lanes.Length)
        {
            var lane = lanes[_activeLaneIndex];

            if (lane.hasUpper)
            {
                Vector2 upper = new Vector2(lane.x, lane.upY);
                pos = Snap(pos, upper, gearSnapRadius, gearSnapSpeed);
            }

            if (lane.hasLower)
            {
                Vector2 lower = new Vector2(lane.x, lane.downY);
                pos = Snap(pos, lower, gearSnapRadius, gearSnapSpeed);
            }

            return pos;
        }

        // В нейтрали можно либо вообще не снапать,
        // либо снапать только к ближайшей конечной точке
        return SnapToNearestEndpoint(pos);
    }

    private Vector2 Snap(Vector2 current, Vector2 target, float radius, float speed)
    {
        float dist = Vector2.Distance(current, target);
        if (dist > radius || dist < 0.001f)
            return current;

        float t = 1f - dist / radius;
        return Vector2.Lerp(current, target, t * speed * ESTime.unscaledDeltaTime);
    }

    private Vector2 SnapToNearestEndpoint(Vector2 pos)
    {
        Vector2 bestTarget = pos;
        float bestDist = float.MaxValue;
        bool found = false;

        foreach (var lane in lanes)
        {
            if (lane.hasUpper)
            {
                Vector2 upper = new Vector2(lane.x, lane.upY);
                float d = Vector2.Distance(pos, upper);
                if (d < bestDist && d <= gearSnapRadius)
                {
                    bestDist = d;
                    bestTarget = upper;
                    found = true;
                }
            }

            if (lane.hasLower)
            {
                Vector2 lower = new Vector2(lane.x, lane.downY);
                float d = Vector2.Distance(pos, lower);
                if (d < bestDist && d <= gearSnapRadius)
                {
                    bestDist = d;
                    bestTarget = lower;
                    found = true;
                }
            }
        }

        if (!found)
            return pos;

        return Snap(pos, bestTarget, gearSnapRadius, gearSnapSpeed);
    }

private GearSlot DetectGear()
{
    if (_state == State.Vertical && _activeLaneIndex >= 0 && _activeLaneIndex < lanes.Length)
    {
        var lane = lanes[_activeLaneIndex];

        if (lane.hasUpper && Mathf.Abs(_leverPos.y - lane.upY) <= gearSnapRadius)
            return lane.upperGear;

        if (lane.hasLower && Mathf.Abs(_leverPos.y - lane.downY) <= gearSnapRadius)
            return lane.lowerGear;

        return GearSlot.Neutral;
    }

    return GearSlot.Neutral;
}

    private int FindNearestLaneByX(float x, float threshold)
    {
        int best = -1;
        float bestDist = threshold;

        for (int i = 0; i < lanes.Length; i++)
        {
            float d = Mathf.Abs(x - lanes[i].x);
            if (d <= bestDist)
            {
                bestDist = d;
                best = i;
            }
        }

        return best;
    }

    private Vector2 GetNormalizedLeverPosition(Vector2 position)
    {
        float normalizedX = Mathf.InverseLerp(neutralMinX, neutralMaxX, position.x) * 2f - 1f;
        float normalizedY = 0f;

        if (lanes != null && lanes.Length > 0 && !Mathf.Approximately(position.y, neutralY))
        {
            int laneIndex = FindNearestLaneByX(position.x, float.MaxValue);
            if (laneIndex >= 0)
            {
                GearLane lane = lanes[laneIndex];

                if (position.y > neutralY && lane.hasUpper)
                    normalizedY = Mathf.InverseLerp(neutralY, lane.upY, position.y);
                else if (position.y < neutralY && lane.hasLower)
                    normalizedY = -Mathf.InverseLerp(neutralY, lane.downY, position.y);
            }
        }

        return new Vector2(
            Mathf.Clamp(normalizedX, -1f, 1f),
            Mathf.Clamp(normalizedY, -1f, 1f)
        );
    }



    private bool IsUpperPosition(Vector2 pos)
    {
        return pos.y > neutralY;
    }

    private void UpdateVisual()
    {
        if (leverVisual != null)
            leverVisual.localPosition = _leverPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(
            transform.TransformPoint(new Vector3(neutralMinX, neutralY, 0f)),
            transform.TransformPoint(new Vector3(neutralMaxX, neutralY, 0f))
        );

        if (lanes == null)
            return;

        foreach (var lane in lanes)
        {
            if (lane.hasUpper)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(
                    transform.TransformPoint(new Vector3(lane.x, neutralY, 0f)),
                    transform.TransformPoint(new Vector3(lane.x, lane.upY, 0f))
                );
            }

            if (lane.hasLower)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(
                    transform.TransformPoint(new Vector3(lane.x, neutralY, 0f)),
                    transform.TransformPoint(new Vector3(lane.x, lane.downY, 0f))
                );
            }
        }
    }
}
