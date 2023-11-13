using UnityEngine;

public class DialogueConverter
{
    private TextLoader _textLoader;

    public DialogueConverter(string filename)
    {
        //Debug.Log($"try to open dialogue: {filename}");
        _textLoader = new TextLoader(Dialogue.dialogues_text_path, PlayerPrefs.GetString("Language", "RU-") + filename);
    }

    public int GetDialogueLength() => _textLoader.TextMatrixLength;

    public string[] GetDialogueFrame(int id) => _textLoader.GetRow(id);
}
