using UnityEngine;

public class Decoy : MonoBehaviour
{
    [SerializeField] private float _delay;

    private float _timer;
    private bool _used = false;

    private void OnEnable() {
        _timer = _delay;
        _used = false;
    }

    private void Update() {
        _timer -= ESTime.worldDeltaTime;

        if (_timer <= 0f && !_used)
        {
            StartWork();
            _used = true;
        }
    }

    public void StartWork()
    {
        Player.SetTargetPlayer(transform);
    }

    private void OnDisable() {
        Player.ResetTargetPlayer();
    }
}
