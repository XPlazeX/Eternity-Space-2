using UnityEngine;

public class SoundAnimatorTrigger : MonoBehaviour
{
    [SerializeField] private SoundObject _soundObject;
    [SerializeField] private bool _UI;
    [SerializeField] private bool _soundtrack;

    public void PlaySound()
    {
        if (_soundtrack)
            throw new System.InvalidCastException("саундтрек стоит устанавливать через метод SetSoundtrack");
        if (_UI)
            SoundPlayer.PlayUISound(_soundObject);
        else
            SoundPlayer.PlaySound(_soundObject);
    }

    public void SetSoundtrack()
    {
        if (!_soundtrack)
            throw new System.InvalidCastException("Данный SoundObject не имеет флага soundtrack.");
        
        GameObject.FindWithTag("AudioCore").GetComponent<SoundPlayer>().SetSoundtrack(_soundObject);
    }
}
