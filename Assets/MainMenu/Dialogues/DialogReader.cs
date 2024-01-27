using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogReader : MonoBehaviour
{
    [SerializeField] private Text _terminal;
    [SerializeField] private Vector2Int _minMaxSymbols;

    private string[] _message;
    private int _carette = 0;

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
        }

        _terminal.text = _message[id];
    }

    private void SetRandomMessage()
    {
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
