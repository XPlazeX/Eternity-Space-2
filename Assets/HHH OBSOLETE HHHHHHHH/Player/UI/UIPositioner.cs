using UnityEngine;

public class UIPositioner : MonoBehaviour
{
    [SerializeField] private UIPivots[] _uiPivots;

    private void OnEnable() {
        UpdateUIPositions();
    }

    public void UpdateUIPositions()
    {
        for (int i = 0; i < _uiPivots.Length; i++)
        {
            _uiPivots[i].rect.anchoredPosition = _uiPivots[i].positions[PlayerPrefs.GetInt(_uiPivots[i].observingCode)];
        }
    }

    [System.Serializable]
    public struct UIPivots
    {
        public RectTransform rect;
        public string observingCode;
        public Vector2[] positions;
    }
}
