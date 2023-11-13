using UnityEngine;

public class EmissionVisualizer : MonoBehaviour
{
    [SerializeField] private Gradient _emitProgressGradient;
    [SerializeField] private ParticleSystem[] _emitPSes;

    private SpriteRenderer _playerSR;
    private Transform _target;

    public void Bind(Transform target)
    {
        _target = target;
        _playerSR = target.GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate() {
        if (_target != null)
            transform.position = _target.position;
    }

    public void TogglePS(bool tog)
    {
        if (tog)
        {
            for (int i = 0; i < _emitPSes.Length; i++)
            {
                _emitPSes[i].Play();
            }
        }
        else
        {
            for (int i = 0; i < _emitPSes.Length; i++)
            {
                _emitPSes[i].Stop();
            }
        }
    }

    public void ColorProgress(float part)
    {
        if (_playerSR != null)
            _playerSR.color = _emitProgressGradient.Evaluate(part);
    }
}
