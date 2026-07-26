using UnityEngine;

public class SteeringWheel : MonoBehaviour
{
    [SerializeField] private Transform steeringTransform;

    [Header("Rotation")]
    [SerializeField] private float maxAngle = 10f;
    [SerializeField] private float inputSensitivity = 0.15f;

    [Header("Speed")]
    [SerializeField] private float steeringSpeed = 180f;
    [SerializeField] private float stabilizationSpeed = 120f;

    [Header("Weight")]
    [SerializeField, Min(0f)] private float inputSmoothTime = 0.16f;
    [SerializeField, Min(1f)] private float inputResponseCurve = 1.15f;
    [SerializeField, Min(0f)] private float inputRestThreshold = 0.01f;

    [Header("Input")]
    [SerializeField] private float deadZone = 0.01f;
    [SerializeField] private bool invertDirection;

    private float _currentAngle;
    private float _smoothedInputX;
    private float _inputSmoothVelocity;

    private void Reset()
    {
        steeringTransform = transform;
    }

    private void Update()
    {
        UpdateSteeringVisual(PlayerInput.PointerDelta);
    }

    private void UpdateSteeringVisual(Vector2 pointerDelta)
    {
        float deltaTime = ESTime.unscaledDeltaTime;

        float rawInputX = Mathf.Abs(pointerDelta.x) > deadZone 
            ? pointerDelta.x 
            : 0f;

        _smoothedInputX = Mathf.SmoothDamp(
            _smoothedInputX,
            rawInputX,
            ref _inputSmoothVelocity,
            inputSmoothTime,
            Mathf.Infinity,
            deltaTime
        );

        if (rawInputX == 0f && Mathf.Abs(_smoothedInputX) < inputRestThreshold)
        {
            _smoothedInputX = 0f;
            _inputSmoothVelocity = 0f;
        }

        float inputX = Mathf.Abs(_smoothedInputX) > deadZone
            ? _smoothedInputX
            : 0f;

        float targetAngle = 0f;

        if (inputX != 0f)
        {
            float angleLimit = Mathf.Max(Mathf.Abs(maxAngle), Mathf.Epsilon);
            float linearTargetAngle = Mathf.Clamp(
                inputX * inputSensitivity,
                -angleLimit,
                angleLimit
            );
            float normalizedAngle = Mathf.Abs(linearTargetAngle) / angleLimit;

            targetAngle = Mathf.Sign(linearTargetAngle) 
                * Mathf.Pow(normalizedAngle, inputResponseCurve) 
                * angleLimit;

            if (invertDirection)
                targetAngle *= -1f;
        }

        float speed = inputX != 0f 
            ? steeringSpeed 
            : stabilizationSpeed;

        _currentAngle = Mathf.MoveTowards(
            _currentAngle,
            targetAngle,
            speed * deltaTime
        );

        steeringTransform.localRotation = Quaternion.Euler(0f, 0f, _currentAngle);
    }
}
