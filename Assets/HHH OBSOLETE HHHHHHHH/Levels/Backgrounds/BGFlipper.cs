using UnityEngine;

public class BGFlipper : MonoBehaviour
{
    [SerializeField] private float _flipTimeStep;

    private SpriteRenderer _bgSR;
    private float _flipTimer;
    private int _state = 0;

    private void Start() {
        _bgSR = GameObject.FindWithTag("BG-Background").GetComponent<SpriteRenderer>();
    }

    private void Update() {
        _flipTimer -= ESTime.unscaledDeltaTime;

        if (_flipTimer < 0f)
        {
            _state += 1;

            switch (_state)
            {
                case 0:
                    _bgSR.flipX = false;
                    _bgSR.flipY = false;
                    break;
                case 1:
                    _bgSR.flipX = false;
                    _bgSR.flipY = true;
                    break;
                case 2:
                    _bgSR.flipX = true;
                    _bgSR.flipY = true;
                    break;
                case 3:
                    _bgSR.flipX = true;
                    _bgSR.flipY = false;
                    break;
                default:
                    break;
            }

            if (_state >= 4)
                _state = -1;

            _flipTimer = _flipTimeStep;
        }
    }
}
