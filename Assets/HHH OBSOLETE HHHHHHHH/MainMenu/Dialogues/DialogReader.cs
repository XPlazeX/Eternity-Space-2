using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogReader : MonoBehaviour
{
    const int contagion_message_count = 11;

    [SerializeField] private Text _terminal;
    [SerializeField] private Vector2Int _minMaxSymbols;
    [SerializeField] private Font _normalFont;
    [SerializeField] private Font _contagionFont;

    private string[] _message;
    private int _carette = 0;
    private int _contagionCarette = 0;

    private void Start() {
        _message = new TextLoader("Dialogues", GameSessionInfoHandler.GetSessionSave().DialogueEntry).GetRow(0);
        Debug.Log($"loaded radio with len {_message.Length}");
        SetMessage(0);
    }

    public void NextMessage()
    {
        if (_carette + 1 >= _message.Length)
        {
            SetRandomMessage();
            _carette = _message.Length;
            return;
        }

        _carette ++;
        SetMessage(_carette);
    }

    public void PreviousMessage()
    {
        if (_carette - 1 < 0)
        {
            SetRandomMessage();
            _carette = -1;
            return;
        }

        _carette --;
        SetMessage(_carette);
    }

    private void SetMessage(int id)
    {
        if (id >= _message.Length || id < 0)
        {
            SetRandomMessage();
            return;
        }

        _terminal.font = _normalFont;
        _terminal.text = _message[id];
    }

    private void SetRandomMessage()
    {
        if (ContagionHandler.ContagionLevel >= 11)
        {
            _terminal.font = _contagionFont;
            _terminal.text = SceneLocalizator.GetLocalizedString("Contagion", _contagionCarette, 0);

            _contagionCarette += 1;

            if (_contagionCarette >= contagion_message_count)
                _contagionCarette = 0;

            return;
        }

        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz 0123456789 !@$^&*()_+=-?/|;:%~";
        int len = UnityEngine.Random.Range(_minMaxSymbols.x, _minMaxSymbols.y);
        var stringChars = new char[len];

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
        }

        var finalString = new String(stringChars);
        _terminal.text = finalString;
    }
}
