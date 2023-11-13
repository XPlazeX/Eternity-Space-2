using UnityEngine;

public class SoundOnEnable : MonoBehaviour
{
    [SerializeField] private int _code;

    // private SoundPlayer _soundPlayer;

    // private void Awake() {
    //     _soundPlayer = GameObject.FindWithTag("SceneCore").GetComponent<SoundPlayer>();
    // }

    private void OnEnable() {
        print("Звук при включении не работает");
        //SoundPlayer.PlaySound(_code);
    }
}
