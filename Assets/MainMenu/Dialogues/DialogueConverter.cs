using UnityEngine;

public class DialogueConverter
{
    private TextLoader _textLoader;

    public DialogueConverter(string filecode)
    {
        string dialogFolder = filecode.Split('-')[0];
        string filename = filecode.Substring(filecode.IndexOf('-') + 1);
        Debug.Log($"Dialogues/{dialogFolder}/{filename}");

        _textLoader = new TextLoader($"Dialogues/{dialogFolder}", filename);
    }

    public int GetDialogueLength() => _textLoader.TextMatrixLength;

    public string[] GetDialogueFrame(int id) => _textLoader.GetRow(id);
}
