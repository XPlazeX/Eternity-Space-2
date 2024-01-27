using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private DialogueSwitcher _dialogueSwitcher;
    [SerializeField] private float _voiceTime = 0.05f;
    [SerializeField] private float _voiceVolume = 0.66f;
    [SerializeField] private float _voicePitchRandomizing = 0.33f;
    [Space]
    [SerializeField] private AssetReference[] _characterIcons;
    [SerializeField] private AssetReference[] _voiceClips;
    [SerializeField] private int[] _repeatsVariants;

    private AsyncOperationHandle _characterOperationHandle;
    private AsyncOperationHandle _voiceOperationHandle;
    private DialogueConverter _dialogueConverter;
    private int _currentPhrase = 0;

    private bool _isVoicing;
    private int _lastVoiceRepeats;
    private float _voiceTimer;
    private SoundObject _activeVoice;
    private bool _processing = false;

    private void Update() 
    {
        if (!_isVoicing)
            return;

        _voiceTimer -= Time.unscaledDeltaTime;

        if (_voiceTimer <= 0)
        {
            SoundPlayer.PlayUISound(_activeVoice);

            _lastVoiceRepeats --;
            if (_lastVoiceRepeats <= 0)
            {
                _isVoicing = false;
            }

            _voiceTimer = _voiceTime;
        }

    }

    public void InitializeDialog(string filename)
    {
        if (PlayerPrefs.GetFloat("DialogMod", 0) == 2)
            return;
            
        _dialogueConverter = new DialogueConverter(filename);

        SetDialogueFrame();
        TimeHandler.Pause();
    }

    public void Next()
    {
        if (_processing)
            return;
            
        _currentPhrase ++;

        if (_currentPhrase >= _dialogueConverter.GetDialogueLength() - 1)
        {
            EndDialog();
            return;
        }

        SetDialogueFrame();
    }

    public void SetDialogueFrame()
    {
        string[] frameData = _dialogueConverter.GetDialogueFrame(_currentPhrase);

        bool isRight = (frameData[0] == "1");

        StartCoroutine(ProcessDialogueFrame(int.Parse(frameData[1]), int.Parse(frameData[2]), _repeatsVariants[int.Parse(frameData[3])], isRight));
        _dialogueSwitcher.SetText(frameData[4]);
    }

    private IEnumerator ProcessDialogueFrame(int characterID, int voiceID, int repeats, bool isRight)
    {
        _processing = true;
        if (_characterOperationHandle.IsValid())
            Addressables.Release(_characterOperationHandle);
        if (_voiceOperationHandle.IsValid())
            Addressables.Release(_voiceOperationHandle);

        var iconReference = _characterIcons[characterID];
        var voiceReference = _voiceClips[voiceID];

        _characterOperationHandle = Addressables.LoadAssetAsync<Sprite>(iconReference);
        yield return _characterOperationHandle;

        if (isRight)
        {
            _dialogueSwitcher.SetRight();
            _dialogueSwitcher.SetRightIcon((Sprite)_characterOperationHandle.Result);
        } else {
            _dialogueSwitcher.SetLeft();
            _dialogueSwitcher.SetLeftIcon((Sprite)_characterOperationHandle.Result);
        }

        _voiceOperationHandle = Addressables.LoadAssetAsync<AudioClip>(voiceReference);//voiceReference.LoadAssetAsync<AudioClip>();
        yield return _voiceOperationHandle;

        _activeVoice = new SoundObject((AudioClip)_voiceOperationHandle.Result, _voiceVolume, _voicePitchRandomizing);
        _isVoicing = true;
        _lastVoiceRepeats = repeats;
        _processing = false;
    }

    public void EndDialog()
    {
        TimeHandler.Resume();
        _dialogueSwitcher.CloseDialogue();
    }
}
