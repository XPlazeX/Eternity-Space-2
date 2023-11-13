using UnityEngine;
using UnityEngine.UI;

public class ControllableText : MonoBehaviour
{
    [SerializeField] private Text _label;

    private string _filename;
    private int _readingColumn;

    public void Initialize(string filename, int readingColumn)
    {
        _filename = filename;
        _readingColumn = readingColumn;
        SetText(0);
    }

    public void SetText(int row)
    {
        _label.text = SceneLocalizator.GetLocalizedString(_filename, row, _readingColumn);
    }
}
