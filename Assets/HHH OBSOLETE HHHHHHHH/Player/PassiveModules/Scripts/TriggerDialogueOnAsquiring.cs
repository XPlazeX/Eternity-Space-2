using UnityEngine;

public class TriggerDialogueOnAsquiring : Module
{
    [SerializeField] private string _dialogueFilename;
    [SerializeField] private bool _unique;
    [SerializeField] private int _unlockCodeForUnique;
    [SerializeField] private bool _progressUnlocks;
    [SerializeField] private int _unlockCodeForProgress;
    [SerializeField] private bool _disableLight;
    [SerializeField] private bool _disableLaunch;
    [SerializeField] private SoundObject _newOST;

    public override void Asquiring()
    {
        print("asq");
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

        if (_disableLight)
        {
            Camera.main.GetComponent<MainMenuCamera>().CodeRed();
        }

        if (_disableLaunch)
        {
            GameObject.FindObjectOfType<MissionStopper>().ToggleLaunchButton(false);
        }

        if (_newOST.Clip != null)
        {
            SceneStatics.AudioCore.GetComponent<SoundPlayer>().SetSoundtrack(_newOST);
        }
    }
}
