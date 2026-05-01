using UnityEngine;
using UnityEngine.UI;

public class RotateByScrolling : MonoBehaviour
{
    [SerializeField] private Scrollbar _targetScrollbar;
    [SerializeField] private Transform _rotatingTransform;
    [SerializeField] private float _speedMultiplier = 0;

    private float _oldValue;

    private void Start() {
        _targetScrollbar.onValueChanged.AddListener(OnValueChanged);
        _oldValue = _targetScrollbar.value;
    }

    private void OnValueChanged(float val)
    {
        //float difference = (val - _oldValue) * _speedMultiplier;
        _rotatingTransform.Rotate(0, 0, (val - _oldValue) * _speedMultiplier);

        _oldValue = val;
    }
}
