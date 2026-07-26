using UnityEngine;

public class SlidingRect : MonoBehaviour
{
    [SerializeField] private Transform point;
    [SerializeField] private RectTransform slidingRect;
    [SerializeField] private float slidingRadius = 100f;

    private void LateUpdate()
    {
        UpdatePosition();
    }

    private void OnValidate()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (point == null || slidingRect == null)
            return;

        Transform rectParent = slidingRect.parent;
        Vector3 direction = GetDirection(rectParent);

        if (direction.sqrMagnitude <= Mathf.Epsilon)
            return;

        direction.Normalize();

        Vector3 rectRight = GetRectAxis(rectParent, slidingRect.right);
        Vector3 rectUp = GetRectAxis(rectParent, slidingRect.up);

        Vector2 rectSize = slidingRect.rect.size;
        Vector3 rectScale = slidingRect.localScale;

        float halfWidth = Mathf.Abs(rectSize.x * rectScale.x) * 0.5f;
        float halfHeight = Mathf.Abs(rectSize.y * rectScale.y) * 0.5f;
        Vector2 localDirection = new Vector2(
            Vector3.Dot(direction, rectRight),
            Vector3.Dot(direction, rectUp));

        Vector3 pointPosition = rectParent != null
            ? rectParent.InverseTransformPoint(point.position)
            : point.position;

        Vector3 contactPosition = pointPosition + direction * Mathf.Max(0f, slidingRadius);
        Vector2 rectCenter = slidingRect.rect.center;
        Vector3 centerToContact = GetCenterToContactOffset(localDirection, rectRight, rectUp, halfWidth, halfHeight);
        Vector3 pivotToCenter = rectRight * (rectCenter.x * rectScale.x) + rectUp * (rectCenter.y * rectScale.y);
        Vector3 targetPivot = contactPosition - centerToContact - pivotToCenter;

        slidingRect.position = rectParent != null
            ? rectParent.TransformPoint(targetPivot)
            : targetPivot;
    }

    private Vector3 GetDirection(Transform rectParent)
    {
        Vector3 direction = rectParent != null
            ? rectParent.InverseTransformDirection(point.up)
            : point.up;

        direction.z = 0f;
        return direction;
    }

    private static Vector3 GetRectAxis(Transform rectParent, Vector3 worldAxis)
    {
        Vector3 axis = rectParent != null
            ? rectParent.InverseTransformDirection(worldAxis)
            : worldAxis;

        axis.z = 0f;
        return axis.normalized;
    }

    private static Vector3 GetCenterToContactOffset(
        Vector2 direction,
        Vector3 rectRight,
        Vector3 rectUp,
        float halfWidth,
        float halfHeight)
    {
        const float AxisThreshold = 0.001f;

        if (direction.sqrMagnitude <= AxisThreshold * AxisThreshold)
            return Vector3.zero;

        direction.Normalize();

        float x = Mathf.Abs(direction.x) > AxisThreshold
            ? -Mathf.Sign(direction.x) * halfWidth
            : 0f;
        float y = Mathf.Abs(direction.y) > AxisThreshold
            ? -Mathf.Sign(direction.y) * halfHeight
            : 0f;

        return rectRight * x + rectUp * y;
    }
}
