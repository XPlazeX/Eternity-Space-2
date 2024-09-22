using UnityEngine;

public class OSTReplacer : MonoBehaviour
{
    [SerializeField] private SoundObject _newOst;
    [SerializeField] private float _delay = 0f;
    [SerializeField] private float _transitionTime = 4f;

    void Start()
    {
        if (_delay > 0)
            Invoke("Replace", _delay);
        else
            Replace();
    }

    private void Replace()
    {
        GameObject.FindWithTag("AudioCore").GetComponent<InteriorSoundController>().SetOSTSoundObject(_newOst, _transitionTime);
    }

}
