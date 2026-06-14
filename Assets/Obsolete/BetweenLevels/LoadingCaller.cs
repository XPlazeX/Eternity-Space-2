using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingCaller : MonoBehaviour
{
    private const float mask_wearing_animation_duration = 3f;

    [SerializeField] private SoundObject _helmetON;
    [SerializeField] private SoundObject _helmetOFF;
    [SerializeField] private SoundObject _timebackOST;
    [SerializeField] private SoundObject _silentOST;
    [SerializeField] private Text _messageLabel;
    [SerializeField] private Vector2Int _minMaxTimebackTime;

    public void HelmetONSound()
    {
        SoundPlayer.PlayUISound(_helmetON);
    }

    public void HelmetOFFSound()
    {
        SoundPlayer.PlayUISound(_helmetOFF);
    }

    public void TimebackAnimation()
    {
        StartCoroutine(TimeReseting());
    }

    // private void Start() {
    //     Time.timeScale = 0f;
    // }

    public void OpenMask()
    {
        GetComponent<Animator>().SetTrigger("SceneOpens");
        HelmetOFFSound();
    }

    public void EndMaskWearing()
    {
        SceneTransition.OnAnimationOver();
    }

    private IEnumerator TimeReseting()
    {
        SoundPlayer soundPlayer = SceneStatics.AudioCore.GetComponent<SoundPlayer>();

        yield return new WaitForSecondsRealtime(mask_wearing_animation_duration);

        soundPlayer.SetSoundtrack(_timebackOST, 3f);
        soundPlayer.MuteSounds();

        InteriorSoundController.InteriorSoundWork = false;
        TimeHandler.Pause();

        _messageLabel.text = SceneLocalizator.GetLocalizedString("Game", 10, 0);

        int timer = Random.Range(_minMaxTimebackTime.x, _minMaxTimebackTime.y);

        while (timer > 0)
        {
            if (Random.value < 0.1f)
            {
                _messageLabel.text = _messageLabel.text + "\n" + SceneLocalizator.GetLocalizedString("Game", 11, Random.Range(0, 18));
            }

            timer --;
            yield return new WaitForSecondsRealtime(1f);
        }

        _messageLabel.text = _messageLabel.text + "\n" + SceneLocalizator.GetLocalizedString("Game", 10, 1);

        soundPlayer.SetSoundtrack(_silentOST, 3f);
        yield return new WaitForSecondsRealtime(3f);

        _messageLabel.text = "";

        yield return new WaitForSecondsRealtime(1f);

        // TimeHandler.Resume();
        SceneTransition.OnTimeResetAnimationOver();
    }

    // public void TimeResume()
    // {
    //     Time.timeScale = 1f;
    // }
}
