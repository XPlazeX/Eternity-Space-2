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
    [Header("Limited Speed")]
    [SerializeField] private bool limitedSpeed;
    [SerializeField] private float speedLimit = 5f;
    [SerializeField] private float maneuverability = 25f;
    [SerializeField] private float stopManeuverability = 35f; // можно сделать отдельную "тормозную" маневренность

    private static Transform _player;
    private static Rigidbody2D _playerRb;
    // private static Quaternion _cameraBorders;

    private Vector2 _pendingDragDelta;
    private bool _pendingBeginDrag;
    private bool _pendingEndDrag;
    private Vector2 _currentMove;

    private static Vector3 _previousFixedPosition;

    public static bool CanControl { get; set; } = true;
    public static Vector3 DefaultForce { get; set; } = Vector3.zero;
    public static Vector3 AdditiveForce { get; private set; } = Vector3.zero;
    public static Vector3 FixedDeltaPosition => _playerRb != null ? (Vector3)_playerRb.position - _previousFixedPosition : Vector3.zero;
    // public static float RelativityMultiplier {get; private set;} = 1f;
    public static bool IsControlling { get; private set; } = false;
    // public static float MovingOffset {get; private set;}
    public static float RelativeRelativity => _player == null ? 0f : ArenaLocal.GetRelativityAtPoint(_player.position);


    private static PlayerController instance;

    public static void Initialize()
    {
        ReplacePlayer(Player.PlayerTransform);
    }

    void OnEnable()
    {
        instance = this;
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
            Vector2 frameDrag = ((Player.PlayerTransform == null ? Quaternion.identity : Player.PlayerTransform.rotation) * PlayerInput.PointerDrag) * _sensivity;

            if (frameDrag.sqrMagnitude > 0f)
            {
                _pendingDragDelta += frameDrag;
                Dragging?.Invoke(frameDrag);
            }
        }
    }

    private void FixedUpdate()
    {
        _previousFixedPosition = _playerRb != null ? (Vector3)_playerRb.position : Vector3.zero;

        if (_player == null || _playerRb == null || !Player.Alive)
            return;

        if (!CanControl)
        {
            _pendingDragDelta = Vector2.zero;
            _pendingBeginDrag = false;
            _pendingEndDrag = false;
            _currentMove = Vector2.zero;
            AdditiveForce = Vector3.zero;
            return;
        }

        // CalculateRelativity();
        HandleControlState();
        TickAdditiveForce();
        if (limitedSpeed)
        {
            TickLimitedMovement(Time.fixedDeltaTime);
        } else
        {
            TickMovement();
        }

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
        Vector2 move = forceMove + dragMove;

        move = ArenaLocal.RelativeVectorAtPoint(move, currentPos, 1f, false);

        Vector2 nextPos = currentPos + move;
        nextPos = ClampPosition(nextPos);

        _playerRb.MovePosition(nextPos);
    }

    private void TickLimitedMovement(float fdt)
    {
        Vector2 currentPos = _playerRb.position;

        Vector2 forceMove = (Vector2)(DefaultForce + AdditiveForce) * Time.fixedDeltaTime;
        Vector2 dragMove = CanControl && Time.timeScale != 0f ? _pendingDragDelta : Vector2.zero;

        Vector2 targetMove = forceMove + dragMove;

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
            currentManeuverability * Time.fixedDeltaTime
        );

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

    // private void OnDisable()
    // {
    //     BeginDrag -= TimeHandler.Recover;
    //     EndDrag -= TimeHandler.SlowDown;
    // }

    public void ChangeSensivity(float val) => _sensivity = val;

    // private static void SetBorders(float empty = 0)
    // {
    //     _cameraBorders = CameraController.Borders_xXyY;
    // }

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
        // if ((pos - ArenaLocal.R))
        // return new Vector2(
        //     Mathf.Clamp(pos.x, ArenaLocal.WNegX - offsetOut, ArenaLocal.WPosX + offsetOut),
        //     Mathf.Clamp(pos.y, ArenaLocal.WNegY - offsetOut, ArenaLocal.WPosY + offsetOut)
        // );
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