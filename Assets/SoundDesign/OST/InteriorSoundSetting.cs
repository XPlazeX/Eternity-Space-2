using UnityEngine;

public class InteriorSoundSetting : MonoBehaviour
{
    [SerializeField] private bool _autostart = true;
    [SerializeField] private bool _setInteriorToggle;
    [SerializeField] private bool _settingInteriorToggle;
    [Space()]
    [SerializeField] private bool _setOstTransitionTime;
    [SerializeField] private float _settingOstTransitionTime;
    [Space()]
    [SerializeField] private bool _stopDelays;

    private void Start() {
        if (_autostart)
            Enforce();
    }

    public void Enforce()
    {
        if (_setOstTransitionTime)
            InteriorSoundController.DefaultOSTTransitionTime = _settingOstTransitionTime;
        if (_setInteriorToggle)
            GameObject.FindWithTag("AudioCore").GetComponent<InteriorSoundController>().ToggleinteriorWork(_settingInteriorToggle);
        if (_stopDelays)
            GameObject.FindWithTag("AudioCore").GetComponent<InteriorSoundController>().StopDelays();
    }
}
