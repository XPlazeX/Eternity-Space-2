using UnityEngine;

public class IlyaAnimation : MonoBehaviour
{
    [SerializeField] private LineRenderer _lr;
    [SerializeField] private float _duration;
    [SerializeField] private float _dotOffset;

    private float _timer;

    private void Start() {
        _timer = _duration;
    }

    private void Update() {
        if (_timer < 0f)
        {
            _timer = 0f;
        }
        for (int i = 0; i < 3; i++)
        {
            _lr.SetPosition(i, transform.position + (Quaternion.Euler(0,0,120f * i) * transform.up) * (_dotOffset * (_timer / _duration)));
        }

        _timer -= ESTime.worldDeltaTime;
    }
}
