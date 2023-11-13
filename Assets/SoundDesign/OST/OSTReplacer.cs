using UnityEngine;

public class OSTReplacer : MonoBehaviour
{
    [SerializeField] private SoundObject _newOst;

    void Start()
    {
        GameObject.FindWithTag("AudioCore").GetComponent<SoundPlayer>().SetSoundtrack(_newOst);
    }

}
