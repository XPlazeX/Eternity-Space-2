using UnityEngine;

public class EventedLineColorIndicator : MonoBehaviour
{
    [SerializeField] private Color[] colors;

    public void SetColor(int index)
    {
        if (index < 0 || index >= colors.Length)
            return;

        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = colors[index];
        lineRenderer.endColor = colors[index];
    }
}
