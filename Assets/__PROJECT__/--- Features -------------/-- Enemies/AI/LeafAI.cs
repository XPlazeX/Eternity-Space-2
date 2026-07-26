using UnityEngine;

public class LeafAI : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private Vector3 _targetLocalUp;

    private Transform _targetTransform;
    private Vector3 _targetOffset;
    //private Vector3 _targetUp;

    private void Start() {
        _targetTransform = transform.parent;
        _targetOffset = _targetTransform.position - transform.position;
        //_targetUp = -_targetOffset;

        //print(_targetUp);

        transform.SetParent(null);
    }

    private void FixedUpdate() {
        if (_targetTransform == null)
            return;
            
        transform.position = _targetTransform.position + (_targetTransform.rotation * _targetOffset);

        RotateToTarget();
    }

    private void RotateToTarget()
    {
        transform.up = SceneStatics.FlatVector(Vector3.Lerp(transform.up, _targetTransform.rotation * _targetLocalUp, _rotationSpeed * ESTime.worldDeltaTime));

        CorrectRotation();
    }

    protected void CorrectRotation()
    {
        if (transform.rotation.eulerAngles.y != 180 && transform.rotation .eulerAngles.y != -180)
            return;

        transform.rotation = Quaternion.Euler(0, 0, 180);
    }
}
