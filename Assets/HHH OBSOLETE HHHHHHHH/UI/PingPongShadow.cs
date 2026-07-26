using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Shadow))]
public class PingPongShadow : MonoBehaviour
{
    [SerializeField] private Shadow _targetShadow;
    [SerializeField] private float _timePingToPong;
    [SerializeField] private bool _unscaledTime = true;
    [SerializeField] private bool _useX;
    [SerializeField] private bool _useY;

    private Shadow _shadow;
    private float _timer;
    private float _targetX;
    private float _targetY;
    private float _direction = 1f;

    private void Start() 
    {
        if (_targetShadow == null)
            _shadow = GetComponent<Shadow>();
        else
            _shadow = _targetShadow;
        _targetX = _shadow.effectDistance.x;
        _targetY = _shadow.effectDistance.y;

        //_timer = _timePingToPong * 2f;
    }

    private void Update() 
    {
        _timer += _unscaledTime ? ESTime.unscaledDeltaTime : ESTime.worldDeltaTime;

        // if (_timer <= 0f)
        // {
        //     _direction *= -1f;
        //     _timer = _timePingToPong * 2f;
        // }

        float interpolation = Mathf.Cos(_timer / _timePingToPong);
        //print(interpolation);
        float x = _useX ? _targetX * interpolation: _shadow.effectDistance.x;
        float y = _useY ? _targetY * interpolation: _shadow.effectDistance.y;

        _shadow.effectDistance = new Vector2(x, y);
    }
}
