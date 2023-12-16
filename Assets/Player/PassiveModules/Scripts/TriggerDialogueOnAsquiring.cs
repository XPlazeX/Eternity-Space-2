using UnityEngine;

public class TriggerDialogueOnAsquiring : Module
{
    [SerializeField] private string _dialogueFilename;
    [SerializeField] private bool _unique;
    [SerializeField] private int _unlockCodeForUnique;

    public override void Asquiring()
    {
        print("asq");
        if (_unique)
        {
            if (Unlocks.HasUnlock(_unlockCodeForUnique))
                return;

            Unlocks.NewUnlock(_unlockCodeForUnique);
        }

        GameObject.FindWithTag("BetweenScenes").GetComponent<DialogueOpener>().TriggerDialogue(_dialogueFilename);
    }
}
