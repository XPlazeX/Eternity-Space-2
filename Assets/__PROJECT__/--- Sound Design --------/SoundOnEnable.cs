using UnityEngine;

public class SoundOnEnable : MonoBehaviour
{
    [SerializeField] private SoundObject _sound;
    [SerializeField] private bool _ignoreFirst = false;

    private bool _firstTimeTriggered;

    private void OnEnable() {
        if (_ignoreFirst && !_firstTimeTriggered)
        {
            _firstTimeTriggered = true;
            return;
        }
        // if (!_firstTimeTriggered && GetComponent<PullableObject>() != null)
        // {
        //     _firstTimeTriggered = true;
        //     return;
        // }

        SoundPlayer.PlaySound(_sound);
    }
}
