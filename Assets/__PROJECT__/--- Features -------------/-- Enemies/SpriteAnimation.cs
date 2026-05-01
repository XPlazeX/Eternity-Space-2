using UnityEngine;

public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] _frames;
    [SerializeField] private float _frameTime;

    private SpriteRenderer _sr;
    private float _timer;
    private int _curFrame;

    private void Start() {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate() {
        _timer -= Time.deltaTime;

        if (_timer < 0f)
        {
            _sr.sprite = _frames[_curFrame];
            _curFrame ++;

            if (_curFrame == _frames.Length)
                _curFrame = 0;

            _timer = _frameTime;
        }
    }
}
