using System.Collections.Generic;
using UnityEngine;

public class DesctopUtilsScript : MonoBehaviour
{
    private readonly List<DisplayInfo> _displays = new();
    private DisplayInfo _initialDisplay;
    private int _currentDisplayIndex;
    private int _windowedWidth;
    private int _windowedHeight;
    private bool _showStats;
    private int _frameCounter;
    private int _fixedUpdateCounter;
    private float _statsTimer;
    private float _fps;
    private float _fixedUpdatesPerSecond;

    void Start()
    {
        _windowedWidth = Screen.width;
        _windowedHeight = Screen.height;
        RefreshDisplays();
        _initialDisplay = Screen.mainWindowDisplayInfo;
        _currentDisplayIndex = GetDisplayIndex(_initialDisplay);

        QualitySettings.vSyncCount = 0;      // отключить VSync
        Application.targetFrameRate = -1;   // без программного лимита, как можно быстрее;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFullscreen();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            MoveToNextDisplay();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            _showStats = !_showStats;
        }

        if (Input.GetMouseButtonDown(2))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
        }

        UpdateStats();
    }

    private void FixedUpdate()
    {
        _fixedUpdateCounter++;
    }

    private void OnGUI()
    {
        if (!_showStats)
        {
            return;
        }

        GUI.Label(
            new Rect(16f, 16f, 320f, 80f),
            $"FPS: {_fps:F1}\nFixedUpdate/s: {_fixedUpdatesPerSecond:F1}\nfixedDeltaTime: {ESTime.worldFixedDeltaTime:F5}");
    }

    private void UpdateStats()
    {
        _frameCounter++;
        _statsTimer += ESTime.unscaledDeltaTime;

        if (_statsTimer < 1f)
        {
            return;
        }

        _fps = _frameCounter / _statsTimer;
        _fixedUpdatesPerSecond = _fixedUpdateCounter / _statsTimer;
        _frameCounter = 0;
        _fixedUpdateCounter = 0;
        _statsTimer = 0f;
    }

    private void ToggleFullscreen()
    {
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            _windowedWidth = Screen.width;
            _windowedHeight = Screen.height;
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
            return;
        }

        _windowedWidth = Mathf.Max(_windowedWidth, 640);
        _windowedHeight = Mathf.Max(_windowedHeight, 360);
        Screen.SetResolution(_windowedWidth, _windowedHeight, FullScreenMode.Windowed);
    }

    private void MoveToNextDisplay()
    {
        RefreshDisplays();

        if (_displays.Count < 2)
        {
            return;
        }

        _currentDisplayIndex = GetDisplayIndex(Screen.mainWindowDisplayInfo);

        if (_currentDisplayIndex < 0)
        {
            _currentDisplayIndex = GetDisplayIndex(_initialDisplay);
        }

        int nextDisplayIndex = (_currentDisplayIndex + 1) % _displays.Count;
        MoveToDisplay(nextDisplayIndex);
    }

    private void MoveToDisplay(int displayIndex)
    {
        if (displayIndex < 0 || displayIndex >= _displays.Count)
        {
            return;
        }

        DisplayInfo targetDisplay = _displays[displayIndex];
        Vector2Int targetPosition = new(targetDisplay.workArea.x, targetDisplay.workArea.y);
        Screen.MoveMainWindowTo(in targetDisplay, targetPosition);
        _currentDisplayIndex = displayIndex;
    }

    private void RefreshDisplays()
    {
        _displays.Clear();
        Screen.GetDisplayLayout(_displays);
    }

    private int GetDisplayIndex(DisplayInfo display)
    {
        for (int i = 0; i < _displays.Count; i++)
        {
            DisplayInfo current = _displays[i];
            if (current.name == display.name
                && current.width == display.width
                && current.height == display.height
                && current.workArea == display.workArea)
            {
                return i;
            }
        }

        return -1;
    }
}
