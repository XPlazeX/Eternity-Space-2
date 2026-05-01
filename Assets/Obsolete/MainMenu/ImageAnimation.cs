using UnityEngine;
using UnityEngine.UI;

public class ImageAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] _frames;
    [SerializeField] private float _frameTime;

    private Image _img;
    private float _timer;
    private int _curFrame;

    private void Start() {
        _img = GetComponent<Image>();
    }

    private void FixedUpdate() {
        _timer -= Time.deltaTime;

        if (_timer < 0f)
        {
            _img.sprite = _frames[_curFrame];
            _curFrame ++;

            if (_curFrame == _frames.Length)
                _curFrame = 0;

            _timer = _frameTime;
        }
    }
}
