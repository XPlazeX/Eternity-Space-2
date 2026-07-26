using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour
{
    public delegate void audioActionHandler();
    public event audioActionHandler AudioSettingsChanged;

    public enum OSTMemoryOperation
    {
        None = 0,
        SaveTime = 1,
        ReleaseTime = 2
    }

    const float snapshot_transition_time = 0.15f;
    public const string master_toggle_player_prefs = "AudioMaster";
    public const string music_toggle_player_prefs = "Music";
    public const string sound_toggle_player_prefs = "Sound";
    public const string music_volume_player_prefs = "MusicVolume";
    public const string sound_volume_player_prefs = "SoundVolume";

    [SerializeField] private bool _disable = false;
    [SerializeField] private AudioMixerSnapshot[] _snapshots;
    [SerializeField] private AudioSource _soundtrackAudioSource;
    [SerializeField] private AudioSource[] _soundtrackAudioSources;
    [SerializeField] private AudioSource _soundsAudioSource;
    [SerializeField] private AudioSource _UIsoundsAudioSource;

    private static AudioSource SoundAudioSource;
    private static AudioSource UISoundAudioSource;

    private static bool Disabled {get; set;}

    public static float InnerAudioRadius {get; set;} = 5f;
    public static float OuterAudioRadius {get; set;} = 16f;

    public static float MusicVolume {get; private set;} = 0f;
    public static float SoundVolume {get; private set;} = 0f;

    private float _soundtrackVolume = 1f;
    private Coroutine _soundtrackTransitionCoroutine;
    private bool _firstOSTSourcePlayed = true;
    private float _oldVolume;
    private float _savedOstTime;

    private AudioSource ActiveSoundtrackSource => _soundtrackAudioSources[_firstOSTSourcePlayed ? 0 : 1];

    public void Initialize()
    {
        SoundAudioSource = _soundsAudioSource;
        UISoundAudioSource = _UIsoundsAudioSource;
        Disabled = _disable;
        UpdateData();
    }

    public void SetSoundtrack(SoundObject soundObject, float transitionTime = 6f, OSTMemoryOperation memoryOperation = OSTMemoryOperation.None)
    {
        print($"<color=cyan>Set OST: {soundObject.Clip.name}</color>");
        if (transitionTime == 0f)
        {
            ActiveSoundtrackSource.clip = soundObject.Clip;
            ActiveSoundtrackSource.volume = soundObject.Volume * MusicVolume;
            ActiveSoundtrackSource.pitch = soundObject.GetPitch();
            ActiveSoundtrackSource.Play();

            _soundtrackVolume = soundObject.Volume;
        }
        if (_soundtrackTransitionCoroutine != null)
        {
            StopCoroutine(_soundtrackTransitionCoroutine);

            AudioSource quiterSource = _soundtrackAudioSources[_firstOSTSourcePlayed ? 0 : 1];
            AudioSource louderSource = _soundtrackAudioSources[_firstOSTSourcePlayed ? 1 : 0];

            quiterSource.volume = louderSource.volume;

            _firstOSTSourcePlayed = !_firstOSTSourcePlayed;
        }

        _soundtrackTransitionCoroutine = StartCoroutine(TransitionSoundtrack(soundObject, transitionTime, memoryOperation));
    }

    public IEnumerator TransitionSoundtrack(SoundObject newSoundtrack, float transitionTime, OSTMemoryOperation memoryOperation)
    {
        float transitionTimer = 0;

        AudioSource quiterSource = _soundtrackAudioSources[_firstOSTSourcePlayed ? 0 : 1];
        AudioSource louderSource = _soundtrackAudioSources[_firstOSTSourcePlayed ? 1 : 0];

        _oldVolume = quiterSource.volume;

        louderSource.clip = newSoundtrack.Clip;
        louderSource.volume = 0;
        louderSource.pitch = newSoundtrack.GetPitch();
        louderSource.time = Mathf.Clamp(louderSource.time, 0, louderSource.clip.length - 0.01f);
        louderSource.Play();

        if (memoryOperation == OSTMemoryOperation.SaveTime)
        {
            _savedOstTime = quiterSource.time;
        } else if (memoryOperation == OSTMemoryOperation.ReleaseTime)
        {
            louderSource.time = Mathf.Clamp(_savedOstTime, 0, louderSource.clip.length - 0.01f);
        }

        float startQuiterVolume = quiterSource.volume;

        while (transitionTimer < transitionTime)
        {
            quiterSource.volume = Mathf.Lerp(_oldVolume, 0f, transitionTimer / transitionTime);
            louderSource.volume = Mathf.Lerp(0f, newSoundtrack.Volume * MusicVolume, transitionTimer / transitionTime);

            transitionTimer += ESTime.unscaledDeltaTime;
            yield return null;
        }
        
        quiterSource.Stop();

        louderSource.volume = newSoundtrack.Volume * MusicVolume;
        _soundtrackVolume = louderSource.volume;

        _firstOSTSourcePlayed = !_firstOSTSourcePlayed;
        _soundtrackTransitionCoroutine = null;
    }

    public void UpdateData()
    {
        if (PlayerPrefs.GetInt(master_toggle_player_prefs, 1) == 0)
        {
            MusicVolume = 0f;
            SoundVolume = 0f;
            ActiveSoundtrackSource.volume = _soundtrackVolume * MusicVolume;
            AudioSettingsChanged?.Invoke();
            return;
        }
        if (PlayerPrefs.GetInt(music_toggle_player_prefs, 1) == 1)
        {
            MusicVolume = PlayerPrefs.GetFloat(music_volume_player_prefs, 1f);
        }
        else
            MusicVolume = 0f;
        
        if (PlayerPrefs.GetInt(sound_toggle_player_prefs, 1) == 1)
        {
            SoundVolume = PlayerPrefs.GetFloat(sound_volume_player_prefs, 1f);
        }
        else
            SoundVolume = 0f;

        ActiveSoundtrackSource.volume = _soundtrackVolume * MusicVolume;
        AudioSettingsChanged?.Invoke();
    }

    public void MuteSoundtrack()
    {
        for (int i = 0; i < _soundtrackAudioSources.Length; i++)
        {
            _soundtrackAudioSources[i].mute = true;
        }
    }

    public void MuteSounds()
    {
        UISoundAudioSource.mute = true;
        SoundAudioSource.mute = true;
    }

    public void SetSnapshot(int id) => _snapshots[id].TransitionTo(snapshot_transition_time);

    public static void PlayUISound(SoundObject soundObject)
    {
        if (Disabled || soundObject.Volume == 0)
            return;

        UISoundAudioSource.pitch = soundObject.GetPitch();

        UISoundAudioSource.PlayOneShot(soundObject.Clip, soundObject.Volume * SoundVolume);
    }

    public static void PlaySound(SoundObject soundObject)
    {
        if (Disabled || soundObject.Volume == 0)
            return;

        SoundAudioSource.pitch = soundObject.GetPitch();

        SoundAudioSource.PlayOneShot(soundObject.Clip, soundObject.Volume * SoundVolume);
    }

    public static void PlaySound(SoundObject soundObject, Vector3 sourcePosition)
    {
        if (Disabled || soundObject.Volume == 0)
            return;

        SoundAudioSource.pitch = soundObject.GetPitch();

        float mag = CalculateMagnitude(sourcePosition) - InnerAudioRadius;

        float volumeMultiplier = 1f;

        if (mag > 0)
        {
            volumeMultiplier *= 1f - (mag / (OuterAudioRadius - InnerAudioRadius));
        }

        //print($"volume multiplier: {volumeMultiplier} | pos: {sourcePosition} to {Player.PlayerTransform.position}, mag {mag}");

        SoundAudioSource.PlayOneShot(soundObject.Clip, soundObject.Volume * volumeMultiplier * SoundVolume);
    }

    public static void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (Disabled || clip == null)
            return;

        SoundAudioSource.pitch = pitch;
        
        SoundAudioSource.PlayOneShot(clip, volume * SoundVolume);
        //print("fireSound");
    }

    public static void PlaySound(AudioClip clip, Vector3 sourcePosition, float volume = 1f, float pitch = 1f)
    {
        if (Disabled || clip == null)
            return;

        SoundAudioSource.pitch = pitch;

        float mag = CalculateMagnitude(sourcePosition) - InnerAudioRadius;

        if (mag > 0)
        {
            volume *= 1f - (mag / (OuterAudioRadius - InnerAudioRadius));
        }
        
        SoundAudioSource.PlayOneShot(clip, volume * SoundVolume);
        //print("fireSound");
    }

    public static float CalculateMagnitude(Vector3 position)
    {
        if (Player.PlayerTransform != null)
        {
            return (Player.PlayerTransform.position - position).magnitude;
        } else 
        {
            return (Vector3.zero - position).magnitude;
        }
    }

}

[System.Serializable]
    public class SoundObject
    {
        [SerializeField] private AudioClip _clip;
        [SerializeField][Range(0f, 1.5f)] private float _volume;
        [SerializeField][Range(0f, 1f)] private float _pitchRandomizing;

        public AudioClip Clip => _clip;
        public float Volume => _volume;

        public SoundObject(AudioClip clip, float volume, float pitchRandomizing)
        {
            _clip = clip;
            _volume = volume;
            _pitchRandomizing = pitchRandomizing;
        }

        public float GetPitch()
        {
            if (_pitchRandomizing == 0f)
                return 1f;
            
            return Random.Range(1f - _pitchRandomizing, 1f + _pitchRandomizing);
        }
    }
