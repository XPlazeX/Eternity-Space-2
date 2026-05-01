using UnityEngine;

public class TriggerDebug : MonoBehaviour
{
    [SerializeField] private string _filename;

    void Start()
    {
        Open();
    }

    public void Open() => GameObject.FindWithTag("BetweenScenes").GetComponent<DialogueOpener>().TriggerDialogue(_filename);
}
