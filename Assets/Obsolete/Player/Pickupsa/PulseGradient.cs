using UnityEngine;

public class PulseGradient : MonoBehaviour
{
    [SerializeField] private Gradient _gradient;
    [SerializeField] private float _cycleTime;

    private float _timer;
    private SpriteRenderer _sr;

    private void Start() {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate() 
    {
        if (_timer > _cycleTime)
            _timer = 0f;

        _sr.color = _gradient.Evaluate(_timer / _cycleTime);

        _timer += Time.deltaTime;
    }
}
