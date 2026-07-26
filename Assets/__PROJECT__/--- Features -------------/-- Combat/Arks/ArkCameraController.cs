using UnityEngine;

public class ArkCameraController : MonoBehaviour
{
    [SerializeField] private Camera controllingCamera;
    [SerializeField] private Transform followingTransform;
    [SerializeField] private float followSpeed;

    private bool _active;
    private float _z;

    private void OnEnable() {
        _z = controllingCamera.transform.position.z;
        Deactivate();
    }
    
    public void Activate()
    {
        controllingCamera.transform.position = followingTransform.position;
        controllingCamera.enabled = true;
        _active = true;
    }

    public void Deactivate()
    {
        controllingCamera.enabled = false;
        _active = false;
    }

    void LateUpdate()
    {
        if (_active)
        {
            Vector3 targetPosition = Vector3.Lerp(controllingCamera.transform.position, followingTransform.position, followSpeed * ESTime.arkDeltaTime);
            controllingCamera.transform.position = new Vector3(targetPosition.x, targetPosition.y, _z);
        }
    }
}
