using UnityEngine;

public class ConditionedDialogTrigger : MonoBehaviour
{
    [SerializeField] private DialogCondition[] _dialogs;
    [SerializeField] private bool _randomize = false;
    [SerializeField] private bool _autoStart = false;
    [SerializeField] private bool _isTrigger = false;
    [SerializeField] private bool _safeParry = false;

    private void Start() {
        if (!_autoStart)
            return;

        if (!_randomize)
        {
            TriTriggerFirst();
        } else{
            for (int i = 0; i < _dialogs.Length; i++)
            {
                if (TryTriggerDialog(Random.Range(0, _dialogs.Length)))
                {
                    return;
                }
            }
        }
    }

    public void TriTriggerFirst()
    {
        for (int i = 0; i < _dialogs.Length; i++)
        {
            if (TryTriggerDialog(i))
                break;   
        }
    }

    public virtual bool TryTriggerDialog(int id)
    {
        if (!Unlocks.HasUnlocks(_dialogs[id].unlockRequires))
            return false;
        if (_dialogs[id].uniqueDialog && Unlocks.HasUnlock(_dialogs[id].writtenUnlockID))
            return false;
        else if (_dialogs[id].uniqueDialog && !Unlocks.HasUnlock(_dialogs[id].writtenUnlockID))
            Unlocks.NewUnlock(_dialogs[id].writtenUnlockID);

        GameObject.FindWithTag("BetweenScenes").GetComponent<DialogueOpener>().TriggerDialogue(_dialogs[id].dialogFilename, _dialogs[id].delay, _safeParry);
        print($"Triggering dialog: {id}");
        return true;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (_isTrigger && other.CompareTag("Player"))
        {
            TriTriggerFirst();
        }
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
