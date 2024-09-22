using UnityEngine;

public class DisableBehaviorAfterTime : MonoBehaviour
{
    [SerializeField] private float _timeToggling;
    [SerializeField] private MonoBehaviour[] _behaviours;
    [SerializeField] private bool _targetToggle = false;

    private float _timer;
    private bool _triggered = false;

    private void OnEnable() {
        _timer = _timeToggling;

        for (int i = 0; i < _behaviours.Length; i++)
        {
            _behaviours[i].enabled = !_targetToggle;
        }

        _triggered = false;
    }

    private void FixedUpdate() {
        _timer -= Time.fixedDeltaTime;

        if (!_triggered && _timer < 0)
        {
            for (int i = 0; i < _behaviours.Length; i++)
            {
                _behaviours[i].enabled = _targetToggle;
            }

            _triggered = true;
        }
    }
}
