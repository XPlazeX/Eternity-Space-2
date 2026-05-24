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

    [Header("Input")]
    [SerializeField] private float deadZone = 0.01f;
    [SerializeField] private bool invertDirection;

    private float _currentAngle;

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
        float deltaTime = Time.unscaledDeltaTime;

        float inputX = Mathf.Abs(pointerDelta.x) > deadZone 
            ? pointerDelta.x 
            : 0f;

        float targetAngle = 0f;

        if (inputX != 0f)
        {
            targetAngle = Mathf.Clamp(
                inputX * inputSensitivity,
                -maxAngle,
                maxAngle
            );

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