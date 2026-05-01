using UnityEngine;

public class InteriorSoundController : MonoBehaviour
{
    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private SoundObject _interiorOST;

    public static bool InteriorSoundWork {get; set;} = true;
    public static float DefaultOSTTransitionTime {get; set;} = 6f;

    private SoundObject _ostSoundObject;
    private bool _gameScene = false;
    private bool _stopDelays = false;
    private bool _nowInterior = true;

    private void OnEnable() {
        _gameScene = SceneTransition.ActiveSceneName == "Game";
        InteriorSoundWork = true;
        DefaultOSTTransitionTime = 6f;
        TimeHandler.TimePaused += GoInteriorOST;
        TimeHandler.TimeNormalized += ContinueMainOST;
    }

    public void SetOSTSoundObject(SoundObject ost, float transitionTime = -1f)
    {
        _ostSoundObject = ost;

        if (transitionTime < 0)
            transitionTime = DefaultOSTTransitionTime;

        _soundPlayer.SetSoundtrack(_ostSoundObject, transitionTime);
    }

    public void SetOSTDelayed(SoundObject ost, float delay)
    {
        _ostSoundObject = ost;
        GameObject.FindWithTag("BetweenScenes").GetComponent<TimedDelegator>().FuseAction(SetMainOSTFromDelay, delay);
    }

    public void ToggleinteriorWork(bool tog)
    {
        InteriorSoundWork = tog;

        if (!tog)
        {
            _soundPlayer.SetSoundtrack(_ostSoundObject, 2f);
            _nowInterior = false;
        }
    }

    public void StopDelays() => _stopDelays = true;

    public void SetInteriorOST(float transitionTime = 6f)
    {
        if (!_gameScene)
            return;
        _ostSoundObject = _interiorOST;
        _soundPlayer.SetSoundtrack(_ostSoundObject, transitionTime);

        _nowInterior = true;
    }


    private void OnDisable() {
        TimeHandler.TimePaused -= GoInteriorOST;
        TimeHandler.TimeNormalized -= ContinueMainOST;
    }

    private void SetMainOSTFromDelay()
    {
        if (!_gameScene)
            return;
        if (_stopDelays || !_nowInterior)
        {
            return;
        }
        _soundPlayer.SetSoundtrack(_ostSoundObject, 6f);
        _nowInterior = false;
    }

    public void ContinueMainOST()
    {
        if (!InteriorSoundWork)
            return;
        if (!_gameScene)
            return;
        _soundPlayer.SetSoundtrack(_ostSoundObject, 3f, SoundPlayer.OSTMemoryOperation.ReleaseTime);
        _nowInterior = false;
    }

    public void GoInteriorOST()
    {
        if (!InteriorSoundWork)
            return;
        if (!_gameScene)
            return;
        _soundPlayer.SetSoundtrack(_interiorOST, 3f, SoundPlayer.OSTMemoryOperation.SaveTime);
        _nowInterior = true;
    }
}
