using UnityEngine;

public class CameraObjects : MonoBehaviour
{
    Transform cameraTransform;
    [Range(-1f, 1f)]
    public float paralaxSpeed;

    [SerializeField] private bool _notChangeScale = false;

    float lastCameraX;
    float lastCameraY;

    void Awake(){
        cameraTransform = Camera.main.transform;
        lastCameraX = cameraTransform.position.x;
        lastCameraY = cameraTransform.position.y;

        if (Camera.main.GetComponent<CameraController>())
            Camera.main.GetComponent<CameraController>().ChangingScale += ChangeScale;
    }

    void LateUpdate(){
        float deltaX = cameraTransform.position.x - lastCameraX;
        float deltaY = cameraTransform.position.y - lastCameraY;
        transform.position += new Vector3 (deltaX * paralaxSpeed, deltaY * paralaxSpeed, 0);
        lastCameraX = cameraTransform.position.x;
        lastCameraY = cameraTransform.position.y;        
    }

    private void ChangeScale(float addingValue)
    {
        if (_notChangeScale)
            return;
            
        float yProportion = transform.localScale.y / transform.localScale.x;
        Vector3 localScale = transform.localScale;

        transform.localScale = new Vector3((localScale.x * addingValue), (localScale.y * (addingValue * yProportion)), 1f);
    }

}
