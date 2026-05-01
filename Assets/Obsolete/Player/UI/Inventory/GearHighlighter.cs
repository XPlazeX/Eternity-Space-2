using UnityEngine;
using UnityEngine.UI;

public class GearHighlighter : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] _gearCanvasGroups;
    [SerializeField] private float _dimAlpha = 0.25f;

    private int _activeID = -1;

    public void Select(int id)
    {
        if (_activeID == id)
        {
            Deselect();
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            if (i == id)
            {
                _gearCanvasGroups[i].alpha = 1f;
                continue;
            }

             _gearCanvasGroups[i].alpha = _dimAlpha;
        }

        _activeID = id;
    }

    public void Deselect()
    {
        for (int i = 0; i < 3; i++)
        {
            _gearCanvasGroups[i].alpha = 1f;
        }

        _activeID = -1;
    }

}
