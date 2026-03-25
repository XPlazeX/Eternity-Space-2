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
    [SerializeField] private Vector3 _offset_XyY;
    [SerializeField] private float _forceDecelerration = 1f;
    [SerializeField] private TrailRenderer _trailForce;
    [SerializeField] private float _trailTimeMultiplier = 1f;
    [SerializeField] private float _trailFadeSpeed = 1f;

    private static Transform _player;
    private static Rigidbody2D _playerRb;
    private static Quaternion _cameraBorders;

    private Vector2 _pendingDragDelta;
    private bool _pendingBeginDrag;
    private bool _pendingEndDrag;

    public static bool CanControl { get; set; } = true;
    public static Vector3 DefaultForce { get; set; } = Vector3.zero;
    public static Vector3 AdditiveForce { get; private set; } = Vector3.zero;
    public static bool IsControlling { get; private set; } = false;

    public static void Initialize()
    {
        CanControl = true;

        Camera.main.GetComponent<CameraController>().BordersChange += SetBorders;
        SetBorders();

        ReplacePlayer(Player.PlayerTransform);
    }

    private void Update()
    {
        if (_player == null || _playerRb == null || !Player.Alive)
            return;

        if (!CanControl)
            return;

        if (PlayerInput.MainFireDown)
            _pendingBeginDrag = true;

        if (PlayerInput.MainFireUp)
            _pendingEndDrag = true;

        if (Time.timeScale != 0f)
        {
            Vector2 frameDrag = PlayerInput.PointerDrag * _sensivity;
            if (frameDrag.sqrMagnitude > 0f)
            {
                _pendingDragDelta += frameDrag;
                Dragging?.Invoke(frameDrag);
            }
        }
    }

    private void FixedUpdate()
    {
        if (_player == null || _playerRb == null || !Player.Alive)
            return;

        HandleControlState();
        TickAdditiveForce();
        TickMovement();

        _pendingDragDelta = Vector2.zero;
    }

    private void LateUpdate()
    {
        if (_player == null)
            return;

        UpdateTrailVisual();
    }

    private void HandleControlState()
    {
        if (_pendingBeginDrag)
        {
            TimeHandler.Recover();

            if (!IsControlling)
                BeginDrag?.Invoke();

            IsControlling = true;
            _pendingBeginDrag = false;
        }

        if (_pendingEndDrag)
        {
            if (IsControlling)
                EndDrag?.Invoke();

            IsControlling = false;
            TimeHandler.SlowDown();
            _pendingEndDrag = false;
        }
    }

    private void TickAdditiveForce()
    {
        if (AdditiveForce.sqrMagnitude <= 0f)
            return;

        AdditiveForce = Vector3.Lerp(
            AdditiveForce,
            Vector3.zero,
            _forceDecelerration * Time.fixedDeltaTime);

        if (AdditiveForce.sqrMagnitude < 0.03f * 0.03f)
            AdditiveForce = Vector3.zero;
    }

    private void TickMovement()
    {
        Vector2 currentPos = _playerRb.position;

        Vector2 forceMove = (Vector2)(DefaultForce + AdditiveForce) * Time.fixedDeltaTime;
        Vector2 dragMove = CanControl && Time.timeScale != 0f ? _pendingDragDelta : Vector2.zero;

        Vector2 nextPos = currentPos + forceMove + dragMove;
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

    private void OnDisable()
    {
        BeginDrag -= TimeHandler.Recover;
        EndDrag -= TimeHandler.SlowDown;
    }

    public void ChangeSensivity(float val) => _sensivity = val;

    private static void SetBorders(float empty = 0)
    {
        _cameraBorders = CameraController.Borders_xXyY;
    }

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
        return new Vector2(
            Mathf.Clamp(pos.x, _cameraBorders.x, _cameraBorders.y),
            Mathf.Clamp(pos.y, _cameraBorders.z, _cameraBorders.w)
        );
    }
}