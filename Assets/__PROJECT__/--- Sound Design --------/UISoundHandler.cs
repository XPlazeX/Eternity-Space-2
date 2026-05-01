using UnityEngine;

public class UISoundHandler : MonoBehaviour
{
    [SerializeField] private SoundObject[] _soundObjects;

    public void PlayUISound(int id)
    {
        SoundPlayer.PlayUISound(_soundObjects[id]);
    }

    public void StopSoundtrack()
    {
        SceneStatics.AudioCore.GetComponent<SoundPlayer>().MuteSoundtrack();
    }
}
