using UnityEngine;

public class UnlockCursorOnEnable : MonoBehaviour
{
    [SerializeField] private bool pauseTime;
    [SerializeField] private bool holdEnabledCursor;
    [SerializeField] private bool lockOnDisable;

    private void OnEnable() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pauseTime)
        {
            TimeHandler.Pause();
        }
    }

    void OnDisable()
    {
        if (lockOnDisable)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (pauseTime)
        {
            TimeHandler.Resume();
        }
    }

    void Update()
    {
        if (holdEnabledCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
