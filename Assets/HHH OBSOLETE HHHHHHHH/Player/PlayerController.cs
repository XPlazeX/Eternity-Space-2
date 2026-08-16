using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public delegate void dragEvent();
    public delegate void drag(Vector3 deltaPosition);
    public static event dragEvent BeginDrag;
    public static event dragEvent EndDrag;
    public static event drag Dragging;

    [SerializeField] private float _sensivity = 1f;
    [SerializeField] private float _forceDecelerration = 1f;
    [SerializeField] private TrailRenderer _trailForce;
    [SerializeField] private float _trailTimeMultiplier = 1f;
    [SerializeField] private float _trailFadeSpeed = 1f;
    [Header("Joystic control")]
    [SerializeField] private bool useJoystickControl = true;
    [SerializeField] private float joystickSensivity = 1f;
    [SerializeField] private float joystickDeadAmplitude = 0.01f;
    [SerializeField] private float joystickMaxAmplitude = 2f;
    [Header("Limited Speed")]
    [SerializeField] private bool limitedSpeed;
    [SerializeField] private float speedLimit = 5f;
    [SerializeField] private float maneuverability = 25f;
    [SerializeField] private float stopManeuverability = 35f; // можно сделать отдельную "тормозную" маневренность
    [SerializeField] private float dragVelocityCacheTime = 0.05f;

    private static Transform _player;
    private static Rigidbody2D _playerRb;
    // private static Quaternion _cameraBorders;

    private Vector2 _pendingDragDelta;
    private float _pendingDragTime;
    private Vector2 _lastDragVelocity;
    private float _lastDragVelocityTimer;
    private bool _clutch;
    // private bool _pendingEndDrag;
    private Vector2 _currentMove;
    private Vector2 _joystickVector;

    private static Vector3 _previousFixedPosition;

    public static bool CanControl { get; set; } = true;
    public static Vector3 DefaultForce { get; set; } = Vector3.zero;
    public static Vector3 AdditiveForce { get; private set; } = Vector3.zero;
    public static Vector3 FixedDeltaPosition => _playerRb != null ? (Vector3)_playerRb.position - _previousFixedPosition : Vector3.zero;
    // public static float RelativityMultiplier {get; private set;} = 1f;
    public static bool IsControlling { get; private set; } = false;
    // public static float MovingOffset {get; private set;}
    public static float RelativeRelativity => _player == null ? 0f : ArenaLocal.GetRelativityAtPoint(_player.position);
    public static Vector2 RelativeJoystickPosition {get; private set;} = Vector2.zero;


    private static PlayerController instance;

    public static void Initialize()
    {
        ReplacePlayer(Player.PlayerTransform);
    }

    void OnEnable()
    {
        instance = this;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            ResetMovementState();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            ResetMovementState();
    }

    private void Start() {
        _clutch = true;
    }

    private void Update()
    {
        if (_player == null || _playerRb == null || !Player.Alive)
            return;

        if (!CanControl)
            return;

        if (PlayerInput.NextReleased)
        {
            _joystickVector = Vector2.zero;
        }

        if (ESTime.worldTimeScale != 0f)
        {
            Vector2 frameDrag = ((Player.PlayerTransform == null ? Quaternion.identity : Player.PlayerTransform.rotation) * PlayerInput.PointerDrag) * _sensivity;

            if (frameDrag.sqrMagnitude > 0f)
            {
                _pendingDragDelta += frameDrag;
                _pendingDragTime += ESTime.worldDeltaTime;
                Dragging?.Invoke(frameDrag);
            }

            if (useJoystickControl && IsControlling && !PlayerInput.SelectionPressed)
            {
                Vector2 joystickDrag = ((Player.PlayerTransform == null ? Quaternion.identity : Player.PlayerTransform.rotation) * PlayerInput.PointerDelta) * joystickSensivity;

                _joystickVector += joystickDrag;

                if (_joystickVector.magnitude > joystickMaxAmplitude)
                {
                    _joystickVector = _joystickVector.normalized * joystickMaxAmplitude;
                }
            }
        }

        RelativeJoystickPosition = _joystickVector.normalized * (_joystickVector.magnitude / joystickMaxAmplitude);
    }

    private void FixedUpdate()
    {
        _previousFixedPosition = _playerRb != null ? (Vector3)_playerRb.position : Vector3.zero;

        if (_player == null || _playerRb == null || !Player.Alive)
            return;

        if (!CanControl)
        {
            _pendingDragDelta = Vector2.zero;
            _pendingDragTime = 0f;
            _lastDragVelocity = Vector2.zero;
            _lastDragVelocityTimer = 0f;
            _currentMove = Vector2.zero;
            AdditiveForce = Vector3.zero;
            _joystickVector = Vector2.zero;
            return;
        }

        HandleControlState();
        TickAdditiveForce();
        if (limitedSpeed)
        {
            TickLimitedMovement(Time.fixedUnscaledDeltaTime);
        } else
        {
            TickMovement();
        }

        _pendingDragDelta = Vector2.zero;
        _pendingDragTime = 0f;
    }

    private void LateUpdate()
    {
        if (_player == null)
            return;

        UpdateTrailVisual();
    }


    private void HandleControlState()
    {
        if (_clutch)
        {
            IsControlling = true;
        }
        else
        {
            IsControlling = false;
            _lastDragVelocity = Vector2.zero;
            _lastDragVelocityTimer = 0f;
        }
    }

    private void TickAdditiveForce()
    {
        if (AdditiveForce.sqrMagnitude <= 0f)
            return;

        AdditiveForce = Vector3.Lerp(
            AdditiveForce,
            Vector3.zero,
            _forceDecelerration * ESTime.worldFixedDeltaTime);

        if (AdditiveForce.sqrMagnitude < 0.03f * 0.03f)
            AdditiveForce = Vector3.zero;
    }

    private void TickMovement()
    {
        Vector2 currentPos = _playerRb.position;

        Vector2 forceMove = (Vector2)(DefaultForce + AdditiveForce) * ESTime.worldFixedDeltaTime;
        Vector2 dragMove = CanControl && ESTime.worldTimeScale != 0f ? _pendingDragDelta : Vector2.zero;
        Vector2 move = forceMove + dragMove;

        move = ArenaLocal.RelativeVectorAtPoint(move, currentPos, 1f, false);

        Vector2 nextPos = currentPos + move;
        nextPos = ClampPosition(nextPos);

        _playerRb.MovePosition(nextPos);
    }

    private void TickLimitedMovement(float fdt)
    {
        Vector2 currentPos = _playerRb.position;
        Vector2 targetMove = Vector2.zero;
        Vector2 forceMove = (Vector2)(DefaultForce + AdditiveForce) * ESTime.worldFixedDeltaTime;

        if (!useJoystickControl)
        {
            Vector2 dragVelocity = Vector2.zero;

            if (CanControl && ESTime.worldTimeScale != 0f)
            {
                if (_pendingDragTime > 0f)
                {
                    dragVelocity = _pendingDragDelta / _pendingDragTime;
                    _lastDragVelocity = dragVelocity;
                    _lastDragVelocityTimer = dragVelocityCacheTime;
                }
                else if (PlayerInput.MainFirePressed && _lastDragVelocityTimer > 0f)
                {
                    dragVelocity = _lastDragVelocity;
                    _lastDragVelocityTimer -= fdt;
                }
                else
                {
                    _lastDragVelocity = Vector2.zero;
                    _lastDragVelocityTimer = 0f;
                }
            }

            Vector2 dragMove = dragVelocity * fdt;
            targetMove = forceMove + dragMove;
        }

        else
        {
            Vector2 joystickVelocity = Vector2.zero;

            if (CanControl && ESTime.worldTimeScale != 0f)
            {
                joystickVelocity = _joystickVector;
            }
            if (joystickVelocity.magnitude < joystickDeadAmplitude)
            {
                joystickVelocity = Vector2.zero;
            }

            Vector2 joystickMove = joystickVelocity / Mathf.Max(joystickMaxAmplitude, 0.0001f) * speedLimit * fdt;
            targetMove = forceMove + joystickMove;
        }

        if (targetMove.magnitude > speedLimit * fdt)
        {
            targetMove = targetMove.normalized * speedLimit * fdt;
        }

        targetMove = ArenaLocal.RelativeVectorAtPoint(targetMove, currentPos, 1f, false);

        float currentManeuverability = targetMove.sqrMagnitude > _currentMove.sqrMagnitude
            ? maneuverability
            : stopManeuverability;

        _currentMove = Vector2.MoveTowards(
            _currentMove,
            targetMove,
            currentManeuverability * fdt
        );

        // _currentMove stores the actual displacement for this physics tick.
        // Keep it inside the same limit as targetMove even after a focus/pause hitch.
        _currentMove = Vector2.ClampMagnitude(_currentMove, speedLimit * fdt);

        Vector2 nextPos = currentPos + _currentMove;
        nextPos = ClampPosition(nextPos);

        _playerRb.MovePosition(nextPos);
    }

    private void UpdateTrailVisual()
    {
        Vector3 totalVelocity = DefaultForce + AdditiveForce;

        if (totalVelocity.sqrMagnitude <= 0f)
        {
            if (_trailForce.gameObject.activeSelf)
                _trailForce.gameObject.SetActive(false);

            return;
        }

        if (!_trailForce.gameObject.activeSelf)
            _trailForce.gameObject.SetActive(true);

        _trailForce.time = _trailTimeMultiplier * totalVelocity.magnitude;
        _trailForce.transform.position = _player.position;
    }

    public static void AddImpulse(Vector2 direction, float forceScale)
    {
        AdditiveForce += new Vector3(direction.x, direction.y, 0f) * forceScale;
    }

    private void ResetMovementState()
    {
        _pendingDragDelta = Vector2.zero;
        _pendingDragTime = 0f;
        _lastDragVelocity = Vector2.zero;
        _lastDragVelocityTimer = 0f;
        _currentMove = Vector2.zero;
        _joystickVector = Vector2.zero;

        if (_playerRb != null)
        {
            _playerRb.linearVelocity = Vector2.zero;
            _playerRb.angularVelocity = 0f;
        }
    }

    public void ChangeSensivity(float val) => _sensivity = 1f;

    public static void ReplacePlayer(Transform newPlayer)
    {
        _player = newPlayer;
        _playerRb = _player != null ? _player.GetComponent<Rigidbody2D>() : null;

        if (_playerRb != null)
        {
            _playerRb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }

    private Vector2 ClampPosition(Vector2 pos)
    {
        return ArenaLocal.ClampPosition(pos);
    }

    void OnDrawGizmosSelected()
    {
        // if (ArenaLocal.Pivot == null) return;
        // Gizmos.color = Color.orange;
        // Gizmos.DrawLine(new Vector3(ArenaLocal.WNegX - offsetOut, ArenaLocal.WPosY + offsetOut, 0f), new Vector3(ArenaLocal.WPosX + offsetOut, ArenaLocal.WPosY + offsetOut, 0f));
        // Gizmos.DrawLine(new Vector3(ArenaLocal.WNegX - offsetOut, ArenaLocal.WNegY - offsetOut, 0f), new Vector3(ArenaLocal.WPosX + offsetOut, ArenaLocal.WNegY - offsetOut, 0f));
        // Gizmos.DrawLine(new Vector3(ArenaLocal.WNegX - offsetOut, ArenaLocal.WNegY - offsetOut, 0f), new Vector3(ArenaLocal.WNegX - offsetOut, ArenaLocal.WPosY + offsetOut, 0f));
        // Gizmos.DrawLine(new Vector3(ArenaLocal.WPosX + offsetOut, ArenaLocal.WNegY - offsetOut, 0f), new Vector3(ArenaLocal.WPosX + offsetOut, ArenaLocal.WPosY + offsetOut, 0f));
    }
}
