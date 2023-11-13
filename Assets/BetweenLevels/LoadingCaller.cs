using UnityEngine;

public class LoadingCaller : MonoBehaviour
{
    [SerializeField] private SoundObject _helmetON;
    [SerializeField] private SoundObject _helmetOFF;

    public void HelmetONSound()
    {
        SoundPlayer.PlayUISound(_helmetON);
    }

    public void HelmetOFFSound()
    {
        SoundPlayer.PlayUISound(_helmetOFF);
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

    // public void TimeResume()
    // {
    //     Time.timeScale = 1f;
    // }
}
