using UnityEngine;

public class SledgeController : MonoBehaviour
{
    [SerializeField] private SledgeDirector sledgeDirector;
    [SerializeField] private SledgeBody sledgeBody;
    [SerializeField] private Rigidbody2D sledgeRb;
    [SerializeField] private float maxSpeedToDeploy = 1f;
    [SerializeField] private float maxAngularSpeedToDeploy = 2f;
    [Header("Movement")]
    [SerializeField] private float moveDrag = 1f;
    [SerializeField] private float forwardThrust = 10f;
    [SerializeField] private float backingThrust = 5f;
    [SerializeField] private float breakingThrust = 15f;
    [SerializeField] private float maxSpeed = 60f;
    [SerializeField] private float rotationDrag = 1f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float maxRotationSpeed = 50f;
    [SerializeField] private float notFullEnginePowerMultiplier = 0.4f;
    [Header("Visual")]
    [SerializeField] private GameObject sledgeCanvas;
    [SerializeField] private GameObject sledgeNavigator;

    public bool CanDeployPlayer => IsControlling 
        && _velocity.magnitude <= maxSpeedToDeploy
        && Mathf.Abs(_angularVelocity) <= maxAngularSpeedToDeploy;
    public bool IsControlling => sledgeDirector.IsSledgeControlling;
    public bool CanDeployBySpeed => _velocity.magnitude <= maxSpeedToDeploy;
    public bool CanDeployByAngularSpeed => Mathf.Abs(_angularVelocity) <= maxAngularSpeedToDeploy;
    public bool UsingForwardThrust => IsControlling && !_pendingBreakingInput && _pendingThrustInput > 0f;
    public bool UsingBackingThrust => IsControlling && !_pendingBreakingInput && _pendingThrustInput < 0f;
    public bool UsingBreaking => IsControlling && _pendingBreakingInput;
    public float Speed => _velocity.magnitude;
    public float AngularSpeed => Mathf.Abs(_angularVelocity);

    private float _pendingRotationInput;
    private float _pendingThrustInput;
    private bool _pendingBreakingInput;
    private Vector2 _velocity;
    private float _angularVelocity;

    private void OnEnable() {
        ArkDirector.ArkEntered += OnArkEntered;
        ArkDirector.ArkExited += OnArkExited;       
    }

    void OnDisable()
    {
        ArkDirector.ArkEntered -= OnArkEntered;
        ArkDirector.ArkExited -= OnArkExited;
    }

    public void Initialize()
    {
        sledgeCanvas.SetActive(false);
        sledgeNavigator.SetActive(false);
    }

    private void Start() {
        sledgeRb.gravityScale = 0f;
        sledgeRb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    public void StartControlling()
    {
        sledgeCanvas.SetActive(true);
        sledgeNavigator.SetActive(true);
    }

    void Update()
    {
        if (sledgeDirector.AutoDeployPlayer && CanDeployPlayer)
        {
            if (sledgeDirector.TryDeployPlayer())
            {
                _pendingRotationInput = 0f;
                _pendingThrustInput = 0f;
                _pendingBreakingInput = false;
                sledgeCanvas.SetActive(false);
                sledgeNavigator.SetActive(false);
                return;
            }
        }

        if (PlayerInput.NextReleased && CanDeployPlayer)
        {
            StopRotation();

            if (sledgeDirector.TryDeployPlayer())
            {
                _pendingRotationInput = 0f;
                _pendingThrustInput = 0f;
                _pendingBreakingInput = false;
                sledgeCanvas.SetActive(false);
                sledgeNavigator.SetActive(false);
                return;
            }
        }

        if (!IsControlling)
        {
            _pendingRotationInput = 0f;
            _pendingThrustInput = 0f;
            _pendingBreakingInput = false;
            return;
        }

        _pendingBreakingInput = PlayerInput.MainFirePressed && PlayerInput.SecondaryFirePressed;
        _pendingThrustInput = GetThrustInput();
        _pendingRotationInput += PlayerInput.PointerDelta.x;
    }

    void FixedUpdate()
    {
        float rotationInput = _pendingRotationInput;
        _pendingRotationInput = 0f;

        StabilizeMovement();
        StabilizeRotation();

        if (IsControlling)
        {
            if (_pendingBreakingInput)
            {
                ApplyBreaking();
            }
            else
            {
                ApplyThrust(_pendingThrustInput);
            }

            ApplyRotation(rotationInput);
        }

        ClampVelocity();
        ClampAngularVelocity();
        MoveSledge(ESTime.worldFixedDeltaTime);
    }

    private float GetThrustInput()
    {
        float thrustInput = 0f;

        if (PlayerInput.MainFirePressed)
        {
            thrustInput += 1f;
        }

        if (PlayerInput.SecondaryFirePressed)
        {
            thrustInput -= 1f;
        }

        return thrustInput;
    }

    private void ApplyThrust(float input)
    {
        if (input == 0f || !sledgeBody.EngineAvailiable)
            return;

        bool forward = input > 0f;
        // if (forward && sledgeBody.ForwardThrustersAvailable == false)
        //     return;

        // if (!forward && sledgeBody.BackingThrustersAvailable == false)
        //     return;

        Vector2 direction = forward ? transform.up : -transform.up;
        float speedInDirection = Vector2.Dot(_velocity, direction);
        if (speedInDirection >= maxSpeed)
            return;

        bool isFullPower = sledgeBody.FullEnginePowerAvailiable;

        float thrust = forward ? forwardThrust : backingThrust;
        if (!isFullPower) thrust *= notFullEnginePowerMultiplier;
        _velocity += direction * thrust * ESTime.worldFixedDeltaTime;
    }

    private void ApplyBreaking()
    {
        if (breakingThrust <= 0f || !sledgeBody.EngineAvailiable)
            return;

        _velocity = Vector2.MoveTowards(
            _velocity,
            Vector2.zero,
            breakingThrust * ESTime.worldFixedDeltaTime * (sledgeBody.FullEnginePowerAvailiable ? 1f : notFullEnginePowerMultiplier)
        );
    }

    private void ApplyRotation(float input)
    {
        if (input == 0f || !sledgeBody.EngineAvailiable)
            return;

        _angularVelocity += -input * (rotationSpeed * (sledgeBody.FullEnginePowerAvailiable ? 1f : notFullEnginePowerMultiplier));
    }

    private void StabilizeMovement()
    {
        if (moveDrag <= 0f)
            return;

        float t = 1f - Mathf.Exp(-moveDrag * ESTime.worldFixedDeltaTime);
        _velocity = Vector2.Lerp(_velocity, Vector2.zero, t);
    }

    private void StabilizeRotation()
    {
        if (rotationDrag <= 0f)
            return;

        float t = 1f - Mathf.Exp(-rotationDrag * ESTime.worldFixedDeltaTime);
        _angularVelocity = Mathf.Lerp(_angularVelocity, 0f, t);
    }

    private void ClampVelocity()
    {
        if (maxSpeed <= 0f)
            return;

        _velocity = Vector2.ClampMagnitude(_velocity, maxSpeed);
    }

    private void ClampAngularVelocity()
    {
        if (maxRotationSpeed <= 0f)
            return;

        _angularVelocity = Mathf.Clamp(_angularVelocity, -maxRotationSpeed, maxRotationSpeed);
    }

    private void MoveSledge(float dt)
    {
        sledgeRb.MovePosition(sledgeRb.position + _velocity * dt);
        sledgeRb.MoveRotation(sledgeRb.rotation + _angularVelocity * dt);
    }

    private void StopRotation()
    {
        _pendingRotationInput = 0f;
        _angularVelocity = 0f;
    }

    private void OnArkEntered()
    {
        if (IsControlling)
        {
            sledgeCanvas.SetActive(false);
            sledgeNavigator.SetActive(false);
        }
    }

    private void OnArkExited()
    {
        if (IsControlling)
        {
            sledgeCanvas.SetActive(true);
            sledgeNavigator.SetActive(true);
        }
    }
}
