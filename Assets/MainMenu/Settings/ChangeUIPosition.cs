using UnityEngine;
using UnityEngine.UI;

public class ChangeUIPosition : MonoBehaviour
{
    [SerializeField] private string _uiCode;
    [SerializeField] private int _loopValue;
    [SerializeField] private Text _codeLabel;

    private void OnEnable() {
        int code = PlayerPrefs.GetInt(_uiCode, 0);
        _codeLabel.text = (code + 1).ToString();
    }

    public void NextPosition()
    {
        int code = PlayerPrefs.GetInt(_uiCode, 0);

        code ++;
        if (code > _loopValue)
            code = 0;

        PlayerPrefs.SetInt(_uiCode, code);

        _codeLabel.text = (code + 1).ToString();

        SceneStatics.UICore.GetComponent<UIPositioner>().UpdateUIPositions();
    }
}
