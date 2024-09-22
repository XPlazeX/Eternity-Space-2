using UnityEngine;

public class DialogOnRestart : MonoBehaviour
{
    [SerializeField] private string _dialogueFilename;
    [SerializeField] private bool _unique;
    [SerializeField] private int _unlockCodeForUnique;
    [SerializeField] private bool _progressUnlocks;
    [SerializeField] private int _unlockCodeForProgress;

    private void OnEnable() {
        SceneTransition.SceneRestarted += OnSceneRestarted;
    }

    public void OnSceneRestarted()
    {

        SceneTransition.SceneRestarted -= OnSceneRestarted;
        
        if (_unique)
        {
            if (Unlocks.HasUnlock(_unlockCodeForUnique))
                return;

            Unlocks.NewUnlock(_unlockCodeForUnique);
        }

        if (_progressUnlocks)
        {
            Unlocks.ProgressUnlock(_unlockCodeForProgress, 1);
        }

        GameObject.FindWithTag("BetweenScenes").GetComponent<DialogueOpener>().TriggerDialogue(_dialogueFilename);
    }
}
