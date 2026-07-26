using UnityEngine;
using UnityEngine.UI;

public class TextLocalizer : MonoBehaviour
{
    [SerializeField] private string _filename;
    [SerializeField] private Vector2Int _rowCol;
    [Space()]
    [SerializeField] private string _suffixCode;
    [SerializeField] private bool _multiline;

    private bool _localized = false;

    private void OnEnable() 
    {
        if (_localized)
            return;

        if (!string.IsNullOrEmpty(_suffixCode))
            GetComponent<Text>().text = SceneLocalizator.GetLocalizedString(_filename, _rowCol.x, _rowCol.y) + " " + _suffixCode;
        else
            GetComponent<Text>().text = SceneLocalizator.GetLocalizedString(_filename, _rowCol.x, _rowCol.y);

        if (_multiline)
        {
            GetComponent<Text>().text = (GetComponent<Text>().text).Replace("<br>", "\n");
        }

        _localized = true;
    }

    public void HandLocalize()
    {
        if (!string.IsNullOrEmpty(_suffixCode))
            GetComponent<Text>().text = SceneLocalizator.GetLocalizedString(_filename, _rowCol.x, _rowCol.y) + " " + _suffixCode;
        else
            GetComponent<Text>().text = SceneLocalizator.GetLocalizedString(_filename, _rowCol.x, _rowCol.y);

        if (_multiline)
        {
            GetComponent<Text>().text = (GetComponent<Text>().text).Replace("<br>", "\n");
        }

        _localized = true;
    }
}
