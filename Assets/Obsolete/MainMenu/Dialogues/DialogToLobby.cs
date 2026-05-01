using UnityEngine;

public class DialogToLobby : MonoBehaviour
{
    [SerializeField] private UnlockRequire[] _unlockRequires;
    [SerializeField] private string _dialogFilename;
    [SerializeField] private bool _uniqueDialog;
    [SerializeField] private int _writtenUnlockID;

    private void Start() {
        bool condition = Unlocks.HasUnlocks(_unlockRequires);

        if (_uniqueDialog && Unlocks.HasUnlock(_writtenUnlockID))
            condition = false;

        if (condition)
        {
            GlobalSave gsave = GlobalSaveHandler.GetSave();
            gsave.LobbyDialogue = _dialogFilename;
            GlobalSaveHandler.RewriteSave(gsave);

            if (_uniqueDialog)
                Unlocks.NewUnlock(_writtenUnlockID);
        }
    }
}
