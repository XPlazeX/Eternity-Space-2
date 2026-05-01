using UnityEngine;
using UnityEngine.UI;

public class LabRequireMark : MonoBehaviour
{
    // [SerializeField] private Image _markBorder;
    // [SerializeField] private Text _markLabel;
    // [SerializeField] private MarkState[] _markStates;

    // public void SetState(int state)
    // {
    //     _markBorder.color = _markStates[state].SchemeColor;
    //     _markLabel.color = _markStates[state].SchemeColor;
    //     _markLabel.text = _markStates[state].MarkLabel;
    // }

    // public void ToggleInteractable(bool tog)
    // {
    //     _markBorder.GetComponent<Button>().interactable = tog;
    // }

    // [System.Serializable]
    // private class MarkState
    // {
    //     [SerializeField] private Color _color;
    //     [SerializeField] private Vector2Int _labLocalizationVector;

    //     public Color SchemeColor => _color;
    //     public string MarkLabel => new TextLoader("Lab", _labLocalizationVector.x, _labLocalizationVector.y, true).FirstCell;
    // }
}
