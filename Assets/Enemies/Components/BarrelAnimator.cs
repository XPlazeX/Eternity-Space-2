using UnityEngine;

[RequireComponent(typeof(AttackModule))]
public class BarrelAnimator : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private float _strokeDistance;
    [SerializeField] private float _workSpeed;
    [SerializeField] private float _repairSpeed;
    [SerializeField] private float _deadzone = 0.075f;

    private Vector3 _startLocalPosition;
    private Vector3 _targetLocalPosition;
    private bool _working = false;

    private void OnEnable() 
    {
        GetComponent<AttackModule>().Fired += OnFired;
        _startLocalPosition = _targetTransform.localPosition;
        _targetLocalPosition = _startLocalPosition + _targetTransform.up.normalized * _strokeDistance;
    }

    private void FixedUpdate() 
    {
        if (_working && (_targetLocalPosition - _targetTransform.localPosition).magnitude > _deadzone)
        {
            _targetTransform.localPosition = Vector3.Lerp(_targetTransform.localPosition, _targetLocalPosition, _workSpeed * Time.fixedDeltaTime);
        } else if (_working)
        {
            _working = false;
        } else if (!_working && (_startLocalPosition - _targetTransform.localPosition).magnitude > _deadzone)
        {
            _targetTransform.localPosition = Vector3.Lerp(_targetTransform.localPosition, _startLocalPosition, _repairSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnDisable() 
    {
        GetComponent<AttackModule>().Fired -= OnFired;
    }

    private void OnFired()
    {
        _working = true;
    }
}
