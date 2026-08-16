using UnityEngine;

[AddComponentMenu("Camera/Camera Objects")]
public class CameraObjects : MonoBehaviour
{
    private const float MIN_CAMERA_SCALE = 0.0001f;

    private Transform cameraTransform;

    [Header("Ближе - Основной слой - Дальше")]
    [Range(-1f, 1f)]
    public float paralaxSpeed;

    [SerializeField] private bool _notChangeScale = false;
    [SerializeField] private Camera customCamera;

    private Vector3 _defaultPosition;
    private Vector3 _defaultSize;
    private Vector3 _baseCameraPosition;
    private Quaternion _baseCameraRotation;
    private float _baseCameraScale = 1f;
    private float _cameraScale = 1f;

    private Vector3 _lastAppliedPosition;
    private bool _hasAppliedState;
    private Vector3 _lastMovedCameraPosition;
    private float _lastScale = 1f;

    public float LocalParallaxMultiplier { get; private set; } = 1f;

    void Awake()
    {
        CacheDefaultState();
    }

    private void OnEnable() {
        if (cameraTransform == null)
            CacheDefaultState();

        CameraController.Moved += OnMoved;
        CameraController.ScaleChanged += ChangeScale;
    }

    void OnDisable()
    {
        CameraController.Moved -= OnMoved;
        CameraController.ScaleChanged -= ChangeScale;
    }

    void OnMoved(){
        ApplyCameraState();
    }

    private void ChangeScale(float oldScale, float newScale)
    {
        if (_notChangeScale)
            return;

        _cameraScale = newScale;
        ApplyCameraState();
    }

    private void CacheDefaultState()
    {
        Camera mainCamera = CameraController.instance.ControllingCamera;
        if (customCamera != null)
        {
            mainCamera = customCamera;
        } 

        if (mainCamera == null)
            return;

        cameraTransform = mainCamera.transform;
        _defaultPosition = transform.position;
        _defaultSize = transform.localScale;
        _baseCameraPosition = cameraTransform.position;
        _baseCameraRotation = cameraTransform.rotation;
        _baseCameraScale = Mathf.Max(mainCamera.orthographicSize, MIN_CAMERA_SCALE);
        _cameraScale = _baseCameraScale;
        LocalParallaxMultiplier = GetLocalParallaxMultiplier(1f);
        _lastAppliedPosition = transform.position;
        _hasAppliedState = true;
        _lastScale = transform.localScale.x;
        _lastMovedCameraPosition = cameraTransform.position;
    }

    private void ApplyCameraState()
    {
        if (cameraTransform == null)
            return;

        if (_hasAppliedState)
        {
            Vector3 externalDelta = transform.position - _lastAppliedPosition;
            _defaultPosition += externalDelta;
        }

        Vector3 cameraDelta = cameraTransform.position - _lastMovedCameraPosition;

        // SCALE BLOCK

        float scaleRatio = Mathf.Max(_cameraScale, MIN_CAMERA_SCALE) / _baseCameraScale;// отношение текущего размера камеры к изначальному
        LocalParallaxMultiplier = GetLocalParallaxMultiplier(scaleRatio);

        // float scaleParallax = Mathf.Clamp01(Mathf.Abs(paralaxSpeed));
        float objectScaleMultiplier = Mathf.Lerp(1f, scaleRatio, paralaxSpeed);

        if (!_notChangeScale)
            transform.localScale = _defaultSize * objectScaleMultiplier;

        float scaleRatioDelta = transform.localScale.x / _lastScale;
        Vector3 relativeByCameraPosition = transform.position - (Vector3)SceneStatics.FlatVector(cameraTransform.position);
        relativeByCameraPosition *= scaleRatioDelta;

        transform.position = (Vector3)SceneStatics.FlatVector(cameraTransform.position) + relativeByCameraPosition;


        // NORMAL BLOCK

        Vector3 relativeParallaxDelta = cameraDelta * (paralaxSpeed * LocalParallaxMultiplier);
        // Quaternion cameraRotationDelta = cameraTransform.rotation * Quaternion.Inverse(_baseCameraRotation);
        // Vector3 baseOffset = _defaultPosition - _baseCameraPosition; // относительный offset камеры
        // Vector3 followPosition 

        // Vector3 screenLockedPosition = cameraTransform.position + ((baseOffset * scaleRatio));//cameraRotationDelta * 

        transform.position += (Vector3)SceneStatics.FlatVector(relativeParallaxDelta);

        // transform.position = Vector3.LerpUnclamped(_defaultPosition, screenLockedPosition, LocalParallaxMultiplier);

        _lastScale = transform.localScale.x;
        _lastAppliedPosition = transform.position;
        _hasAppliedState = true;
        _lastMovedCameraPosition = cameraTransform.position;
    }

    private float GetLocalParallaxMultiplier(float scaleRatio)
    {
        if (Mathf.Approximately(paralaxSpeed, 0f))
            return 0f;

        if (scaleRatio <= MIN_CAMERA_SCALE)
            return paralaxSpeed;

        return 1f - ((1f - paralaxSpeed) / scaleRatio);
    }
}
