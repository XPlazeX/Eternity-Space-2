using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSettingObserver : MonoBehaviour
{
    private enum AudioChannel 
    {
        Soundtrack = 0,
        Sounds = 1
    }

    [SerializeField] private AudioChannel _audioChannel;
    
    private AudioSource _audioSource;
    private float _defaultVolume;

    private void Start() 
    {
        GameObject.FindWithTag("AudioCore").GetComponent<SoundPlayer>().AudioSettingsChanged += OnAudioSettingsChanged;
        OnAudioSettingsChanged();
    }

    public void OnAudioSettingsChanged()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
            _defaultVolume = _audioSource.volume;
        }

        if (_audioChannel == AudioChannel.Soundtrack)
            _audioSource.volume = _defaultVolume * SoundPlayer.MusicVolume;

        else if (_audioChannel == AudioChannel.Sounds)
            _audioSource.volume = _defaultVolume * SoundPlayer.SoundVolume;
    }
}
