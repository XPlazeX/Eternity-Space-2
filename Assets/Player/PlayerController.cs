using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
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
    private Vector3 _lastMousePosition;
    private static Quaternion _cameraBorders; 

    public static bool CanControl {get; set;} = true;
    public static Vector3 DefaultForce {get; set;} = Vector3.zero;
    public static Vector3 AdditiveForce {get; private set;} = Vector3.zero;
    public static bool IsControlling {get; private set;} = false;

    public static void Initialize() 
    {        
        CanControl = true;
        
        Camera.main.GetComponent<CameraController>().BordersChange += SetBorders;
        SetBorders();

        BeginDrag += TimeHandler.Recover;
        EndDrag += TimeHandler.SlowDown;

        ReplacePlayer(Player.PlayerTransform);
        //print(_player == null);
    }

    private void Update()
    {
        if (_player == null || !Player.Alive)
            return;

        if (AdditiveForce.magnitude > 0f)
        {
            AdditiveForce = Vector3.Lerp(AdditiveForce, Vector3.zero, _forceDecelerration * Time.deltaTime);

            if (AdditiveForce.magnitude < 0.03f)
            {
                AdditiveForce = Vector3.zero;
            }
        }

        // if (!IsControlling)
        // {
        _player.position += (DefaultForce + AdditiveForce) * Time.deltaTime;
        ClampPosition();

        if ((DefaultForce + AdditiveForce).magnitude == 0)
        {
            _trailForce.gameObject.SetActive(false);
            return;
        } else{
            _trailForce.gameObject.SetActive(true);
            _trailForce.time = _trailTimeMultiplier * (AdditiveForce.magnitude + DefaultForce.magnitude);
            _trailForce.transform.position = _player.position;
        }
    }

    public static void AddImpulse(Vector2 direction, float forceScale)
    {
        AdditiveForce += (new Vector3(direction.x, direction.y, 0f) * forceScale);
    }

    private void OnDisable() {
        BeginDrag -= TimeHandler.Recover;
        EndDrag -= TimeHandler.SlowDown;
    }

    public void ChangeSensivity(float val) => _sensivity = val;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanControl)
            return;
        IsControlling = true;
        BeginDrag?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CanControl)
            return;
        IsControlling = false;
        EndDrag?.Invoke();
    }

    private static void SetBorders(float empty = 0)
    {
        _cameraBorders = CameraController.Borders_xXyY;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 dragDelta = new Vector3 (eventData.delta.x, eventData.delta.y, 0) * _sensivity * Time.deltaTime;
        Dragging?.Invoke(dragDelta);

        if (!CanControl)
            return;

        if (Time.timeScale != 0)
        {
            _player.position += dragDelta;
           
            ClampPosition();
        }
    }

    public static void ReplacePlayer(Transform newPlayer) => _player = newPlayer;

    public void ClampPosition()
    {
        _player.position = new Vector3 
            (
                Mathf.Clamp(_player.position.x, _cameraBorders.x, _cameraBorders.y),
                Mathf.Clamp(_player.position.y, _cameraBorders.z, _cameraBorders.w),
                _player.position.z
            );
    }

}
