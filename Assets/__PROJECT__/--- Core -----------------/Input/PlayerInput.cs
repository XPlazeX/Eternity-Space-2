using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private const float SCROLL_LAG_TIME = 0.3f;

    public static bool MainFireDown {get; private set;}
    public static bool MainFireUp {get; private set;}
    public static bool MainFirePressed {get; private set;}

    public static bool SecondaryFireDown {get; private set;}
    public static bool SecondaryFireUp {get; private set;}
    public static bool SecondaryFirePressed {get; private set;}

    public static bool SelectionDown {get; private set;}
    public static bool SelectionUp {get; private set;}
    public static bool SelectionPressed {get; private set;}

    public static bool SelectionScrolledUp {get; private set;}
    public static bool SelectionScrolledDown {get; private set;}

    public static bool NextReleased {get; private set;}
    public static bool PauseReleased {get; private set;}

    public static Vector2 PointerDelta {get; private set;}
    public static Vector3 PointerWorldPosition {get; private set;}
    public static Vector2 PointerDrag {get; private set;}

    private static Camera MainCamera {get; set;}

    [SerializeField] private Camera mainCamera;

    private float _scrollLag = 0.5f;
    private float _scrollDirection;

    private void Awake() 
    {
        MainCamera = mainCamera;
    }

    private void Update() 
    {
        MainFireDown = Input.GetMouseButtonDown(0);  
        MainFireUp = Input.GetMouseButtonUp(0);  
        MainFirePressed = Input.GetMouseButton(0);  

        SecondaryFireDown = Input.GetMouseButtonDown(1);  
        SecondaryFireUp = Input.GetMouseButtonUp(1);  
        SecondaryFirePressed = Input.GetMouseButton(1);  

        SelectionDown = Input.GetMouseButtonDown(2);  
        SelectionUp = Input.GetMouseButtonUp(2);  
        SelectionPressed = Input.GetMouseButton(2);  

        bool control = Input.GetAxisRaw("Mouse ScrollWheel") != 0;

        bool upReleased = control && Input.GetAxisRaw("Mouse ScrollWheel") > 0 && (_scrollDirection > 0 ? _scrollLag <= 0f : true);
        bool downReleased = control && Input.GetAxisRaw("Mouse ScrollWheel") < 0 && (_scrollDirection < 0 ? _scrollLag <= 0f : true);

        if (upReleased || downReleased)
        {
            _scrollDirection = Input.GetAxisRaw("Mouse ScrollWheel");
        }
        
        if (control)
        {
            _scrollLag = SCROLL_LAG_TIME;
        }

        NextReleased = !SelectionPressed && upReleased;
        PauseReleased = !SelectionPressed && downReleased;

        SelectionScrolledUp = SelectionPressed && upReleased;
        SelectionScrolledDown = SelectionPressed && downReleased;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        PointerWorldPosition = new Vector3(mousePos.x, mousePos.y, 0f);

        PointerDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        PointerDrag = MainFirePressed ? PointerDelta : Vector2.zero;

        _scrollLag -= ESTime.worldDeltaTime;
    }
}
