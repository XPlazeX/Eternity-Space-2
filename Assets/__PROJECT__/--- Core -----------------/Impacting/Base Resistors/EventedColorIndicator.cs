using UnityEngine;

public class EventedColorIndicator : MonoBehaviour
{
    [SerializeField] private Color[] colors;

    public void SetColor(int index)
    {
        if (index < 0 || index >= colors.Length)
            return;

        GetComponent<SpriteRenderer>().color = colors[index];
    }
}
