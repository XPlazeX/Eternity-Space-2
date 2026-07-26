using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class ESTime : MonoBehaviour
{
    private static readonly int UnscaledTimeId = Shader.PropertyToID("_ESUnscaledTime");
    private static readonly int UITimeId = Shader.PropertyToID("_ESUITime");
    private static readonly int WorldTimeId = Shader.PropertyToID("_ESWorldTime");
    private static readonly int ArkTimeId = Shader.PropertyToID("_ESArkTime");

    public static float unscaledTimeScale;
    public static float unscaledTime;
    public static float unscaledDeltaTime;
    public static float unscaledFixedDeltaTime;

    public static float uiTimeScale;
    public static float uiTargetTimeScale;
    public static float uiTime;
    public static float uiDeltaTime;
    public static float uiFixedDeltaTime;

    public static float worldTimeScale;
    public static float worldTargetTimeScale;
    public static float worldTime;
    public static float worldDeltaTime;
    public static float worldFixedDeltaTime;

    public static float arkTimeScale;
    public static float arkTargetTimeScale;
    public static float arkTime;
    public static float arkDeltaTime;
    public static float arkFixedDeltaTime;

    private static ESTime _instance;
    private static readonly List<PauseHandle> _activePauses = new();
    private static bool _isDirty;
    private static bool _cachedUiPause;
    private static bool _cachedWorldPause;
    private static bool _cachedArkPause;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (_instance != null)
            return;

        var gameObject = new GameObject("ES Time")
        {
            hideFlags = HideFlags.HideAndDontSave
        };

        unscaledTimeScale = 1f;
        unscaledTime = 0f;
        unscaledDeltaTime = 0f;
        unscaledFixedDeltaTime = 0f;

        uiTimeScale = 1f;
        uiTargetTimeScale = 1f;
        uiTime = 0f;
        uiDeltaTime = 0f;
        uiFixedDeltaTime = 0f;

        worldTimeScale = 1f;
        worldTargetTimeScale = 1f;
        worldTime = 0f;
        worldDeltaTime = 0f;
        worldFixedDeltaTime = 0f;

        arkTimeScale = 1f;
        arkTargetTimeScale = 1f;
        arkTime = 0f;
        arkDeltaTime = 0f;
        arkFixedDeltaTime = 0f;

        DontDestroyOnLoad(gameObject);
        _instance = gameObject.AddComponent<ESTime>();
    }

    public static PauseHandle AcquirePause(PauseRequest request)
    {
        var handle = new PauseHandle( request);
        _activePauses.Add(handle);
        _isDirty = true;

        RecalculateTime();
        return handle;
    }

    public static bool IsPaused(TimeDomain domain)
    {
        if (_isDirty)
        {
            _cachedUiPause = false;
            _cachedWorldPause = false;
            _cachedArkPause = false;

            foreach (var pause in _activePauses)
            {
                if ((pause.Request.Domains & TimeDomain.UI) != 0)
                    _cachedUiPause = true;
                else if ((pause.Request.Domains & TimeDomain.World) != 0)
                    _cachedWorldPause = true;
                else if ((pause.Request.Domains & TimeDomain.Ark) != 0)
                    _cachedArkPause = true;
            }

            _isDirty = false;
        }
        
        switch (domain)
        {
            case TimeDomain.UI:
                return _cachedUiPause;
            case TimeDomain.World:
                return _cachedWorldPause;
            case TimeDomain.Ark:
                return _cachedArkPause;
            default:
                return false;
        }
    }

    public static void Release(PauseHandle handle)
    {
        _activePauses.Remove(handle);
        _isDirty = true;
        RecalculateTime();
    }

    private static void RecalculateTime()
    {
        uiTimeScale = IsPaused(TimeDomain.UI) ? 0f : uiTargetTimeScale;
        worldTimeScale = IsPaused(TimeDomain.World) ? 0f : worldTargetTimeScale;
        arkTimeScale = IsPaused(TimeDomain.Ark) ? 0f : arkTargetTimeScale;
    }

    void Update()
    {
        unscaledTime = Time.time;
        unscaledDeltaTime = Time.unscaledDeltaTime;
        Shader.SetGlobalFloat(UnscaledTimeId, unscaledTime);

        if (!IsPaused(TimeDomain.UI))
        {
            uiTimeScale = uiTargetTimeScale;
            uiDeltaTime = unscaledDeltaTime * uiTimeScale;
            uiTime += uiDeltaTime;
            Shader.SetGlobalFloat(UITimeId, uiTime);
        }
        else
        {
            uiDeltaTime = 0f;
        }
        
        if (!IsPaused(TimeDomain.World))
        {
            worldTimeScale = worldTargetTimeScale;
            worldDeltaTime = unscaledDeltaTime * worldTimeScale;
            worldTime += worldDeltaTime;
            Shader.SetGlobalFloat(WorldTimeId, worldTime);
        }
        else
        {
            worldDeltaTime = 0f;
        }

        if (!IsPaused(TimeDomain.Ark))
        {
            arkTimeScale = arkTargetTimeScale;
            arkDeltaTime = unscaledDeltaTime * arkTimeScale;
            arkTime += arkDeltaTime;
            Shader.SetGlobalFloat(ArkTimeId, arkTime);
        }
        else
        {
            arkDeltaTime = 0f;
        }
    }

    void FixedUpdate()
    {
        unscaledFixedDeltaTime = Time.fixedUnscaledDeltaTime;

        if (!IsPaused(TimeDomain.UI))
        {
            uiFixedDeltaTime = unscaledFixedDeltaTime * uiTimeScale;
        } else
        {
            uiFixedDeltaTime = 0f;
        }

        if (!IsPaused(TimeDomain.World))
        {
            worldFixedDeltaTime = unscaledFixedDeltaTime * worldTimeScale;
        } else
        {
            worldFixedDeltaTime = 0f;
        }

        if (!IsPaused(TimeDomain.Ark))
        {
            arkFixedDeltaTime = unscaledFixedDeltaTime * arkTimeScale;
        } else
        {
            arkFixedDeltaTime = 0f;
        }
    }
}

public sealed class PauseHandle : IDisposable
{
    public PauseRequest Request { get; }

    public PauseHandle(PauseRequest request)
    {
        Request = request;
    }

    public void Dispose()
    {
        ESTime.Release(this);
    }
}

[System.Flags]
public enum TimeDomain
{
    None = 0,
    UI = 1 << 0,
    World = 1 << 1,
    Ark   = 1 << 2
}

public readonly struct PauseRequest
{
    public readonly TimeDomain Domains;
    public readonly string Reason;

    public PauseRequest(TimeDomain domains, string reason)
    {
        Domains = domains;
        Reason = reason;
    }
}
