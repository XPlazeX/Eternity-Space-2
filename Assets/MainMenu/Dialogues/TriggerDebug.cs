using UnityEngine;

public class TriggerDebug : MonoBehaviour
{
    [SerializeField] private string _filename;

    void Start()
    {
        GameObject.FindWithTag("BetweenScenes").GetComponent<DialogueOpener>().TriggerDialogue(_filename);
    }
}
