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
    private static TimeHandler instance;

    private void Awake() {
        //_normalFixedDeltaTime = Time.fixedDeltaTime;
        instance = this;
        Workable = true;
        normalFixedDeltaTime = PlayerPrefs.GetFloat("FixedUpdateStep", 1f / 60f);
    }

    public static void Initialize() 
    {
        ShipStats.StatChanged += ObserveStat;
        _defaultTimeSlowing = ShipStats.GetValue("TimeSlowValue");

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

        if (Time.timeScale == 0)
            return;
        
        if (value == 0)
            throw new System.Exception("Устанавливайте время равное 0 через метод Pause()");

        Time.timeScale = value;
        Time.fixedDeltaTime = normalFixedDeltaTime * value;
        //print("time custom set");
    }

    public static void Pause()
    {
        _timeScaleBeforePause = Time.timeScale;
        if (_timeScaleBeforePause == 0)
            _timeScaleBeforePause = 1f;
            
        Time.timeScale = 0;
        TimePaused?.Invoke();
    }

    public static void Resume(float forcedMultiplier = -1f)
    {
        if (forcedMultiplier > 0)
        {
            Time.timeScale = forcedMultiplier;
            Time.fixedDeltaTime = normalFixedDeltaTime * forcedMultiplier;
            return;
        }

        Time.timeScale = _timeScaleBeforePause;
        Time.fixedDeltaTime = normalFixedDeltaTime * _timeScaleBeforePause;
        TimeNormalized?.Invoke();
    }
}
