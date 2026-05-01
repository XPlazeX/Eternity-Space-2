using System;
using UnityEngine;

public class ArenaLocal : MonoBehaviour
{
    public static Action PivotChanged;
    public static Action BordersChanged;

    [SerializeField] private Transform pivotTransform;
    [Header("Local hard borders")]
    [SerializeField] private float positiveXBorder = 10f;
    [SerializeField] private float negativeXBorder = 10f;
    [SerializeField] private float positiveYBorder = 10f;
    [SerializeField] private float negativeYBorder = 10f;

    public static Transform Pivot {get; private set;}
    public static Vector3 Center => Pivot == null ? Vector3.zero : Pivot.position;

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
    public static float Radius => CalculateRadius();
    public static float VisibleRadius => CalculateVisibleRadius();

    private static float _cahchedRadius = -1f;
    private static float _cahchedVisibleRadius = -1f;

    void Awake()
    {
        SetPivot(pivotTransform);
        SetBorders(positiveXBorder, negativeXBorder, positiveYBorder, negativeYBorder);
        
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
            float a = Mathf.Max(LPosX, LNegX) + PlayerController.MovingOffset + CameraController.Width;
            float b = Mathf.Max(LPosY, LNegY) + PlayerController.MovingOffset + CameraController.Height;

            _cahchedVisibleRadius = Mathf.Sqrt(a * a + b * b);
        }

        return _cahchedVisibleRadius;
    }

    public void SetPivot(Transform transform)
    {
        Pivot = transform;
        PivotChanged?.Invoke();
    }

    public void SetBorders(float px, float nx, float py, float ny)
    {
        LPosX = px;
        LNegX = nx;
        LPosY = py;
        LNegY = ny;
        BordersChanged?.Invoke();
        _cahchedRadius = -1f;
        _cahchedVisibleRadius = -1f;
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
        Gizmos.DrawLine(new Vector3(WNegX, WPosY, 0f), new Vector3(WPosX, WPosY, 0f));
        Gizmos.DrawLine(new Vector3(WNegX, WNegY, 0f), new Vector3(WPosX, WNegY, 0f));
        Gizmos.DrawLine(new Vector3(WNegX, WNegY, 0f), new Vector3(WNegX, WPosY, 0f));
        Gizmos.DrawLine(new Vector3(WPosX, WNegY, 0f), new Vector3(WPosX, WPosY, 0f));
    }
}
