using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private static int _cursorRequests;

    private void Start() {
        _cursorRequests = 0;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void CursorRequest()
    {
        _cursorRequests ++;
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public static void CursorRelease()
    {
        _cursorRequests --;

        if (_cursorRequests <= 0)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
