using UnityEngine;
using UnityEngine.UI;

public class AudioMaster : MonoBehaviour
{
    [SerializeField] private Image _masterImage;
    [SerializeField] private Image _musicImage;
    [SerializeField] private Image _soundImage;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private Text _musicVolumeLabel;
    [SerializeField] private Text _soundVolumeLabel;
    [Space()]
    [SerializeField] private Sprite _enabledMasterTogglerSprite;
    [SerializeField] private Sprite _disabledMasterTogglerSprite;
    [SerializeField] private Sprite _enabledTogglerSprite;
    [SerializeField] private Sprite _disabledTogglerSprite;
    [SerializeField] private Color _enabledTextColor;
    [SerializeField] private Color _disabledTextColor;
    [Space()]
    [SerializeField] private SoundObject[] _testMusics;
    [SerializeField] private SoundObject[] _testSounds;

    private SoundPlayer _soundPlayer;

    private void Start() {
        _soundPlayer = GameObject.FindWithTag("AudioCore").GetComponent<SoundPlayer>();
        InitState();
    }

    public void ToggleMaster()
    {
        PlayerPrefs.SetInt(SoundPlayer.master_toggle_player_prefs, PlayerPrefs.GetInt(SoundPlayer.master_toggle_player_prefs, 1) == 1 ? 0 : 1);
        PlayerPrefs.Save();
        UpdateState();
    }

    public void ToggleMusic()
    {
        PlayerPrefs.SetInt(SoundPlayer.music_toggle_player_prefs, PlayerPrefs.GetInt(SoundPlayer.music_toggle_player_prefs, 1) == 1 ? 0 : 1);
        PlayerPrefs.Save();
        UpdateState();
    }

    public void ToggleSounds()
    {
        PlayerPrefs.SetInt(SoundPlayer.sound_toggle_player_prefs, PlayerPrefs.GetInt(SoundPlayer.sound_toggle_player_prefs, 1) == 1 ? 0 : 1);
        PlayerPrefs.Save();
        UpdateState();
    }

    public void SoundSliderChanged()
    {
        PlayerPrefs.SetFloat(SoundPlayer.music_volume_player_prefs, _musicSlider.value);
        PlayerPrefs.SetFloat(SoundPlayer.sound_volume_player_prefs, _soundSlider.value);
        PlayerPrefs.Save();
        UpdateState();
    }

    public void PlayTestSound(int id)
    {
        SoundPlayer.PlayUISound(_testSounds[id]);
    }

    public void SetTestOST(int id)
    {
        GameObject.FindWithTag("AudioCore").GetComponent<SoundPlayer>().SetSoundtrack(_testMusics[id]);

        if (id == 1)
            Camera.main.GetComponent<MainMenuCamera>().Nirvus();
        else
            Camera.main.GetComponent<MainMenuCamera>().ExtraNoise();
    }

    public void InitState()
    {
        _musicSlider.value = PlayerPrefs.GetFloat(SoundPlayer.music_volume_player_prefs, 1f);
        _soundSlider.value = PlayerPrefs.GetFloat(SoundPlayer.sound_volume_player_prefs, 1f);

        UpdateState();
    }

    public void UpdateState()
    {
        bool master = (PlayerPrefs.GetInt(SoundPlayer.master_toggle_player_prefs, 1) == 1);
        bool music = (PlayerPrefs.GetInt(SoundPlayer.music_toggle_player_prefs, 1) == 1);
        bool sounds = (PlayerPrefs.GetInt(SoundPlayer.sound_toggle_player_prefs, 1) == 1);

        float musicVolume = _musicSlider.value;
        float soundVolume = _soundSlider.value;

        if (!master || !music)
        {
            _musicVolumeLabel.color = _disabledTextColor;
            _musicVolumeLabel.text = $"0%";
        }
        if (!master || !sounds)
        {
            _soundVolumeLabel.color = _disabledTextColor;
            _soundVolumeLabel.text = $"0%";
        }
        if (master && music)
        {
            _musicVolumeLabel.color = _enabledTextColor;
            _musicVolumeLabel.text = $"{Mathf.RoundToInt(musicVolume * 100f)}%";
        }

        if (master && sounds)
        {
            _soundVolumeLabel.color = _enabledTextColor;
            _soundVolumeLabel.text = $"{Mathf.RoundToInt(soundVolume * 100f)}%";
        }

        _masterImage.sprite = master ? _enabledMasterTogglerSprite : _disabledMasterTogglerSprite;
        _musicImage.sprite = music ? _enabledTogglerSprite : _disabledTogglerSprite;
        _soundImage.sprite = sounds ? _enabledTogglerSprite : _disabledTogglerSprite;

        if (_soundPlayer == null)
            return;
        _soundPlayer.UpdateData();
    }

}
