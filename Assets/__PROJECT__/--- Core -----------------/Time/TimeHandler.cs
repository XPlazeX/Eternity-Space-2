using UnityEngine;
using System.Collections;

public class TimeHandler : MonoBehaviour
{
    private static float normalFixedDeltaTime = 0.015f;

    public delegate void timeControl();
    public static event timeControl TimeSlow;
    public static event timeControl TimeResume;
    public static event timeControl TimePaused;
    public static event timeControl TimeNormalized;

    private static float _defaultTimeSlowing = 0.15f;
    private static float _criticalTimeSlowing = 0.05f;
    private static float _timeScaleBeforePause = 1f;

    public static float DefaultTimeSlowing 
    {
        get { return _defaultTimeSlowing; }
        set 
        {
            _defaultTimeSlowing = Mathf.Clamp01(value);
        }
    }
    public static bool CriticalState {get; set;} = false;
    public static bool Workable {get; set;} = true;
    public static bool AffectCriticalState {get; private set;} = false;
    public static float LevelTime {get; private set;} = 0f;
    private static TimeHandler instance;

    private void Awake() {
        //_normalFixedDeltaTime = ESTime.worldFixedDeltaTime;
        instance = this;
        // Workable = true;
        normalFixedDeltaTime = Time.fixedUnscaledDeltaTime;
        // normalFixedDeltaTime = PlayerPrefs.GetFloat("FixedUpdateStep", 1f / 60f);
    }

    private void Update() 
    {
        if (ESTime.worldTimeScale > 0)
            LevelTime += ESTime.unscaledDeltaTime;
    }

    public static void Initialize() 
    {
        ShipStats.StatChanged += ObserveStat;
        _defaultTimeSlowing = ShipStats.GetValue("TimeSlowValue");
        LevelTime = 0f;

        _criticalTimeSlowing = _defaultTimeSlowing * ShipStats.GetValue("CriticalTimeSlowMultiplier");
        print($"TimeHandler started, time slowing : {_defaultTimeSlowing}, criticalTImeSlowing : {_criticalTimeSlowing}");
    }

    private static void ObserveStat(string name, float val)
    {
        if (name == "TimeSlowValue")
            _defaultTimeSlowing = ShipStats.GetValue("TimeSlowValue");
    }

    public static void SlowDown()
    {
        if (!Workable)
            return;

        if (AffectCriticalState && CriticalState)
            SetTimeScale(_criticalTimeSlowing);
        
        else 
            SetTimeScale(DefaultTimeSlowing);

        TimeSlow?.Invoke();
        //print("time slow");
    }

    public static void Recover()
    {
        SetTimeScale(1f);
        TimeResume?.Invoke();
    }

    private static void SetTimeScale(float value)
    {
        if (!Workable)
            return;

        if (ESTime.worldTimeScale == 0)
            return;
        
        if (value == 0)
            throw new System.Exception("Устанавливайте время равное 0 через метод Pause()");

        ESTime.worldTimeScale = value;
        ESTime.worldFixedDeltaTime = normalFixedDeltaTime * value;
        //print("time custom set");
    }

    public static void Pause()
    {
        Debug.Log("PAUSE");
        _timeScaleBeforePause = ESTime.worldTimeScale;
        if (_timeScaleBeforePause == 0)
            _timeScaleBeforePause = 1f;
            
        ESTime.worldTimeScale = 0;
        TimePaused?.Invoke();
    }

    public static void Resume(float forcedMultiplier = -1f)
    {
        if (Dialogue.ActiveDialog)
        {
            Debug.Log("TRY TO RESUME TIME WHILE DIALOG IS ACTIVE");
            return;
        }
        Debug.Log("RESUME");
        if (forcedMultiplier > 0)
        {
            ESTime.worldTimeScale = forcedMultiplier;
            ESTime.worldFixedDeltaTime = normalFixedDeltaTime * forcedMultiplier;
            return;
        }

        ESTime.worldTimeScale = _timeScaleBeforePause;
        ESTime.worldFixedDeltaTime = normalFixedDeltaTime * _timeScaleBeforePause;
        TimeNormalized?.Invoke();
    }
}
