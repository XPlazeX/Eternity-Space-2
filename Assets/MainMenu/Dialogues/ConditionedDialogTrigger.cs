using UnityEngine;

public class ConditionedDialogTrigger : MonoBehaviour
{
    [SerializeField] private DialogCondition[] _dialogs;

    public void TriTriggerFirst()
    {
        TryTriggerDialog(0);
    }

    public virtual void TryTriggerDialog(int id)
    {
        if (!Unlocks.HasUnlocks(_dialogs[id].unlockRequires))
            return;
        if (_dialogs[id].uniqueDialog && Unlocks.HasUnlock(_dialogs[id].writtenUnlockID))
            return;
        else if (_dialogs[id].uniqueDialog && !Unlocks.HasUnlock(_dialogs[id].writtenUnlockID))
            Unlocks.NewUnlock(_dialogs[id].writtenUnlockID);

        GameObject.FindWithTag("BetweenScenes").GetComponent<DialogueOpener>().TriggerDialogue(_dialogs[id].dialogFilename, _dialogs[id].delay);
    }
    
    [System.Serializable]
    private struct DialogCondition
    {
        public UnlockRequire[] unlockRequires;
        public bool uniqueDialog;
        public int writtenUnlockID;
        public string dialogFilename;
        public float delay;
    }
}
