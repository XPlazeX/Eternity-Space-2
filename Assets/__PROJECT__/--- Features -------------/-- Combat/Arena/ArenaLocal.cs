using System;
using UnityEngine;

public class ArenaLocal : MonoBehaviour
{
    public static Action PivotChanged;
    public static Action BordersChanged;

    [SerializeField] private Transform pivotTransform;
    [Header("Arena")]
    [SerializeField] private float arenaRadius = 20f;
    [SerializeField] private CircleLineRenderer borderCircleRenderer;
    [Header("Relativity")]
    [SerializeField] private float relativityOffsetOut = 10f;
    [SerializeField] private AnimationCurve relativityGrowth;
    [SerializeField] private bool disableRelativity;

    public static Transform Pivot {get; private set;}
    public static Vector3 Center => Pivot == null ? Vector3.zero : Pivot.position;
    public static float Radius {get; private set;}
    public static float RelativityRadius => instance == null ? 0f : Radius + instance.relativityOffsetOut;
    public static float RelativityMultiplier {get; private set;} = 1f;

    public static float LPosX {get; private set;} = 10f;
    public static float LNegX {get; private set;} = 10f;
    public static float LPosY {get; private set;} = 10f;
    public static float LNegY {get; private set;} = 10f;

    public static float WPosX => Pivot.position.x + LPosX;
    public static float WNegX => Pivot.position.x - LNegX;
    public static float WPosY => Pivot.position.y + LPosY;
    public static float WNegY => Pivot.position.y - LNegY;
    public static float Width => LNegX + LPosX;
    public static float Height => LNegY + LPosY;
    
    public static float VisibleRadius => CalculateVisibleRadius();
    private static float _cahchedRadius = -1f;
    private static float _cahchedVisibleRadius = -1f;

    private static ArenaLocal instance;

    void Awake()
    {
        instance = this;
        SetBallionCore(pivotTransform, arenaRadius);
        // SetPivot(pivotTransform);
        // SetBorders(positiveXBorder, negativeXBorder, positiveYBorder, negativeYBorder);
    }

    private static float CalculateRadius()
    {
        if (_cahchedRadius < 0f)
        {
            _cahchedRadius = Mathf.Sqrt(
                Mathf.Max(LPosX, LNegX) * Mathf.Max(LPosX, LNegX) 
                + Mathf.Max(LPosY, LNegY) * Mathf.Max(LPosY, LNegY)); // описаная окружность

            // Debug.Log($"{Mathf.Max(LPosX, LNegX)}^2 + {Mathf.Max(LPosY, LNegY)}^2 sqrt = {_cahchedRadius}");
        }

        return _cahchedRadius;
    }

    private static float CalculateVisibleRadius()
    {
        if (_cahchedVisibleRadius < 0f)
        {
            float a = RelativityRadius + CameraController.Width;
            float b = RelativityRadius + CameraController.Height;

            _cahchedVisibleRadius = Mathf.Sqrt(a * a + b * b);
        }

        return _cahchedVisibleRadius;
    }

    public void SetBallionCore(Transform pivot, float arenaRadius)
    {
        SetPivot(pivot);
        Radius = arenaRadius;
        float a = Mathf.Sqrt(arenaRadius * arenaRadius / 2f);
        SetBorders(a, a, a, a);

        borderCircleRenderer.SetRadius(Radius);
    }

    private void SetPivot(Transform transform)
    {
        Pivot = transform;
        PivotChanged?.Invoke();
    }

    private void SetBorders(float px, float nx, float py, float ny)
    {
        LPosX = px;
        LNegX = nx;
        LPosY = py;
        LNegY = ny;
        BordersChanged?.Invoke();
        _cahchedRadius = -1f;
        _cahchedVisibleRadius = -1f;
    }

    public static Vector3 RelativeVectorAtPoint(Vector3 moveDelta, Vector3 point, float relativeFactor)
    {
        if (instance == null)
            return moveDelta;

        if (Pivot == null)
            return moveDelta;

        if (instance.disableRelativity)
            return moveDelta;

        if (moveDelta.sqrMagnitude <= Mathf.Epsilon)
            return moveDelta;

        relativeFactor = Mathf.Clamp01(relativeFactor * RelativityMultiplier);

        // При 0 замедление вообще не работает.
        if (relativeFactor <= 0f)
            return moveDelta;

        float innerRadius = Mathf.Max(0f, Radius);
        float outerRadius = Mathf.Max(innerRadius, RelativityRadius);

        if (outerRadius <= Mathf.Epsilon)
            return Vector3.zero;

        // Работаем в XY, потому что это 2D-арена.
        Vector2 center = Center;
        Vector2 currentPoint = point;
        Vector2 delta = moveDelta;

        Vector2 fromCenter = currentPoint - center;
        float distance = fromCenter.magnitude;

        // Если объект ровно в центре, направления "наружу" ещё нет.
        // В этом случае просто страхуемся от слишком большого delta.
        if (distance <= 0.0001f)
        {
            Vector2 clampedDelta = ClampDeltaInsideRadius2D(
                currentPoint,
                delta,
                center,
                outerRadius
            );

            return new Vector3(clampedDelta.x, clampedDelta.y, moveDelta.z);
        }

        Vector2 outwardDir = fromCenter / distance;

        // Положительное значение = движение от центра.
        // Отрицательное значение = движение к центру.
        float radialAmount = Vector2.Dot(delta, outwardDir);

        // Всё, что не является радиальным движением наружу/внутрь:
        // касательная часть.
        Vector2 tangentDelta = delta - outwardDir * radialAmount;

        float finalRadialAmount = radialAmount;

        // Замедляем только движение наружу и только начиная с Radius.
        if (radialAmount > 0f && distance >= innerRadius)
        {
            float t;

            if (outerRadius > innerRadius)
                t = Mathf.InverseLerp(innerRadius, outerRadius, distance);
            else
                t = 1f;

            float growth = instance.relativityGrowth != null
                ? Mathf.Clamp01(instance.relativityGrowth.Evaluate(t))
                : t;

            // growth = 0 -> движение не замедлено
            // growth = 1, relativeFactor = 1 -> полная остановка наружной компоненты
            // growth = 1, relativeFactor = 0.5 -> наружная компонента работает на 50%
            float radialMultiplier = 1f - growth * relativeFactor;

            finalRadialAmount *= radialMultiplier;
        }

        Vector2 resultDelta = tangentDelta + outwardDir * finalRadialAmount;

        // Страховка от пересечения RelativityRadius.
        // Особенно важно при больших moveDelta.
        if (IsInsideArena(point) && relativeFactor >= 1f)
        {
            resultDelta = ClampDeltaInsideRadius2D(
            currentPoint,
            resultDelta,
            center,
            outerRadius
        );
        }
        
        return new Vector3(resultDelta.x, resultDelta.y, moveDelta.z);
    }

    private static Vector2 ClampDeltaInsideRadius2D(
        Vector2 point,
        Vector2 delta,
        Vector2 center,
        float maxRadius)
    {
        Vector2 targetPoint = point + delta;
        Vector2 fromCenter = targetPoint - center;

        float distance = fromCenter.magnitude;

        if (distance <= maxRadius || distance <= 0.0001f)
            return delta;

        Vector2 clampedTargetPoint = center + fromCenter / distance * maxRadius;

        return clampedTargetPoint - point;
    }

    public static bool IsInsideArena(Vector3 point)
    {
        float mag = (Pivot.position - point).magnitude;

        return mag < RelativityRadius;
    }

    public static Vector3 ClampPosition(Vector3 position)
    {
        if (IsInsideArena(position)) return position;

        Vector3 normalized = (position - Pivot.position).normalized;

        return Pivot.position + (normalized * RelativityRadius);
    }

    public static float GetRelativityAtPoint(Vector3 point)
    {
        if (instance.disableRelativity) return 0f;

        float mag = (Pivot.position - point).magnitude;

        if (mag < Radius || mag > RelativityRadius) return 0f;

        return Mathf.Clamp01(instance.relativityGrowth.Evaluate((mag - Radius) / (RelativityRadius - Radius)) * RelativityMultiplier);
    }

/// <summary>
/// Возвращает вектор внутри арены в мировых координатах с отступом.
/// </summary>
/// <param name="padding">Отступ вовнутрь, если отрицательный - то наружу</param>
/// <returns></returns>
    public static Vector3 GetRandomFieldPosition(float padding = 2f)
    {
        return new Vector3(
            UnityEngine.Random.Range(WNegX + padding, WPosX - padding),
            UnityEngine.Random.Range(WNegY + padding, WPosY - padding), 0f
        );
    }

    void OnDrawGizmosSelected()
    {
        if (Pivot == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pivotTransform.position, arenaRadius);
        Gizmos.color = Color.orange;
        Gizmos.DrawLine(new Vector3(WNegX, WPosY, 0f), new Vector3(WPosX, WPosY, 0f));
        Gizmos.DrawLine(new Vector3(WNegX, WNegY, 0f), new Vector3(WPosX, WNegY, 0f));
        Gizmos.DrawLine(new Vector3(WNegX, WNegY, 0f), new Vector3(WNegX, WPosY, 0f));
        Gizmos.DrawLine(new Vector3(WPosX, WNegY, 0f), new Vector3(WPosX, WPosY, 0f));
        Gizmos.DrawWireSphere(pivotTransform.position, RelativityRadius);
    }
}
