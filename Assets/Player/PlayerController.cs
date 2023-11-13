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

    private static Transform _player;
    private Vector3 _lastMousePosition;
    private static Quaternion _cameraBorders; 

    public static bool CanControl {get; set;} = true;

    public static void Initialize() 
    {        
        CanControl = true;
        
        Camera.main.GetComponent<CameraController>().BordersChange += SetBorders;
        SetBorders();

        BeginDrag += TimeHandler.Recover;
        EndDrag += TimeHandler.SlowDown;

        //Player.PlayerChanged += FindPlayer;
        //FindPlayer();
        ReplacePlayer(Player.PlayerTransform);
        print(_player == null);
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
        BeginDrag?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CanControl)
            return;
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
           
            _player.position = new Vector3 
            (
                Mathf.Clamp(_player.position.x, _cameraBorders.x, _cameraBorders.y),
                Mathf.Clamp(_player.position.y, _cameraBorders.z, _cameraBorders.w),
                _player.position.z
            );

            //print(_cameraBorders.y);
        }
    }

    public static void ReplacePlayer(Transform newPlayer) => _player = newPlayer;

}
