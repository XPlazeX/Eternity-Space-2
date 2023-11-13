using UnityEngine;

public class WaveDialogueObserver : WaveEvents
{
    [Space()]
    [SerializeField] private string[] _dialogueFilenames;

    //private Spawner _spawner;
    private DialogueOpener _dialogueOpener;

    protected override void Start() 
    {
        _dialogueOpener = GameObject.FindWithTag("BetweenScenes").GetComponent<DialogueOpener>();
        base.Start();
    }

    protected override void Trigger(int conditionID)
    {
        _dialogueOpener.TriggerDialogue(_dialogueFilenames[conditionID]); 
        print($"wave trigger dialogue: {_dialogueFilenames[conditionID]}");
    }
}
