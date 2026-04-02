using UnityEngine;

[AddComponentMenu("Camera/Camera Objects")]
public class CameraObjects : MonoBehaviour
{
    Transform cameraTransform;
    [Header("Ближе - Основной слой - Дальше")]
    [Range(-1f, 1f)]
    public float paralaxSpeed;

    [SerializeField] private bool _notChangeScale = false;

    float lastCameraX;
    float lastCameraY;
    Vector3 _defaultSize;
    float _cameraScale = 1f;

    void Awake()
    {
        cameraTransform = Camera.main.transform;
        lastCameraX = cameraTransform.position.x;
        lastCameraY = cameraTransform.position.y;

        // if (Camera.main.GetComponent<CameraController>())
        //     Camera.main.GetComponent<CameraController>().ChangingScale += ChangeScale;

        _defaultSize = transform.localScale;
    }

    private void OnEnable() {
        CameraController.Moved += OnMoved;
    }

    void OnDisable()
    {
        CameraController.Moved -= OnMoved;
    }

    void OnMoved(){
        float deltaX = cameraTransform.position.x - lastCameraX;
        float deltaY = cameraTransform.position.y - lastCameraY;
        transform.position += new Vector3 (deltaX * (paralaxSpeed + ((1f - paralaxSpeed) - (1f - paralaxSpeed) / _cameraScale)), deltaY * (paralaxSpeed + ((1f - paralaxSpeed) - (1f - paralaxSpeed) / _cameraScale)), 0);
        lastCameraX = cameraTransform.position.x;
        lastCameraY = cameraTransform.position.y;        
    }

    private void ChangeScale(float newScale)
    {
        if (_notChangeScale)
            return;
            
        _cameraScale = newScale;

        transform.localScale = _defaultSize * (1f + ((newScale - 1f) * (paralaxSpeed * paralaxSpeed)));
        
        Vector3 oldPosition = transform.position;
        Vector3 byCameraPosition = transform.position - cameraTransform.position;

        transform.position = cameraTransform.position + (byCameraPosition / (newScale / _cameraScale));
        
        // float yProportion = transform.localScale.y / transform.localScale.x;
        // Vector3 localScale = transform.localScale;

        // transform.localScale = new Vector3((localScale.x * addingValue), (localScale.y * (addingValue * yProportion)), 1f);
    }

}
