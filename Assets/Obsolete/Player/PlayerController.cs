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

    // [Header("Arena")]
    // [SerializeField] private float offsetOut = 10f;
    // [SerializeField] private AnimationCurve relativityGrowth;
    // [SerializeField] private bool disableRelativity;

    private static Transform _player;
    private static Rigidbody2D _playerRb;
    // private static Quaternion _cameraBorders;

    private Vector2 _pendingDragDelta;
    private bool _pendingBeginDrag;
    private bool _pendingEndDrag;
    private Vector2 _currentMove;

    public static bool CanControl { get; set; } = true;
    public static Vector3 DefaultForce { get; set; } = Vector3.zero;
    public static Vector3 AdditiveForce { get; private set; } = Vector3.zero;
    // public static float RelativityMultiplier {get; private set;} = 1f;
    public static bool IsControlling { get; private set; } = false;
    // public static float MovingOffset {get; private set;}
    public static float RelativeRelativity => _player == null ? 0f : ArenaLocal.GetRelativityAtPoint(_player.position);
    // public static float RelativityX { get; private set; }
    // public static float RelativityY { get; private set; }
    // public static float AbsRelativity => Mathf.Max(Mathf.Abs(RelativityX), Mathf.Abs(RelativityY));


    private static PlayerController instance;

    public static void Initialize()
    {
        CanControl = true;

        // Camera.main.GetComponent<CameraController>().BordersChange += SetBorders;
        // SetBorders();

        ReplacePlayer(Player.PlayerTransform);
    }

    void OnEnable()
    {
        // MovingOffset = offsetOut;
        instance = this;

        // if (disableRelativity)
        // {
        //     RelativityMultiplier = 0f;
        // }
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

    // private void CalculateRelativity()
    // {
    //     RelativityX = 0f;
    //     RelativityY = 0f;

    //     if (_playerRb.position.x > ArenaLocal.WPosX)
    //         RelativityX = relativityGrowth.Evaluate((_playerRb.position.x - ArenaLocal.WPosX) / offsetOut) * RelativityMultiplier;
    //     else if (_playerRb.position.x < ArenaLocal.WNegX)
    //         RelativityX = -relativityGrowth.Evaluate((-_playerRb.position.x + ArenaLocal.WNegX) / offsetOut) * RelativityMultiplier;

    //     if (_playerRb.position.y > ArenaLocal.WPosY)
    //         RelativityY = relativityGrowth.Evaluate((_playerRb.position.y - ArenaLocal.WPosY) / offsetOut) * RelativityMultiplier;
    //     else if (_playerRb.position.y < ArenaLocal.WNegY)
    //         RelativityY = -relativityGrowth.Evaluate((-_playerRb.position.y + ArenaLocal.WNegY) / offsetOut) * RelativityMultiplier;
    // }

    // public static Vector3 RelativeVectorAtPoint(Vector3 moveDelta, Vector3 point, float relativeFactor)
    // {
    //     float xMult = 1f;
    //     float yMult = 1f;

    //     // X+
    //     float rightBarrier = ArenaLocal.WPosX + instance.offsetOut;
    //     xMult *= GetSideMultiplier(
    //         point.x,
    //         moveDelta.x,
    //         rightBarrier,
    //         rightBarrier - instance.offsetOut,
    //         rightBarrier + instance.offsetOut,
    //         relativeFactor * RelativityMultiplier,
    //         instance.relativityGrowth
    //     );

    //     // X-
    //     float leftBarrier = ArenaLocal.WNegX - instance.offsetOut;
    //     xMult *= GetSideMultiplier(
    //         point.x,
    //         moveDelta.x,
    //         leftBarrier,
    //         leftBarrier - instance.offsetOut,
    //         leftBarrier + instance.offsetOut,
    //         relativeFactor * RelativityMultiplier,
    //         instance.relativityGrowth
    //     );

    //     // Y+
    //     float topBarrier = ArenaLocal.WPosY + instance.offsetOut;
    //     yMult *= GetSideMultiplier(
    //         point.y,
    //         moveDelta.y,
    //         topBarrier,
    //         topBarrier - instance.offsetOut,
    //         topBarrier + instance.offsetOut,
    //         relativeFactor * RelativityMultiplier,
    //         instance.relativityGrowth
    //     );

    //     // Y-
    //     float bottomBarrier = ArenaLocal.WNegY - instance.offsetOut;
    //     yMult *= GetSideMultiplier(
    //         point.y,
    //         moveDelta.y,
    //         bottomBarrier,
    //         bottomBarrier - instance.offsetOut,
    //         bottomBarrier + instance.offsetOut,
    //         relativeFactor * RelativityMultiplier,
    //         instance.relativityGrowth
    //     );

    //     Vector3 result = new Vector3(
    //         moveDelta.x * xMult,
    //         moveDelta.y * yMult,
    //         moveDelta.z
    //     );

    //     // Жесткий запрет на пересечение только при полной "непроходимости"
    //     if ((relativeFactor  * RelativityMultiplier) >= 1f)
    //     {
    //         const float epsilon = 0.0001f;

    //         result.x = ClampDeltaAgainstBarrier(point.x, result.x, rightBarrier, epsilon);
    //         result.x = ClampDeltaAgainstBarrier(point.x, result.x, leftBarrier, epsilon);

    //         result.y = ClampDeltaAgainstBarrier(point.y, result.y, topBarrier, epsilon);
    //         result.y = ClampDeltaAgainstBarrier(point.y, result.y, bottomBarrier, epsilon);
    //     }

    //     return result;
    // }

    // private static float GetSideMultiplier(
    //     float pos,
    //     float delta,
    //     float barrier,
    //     float zoneMin,
    //     float zoneMax,
    //     float relativeFactor,
    //     AnimationCurve curve)
    // {
    //     if (Mathf.Approximately(delta, 0f))
    //         return 1f;

    //     if (pos < zoneMin || pos > zoneMax)
    //         return 1f;

    //     float currentDistance = Mathf.Abs(barrier - pos);
    //     float nextDistance = Mathf.Abs(barrier - (pos + delta));

    //     // Тормозим только при движении к границе
    //     if (nextDistance >= currentDistance)
    //         return 1f;

    //     float halfWidth = (zoneMax - zoneMin) * 0.5f;
    //     if (halfWidth <= 0.0001f)
    //         return 1f;

    //     float t = 1f - Mathf.Clamp01(currentDistance / halfWidth);

    //     float factor = curve.Evaluate(t) * relativeFactor;
    //     return Mathf.Clamp01(1f - factor);
    // }

    // private static float ClampDeltaAgainstBarrier(float pos, float delta, float barrier, float epsilon)
    // {
    //     if (Mathf.Approximately(delta, 0f))
    //         return delta;

    //     float nextPos = pos + delta;

    //     // Барьер справа/сверху относительно текущей позиции
    //     if (barrier > pos)
    //     {
    //         // Пытаемся пересечь барьер слева направо
    //         if (nextPos >= barrier)
    //             return Mathf.Max(0f, barrier - pos - epsilon);
    //     }
    //     else // барьер слева/снизу относительно текущей позиции
    //     {
    //         // Пытаемся пересечь барьер справа налево
    //         if (nextPos <= barrier)
    //             return Mathf.Min(0f, barrier - pos + epsilon);
    //     }

    //     return delta;
    // }

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

        // if ((move.x < 0f && RelativityX < 0f) || (move.x > 0f && RelativityX > 0f))
        //     move = new Vector2(move.x * (1f - Mathf.Abs(RelativityX)), move.y);

        // if ((move.y < 0f && RelativityY < 0f) || (move.y > 0f && RelativityY > 0f))
        //     move = new Vector2(move.x, move.y * (1f - Mathf.Abs(RelativityY)));
        move = ArenaLocal.RelativeVectorAtPoint(move, currentPos, 1f);

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

        // if ((targetMove.x < 0f && RelativityX < 0f) || (targetMove.x > 0f && RelativityX > 0f))
        //     targetMove.x *= 1f - Mathf.Abs(RelativityX);

        // if ((targetMove.y < 0f && RelativityY < 0f) || (targetMove.y > 0f && RelativityY > 0f))
        //     targetMove.y *= 1f - Mathf.Abs(RelativityY);
        targetMove = ArenaLocal.RelativeVectorAtPoint(targetMove, currentPos, 1f);

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

        // // чтобы не залипал в край экрана
        // if (nextPos.x != currentPos.x + _currentMove.x)
        //     _currentMove.x = 0f;

        // if (nextPos.y != currentPos.y + _currentMove.y)
        //     _currentMove.y = 0f;

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