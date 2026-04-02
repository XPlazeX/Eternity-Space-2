using System.Collections;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    public const float CAMERA_Z_POSITION = -10f;
    public event Action<float> ScaleChanged;
    public static event Action Moved;

    [Header("Main")]
    [SerializeField] private Transform movingParentTransform;
    [SerializeField] private Camera controllingCamera;
    [SerializeField] private bool unscaledTime;
    [SerializeField] private UpdateMode updateMode = UpdateMode.Update;
    [SerializeField] private bool debugFeatures = true;
    // [Header("2D Audio")]

    private float _defaultSize;

    private bool _moveTowards;
    private Vector3 _movePosition;
    private float _moveLerpSpeed;
    private float _maxMoveSpeed;
    private float _moveDeltaComplete;

    private bool _follow;
    private Transform _followTarget;
    private float _followSpeed;
    private Vector3 _followOffset;
    private float _predicationFactor;
    private Vector3 _oldFollowTargetPosition;

    private bool _scaling;
    private float _startScale;
    private float _targetScale;
    private float _scaleTime;
    private float _scaleTimer;
    private AnimationCurve _scaleCurve;

    private bool _shaking;

    private float _amplitude;
    private float _frequency;
    private float _duration;
    private float _elapsed;

    private float _positionWeight;
    private float _rotationWeight;
    private float _dampingPower;
    private float _rotationScaleDeg; // multiplier from amplitude to degrees

    private Vector3 _baseLocalPos;
    private float _baseLocalZRot;

    // Smooth noise state
    private Vector2 _nPosX, _nPosY, _nRot;
    private Vector2 _nPosXVel, _nPosYVel, _nRotVel;

    public Camera ControllingCamera => controllingCamera;

    public static CameraController instance;

    // private float _dumping = 6f;

    private void OnEnable()
    {
        _defaultSize = controllingCamera.orthographicSize;
        instance = this;
        StopMotion();
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    void Update()
    {
        if (updateMode == UpdateMode.Update) DoMove(DeltaTime());

        #if UNITY_EDITOR
        if (!debugFeatures) return;

        if (Input.GetKeyDown(KeyCode.S))
        {
            StartShaking(0.25f, 10f, 0.35f);
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            StartMoveTowards(controllingCamera.ScreenToWorldPoint(Input.mousePosition), 1f, 1f);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            StartScaling(1000f, 4f);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            ResetScale(4f);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            StopMotion();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            // StartFollowing(State.PlayerTransform, 5f, Vector3.zero, 0f);
        }
        #endif
    }

    private void LateUpdate() 
    {
        if (updateMode == UpdateMode.LateUpdate) DoMove(DeltaTime());
    }

    void FixedUpdate()
    {
        if (updateMode == UpdateMode.FixedUpdate) DoMove(FixedDeltaTime());
    }

    private void DoMove(float deltaTime)
    {
        if (_moveTowards) MovingTowards(deltaTime);
        if (_follow) Following(deltaTime);
        if (_scaling) Scaling(deltaTime);
        if (_shaking) Shaking(deltaTime);

        Moved?.Invoke();
    }

    public void ToggleCamera(bool tog)
    {
        controllingCamera.enabled = tog;
    }

    public void StopMotion()
    {
        StopMoveTowards();
        StopFollowing();
        StopScaling();
        StopShaking();
    }

    public void StartMoveTowards(Vector3 targetPosition, float lerpSpeed, float maxSpeed, float deltaComplete = 0.001f)
    {
        _movePosition = targetPosition;
        _moveLerpSpeed = lerpSpeed;
        _maxMoveSpeed = maxSpeed;
        _moveDeltaComplete = deltaComplete;
        _moveTowards = true;
    }

    public void StopMoveTowards()
    {
        _moveTowards = false;
    }

    private void MovingTowards(float deltaTime)
    {
        Vector3 lerpPosition = Vector3.Lerp(movingParentTransform.position, _movePosition, _moveLerpSpeed * deltaTime);
        Vector3 lerpDif = lerpPosition - movingParentTransform.position;
        if (lerpDif.magnitude <= _moveDeltaComplete)
        {
            StopMoveTowards();
            return;
        }
        if (lerpDif.magnitude > _maxMoveSpeed)
        {
            lerpDif = lerpDif.normalized * _maxMoveSpeed;
        }
        
        movingParentTransform.position = FlatVector(movingParentTransform.position + lerpDif);
    }

    public void StartFollowing(Transform followTransform, float followSpeed, Vector3 offset, float predication = 0f)
    {
        _followTarget = followTransform;
        _followSpeed = followSpeed;
        _followOffset = offset;
        _predicationFactor = predication;
        _oldFollowTargetPosition = followTransform.position;
        _follow = true;
    }

    public void StopFollowing()
    {
        _follow = false;
    }

    private void Following(float deltaTime)
    {
        if (_followTarget == null)
        {
            StopFollowing();
            return;
        }

        Vector3 prediction = (_followTarget.position - _oldFollowTargetPosition) * _predicationFactor;

        movingParentTransform.position = FlatVector(Vector3.Lerp(movingParentTransform.position, _followTarget.position + _followOffset + prediction, _followSpeed * deltaTime));
        _oldFollowTargetPosition = movingParentTransform.position;
    }

    public void StartScaling(float targetScale, float scaleTime) => StartScaling(targetScale, scaleTime, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));

    public void StartScaling(float targetScale, float scaleTime, AnimationCurve scaleCurve)
    {
        _startScale = controllingCamera.orthographicSize;
        _targetScale = targetScale;
        _scaleTime = scaleTime;
        _scaleCurve = scaleCurve;
        _scaleTimer = 0f;
        _scaling = true;
    }

    public void ResetScale(float scaleTime)
    {
        ResetScale(scaleTime, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));
    }

    public void ResetScale(float scaleTime, AnimationCurve animationCurve)
    {
        StartScaling(_defaultSize, scaleTime, animationCurve);
    }

    public void StopScaling()
    {
        _scaling = false;
    }

    private void Scaling(float deltaTime)
    {
        float elapsed = _scaleTimer / _scaleTime;

        if (elapsed > 1f) elapsed = 1f;

        controllingCamera.orthographicSize = Mathf.Lerp(_startScale, _targetScale, _scaleCurve.Evaluate(elapsed));

        _scaleTimer += deltaTime;
        if (elapsed >= 1f)
        {
            StopScaling();
        }
    }

    public static void Shake(float power)
    {
        instance.StartShaking(power, 10f, 0.35f);
    }

    public void StartShaking(
        float amplitude,
        float frequency,
        float duration,
        float positionWeight = 1.0f,
        float rotationWeight = 0f,
        float dampingPower = 2.0f,
        float rotationScaleDeg = 0f)
    {
        amplitude = Mathf.Max(0f, amplitude);
        frequency = Mathf.Max(0.01f, frequency);
        duration  = Mathf.Max(0f, duration);

        // If we're not currently shaking, capture baseline + init noise
        if (!_shaking)
        {
            _baseLocalPos = controllingCamera.transform.localPosition;
            _baseLocalZRot = controllingCamera.transform.localEulerAngles.z;

            _nPosX = new Vector2(UnityEngine.Random.value * 1000f, UnityEngine.Random.value * 1000f);
            _nPosY = new Vector2(UnityEngine.Random.value * 1000f, UnityEngine.Random.value * 1000f);
            _nRot  = new Vector2(UnityEngine.Random.value * 1000f, UnityEngine.Random.value * 1000f);
            _nPosXVel = _nPosYVel = _nRotVel = Vector2.zero;

            _elapsed = 0f;
            _amplitude = 0f;
            _frequency = 0f;
            _duration  = 0f;
        }

        // Store current tuning (last call wins). Usually what you want.
        _positionWeight = positionWeight;
        _rotationWeight = rotationWeight;
        _dampingPower = Mathf.Max(0.01f, dampingPower);
        _rotationScaleDeg = Mathf.Max(0f, rotationScaleDeg);

        // === STACKING RULES ===
        _amplitude = Mathf.Max(_amplitude, amplitude);
        _frequency = Mathf.Max(_frequency, frequency);

        float remaining = Mathf.Max(0f, _duration - _elapsed);
        _duration = Mathf.Max(remaining, duration);
        _elapsed = 0f;

        _shaking = _duration > 0f && _amplitude > 0f;
    }

    public void StopShaking()
    {
        if (!_shaking) return;

        _shaking = false;
        _amplitude = _frequency = _duration = _elapsed = 0f;

        controllingCamera.transform.localPosition = FlatPosition(_baseLocalPos);
        controllingCamera.transform.localRotation = Quaternion.Euler(0f, 0f, _baseLocalZRot);
    }

    private void Shaking(float dt)
    {
        _elapsed += dt;

        float t = (_duration <= 0f) ? 1f : Mathf.Clamp01(_elapsed / _duration);
        float damper = Mathf.Pow(1f - t, _dampingPower);

        float step = _frequency * dt;

        float nx = SmoothNoise01(ref _nPosX, ref _nPosXVel, step) * 2f - 1f;
        float ny = SmoothNoise01(ref _nPosY, ref _nPosYVel, step) * 2f - 1f;
        float nr = SmoothNoise01(ref _nRot,  ref _nRotVel,  step) * 2f - 1f;

        float posAmp = _amplitude * _positionWeight * damper;
        Vector3 offset = new Vector3(nx, ny, 0f) * posAmp;

        float rotAmpDeg = _amplitude * _rotationWeight * _rotationScaleDeg * damper;
        float zRot = _baseLocalZRot + nr * rotAmpDeg;

        controllingCamera.transform.localPosition = FlatPosition(_baseLocalPos + offset);
        controllingCamera.transform.localRotation = Quaternion.Euler(0f, 0f, zRot);

        if (_elapsed >= _duration)
            StopShaking();
    }

    // Returns smooth 0..1 noise that evolves pleasantly over time.
    private static float SmoothNoise01(ref Vector2 seed, ref Vector2 seedVel, float step)
    {
        Vector2 target = seed + new Vector2(step, step * 0.73f);

        // Keeps it smooth across frame rates & frequencies
        float smoothTime = Mathf.Max(0.02f, 0.12f / Mathf.Max(0.0001f, step * 60f));

        seed = Vector2.SmoothDamp(seed, target, ref seedVel, smoothTime);
        return Mathf.PerlinNoise(seed.x, seed.y);
    }
    
    private Vector3 FlatPosition(Vector3 v3) => new Vector3(v3.x, v3.y, CAMERA_Z_POSITION);
    private Vector3 FlatVector(Vector3 v3) => new Vector3(v3.x, v3.y, 0f);
    private float DeltaTime() => unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
    private float FixedDeltaTime() => unscaledTime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;
}

public enum UpdateMode
{
    Update = 0,
    LateUpdate = 1,
    FixedUpdate = 2
}