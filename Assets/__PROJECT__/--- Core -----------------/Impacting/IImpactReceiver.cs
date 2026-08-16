using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IImpactReceiver
{
    bool Toleranted {get;}

    ImpactResult ReceiveImpact(in ImpactContext context);
    void SetToleranted(bool toleranted);
}

public interface IRuntimeImapctResistorsHandler
{
    RuntimeImpactResistor[] Resistors {get;}
}

[System.Serializable]
public class ImpactResistor
{
    public ImpactTag Tag;
    public bool IsDisposable = true;
    public float ActiveTime = 0f;
    public bool CallChangesWhileActive = false;
    public bool IsContinious = false;
    public float Resistance = 1f;
    public float RegenerateSpeed = 1f;

    public UnityEvent OnResistancePunctured;
    public UnityEvent OnResistanceRecovered;
    public UnityEvent OnResistanceChanged;
    public UnityEvent OnActiveTimeEnded;
    public UnityEvent OnActiveTimeChanged;

    public RuntimeImpactResistor CreateRuntimeResistor()
    {
        return new RuntimeImpactResistor(this);
    }
}

public class RuntimeImpactResistor
{
    public ImpactTag Tag {get; private set;}
    public bool IsDisposable {get; private set;}
    public float ActiveTime {get; private set;}
    public bool CallChangesWhileActive {get; private set;}
    public bool IsContinious {get; private set;}
    public float Resistance {get; private set;}
    public float RegenerateSpeed {get; private set;}
    public UnityEvent OnResistancePunctured {get; private set;}
    public UnityEvent OnResistanceRecovered {get; private set;}
    public UnityEvent OnResistanceChanged {get; private set;}
    public UnityEvent OnActiveTimeEnded {get; private set;}
    public UnityEvent OnActiveTimeChanged {get; private set;}

    private float _currentResistance;
    private bool _disposed = false;
    private float _activeTimer = 0f;
    private bool _recovered = false;

    public float CurrentResistance => _currentResistance;
    public float CurrentResistanceNormalized => _currentResistance / Resistance;
    public float ActiveTimer => _activeTimer;
    public float ActiveTimerNormalized => _activeTimer / ActiveTime;
    public bool Disposed => _disposed;

    public RuntimeImpactResistor(ImpactResistor resistor)
    {
        Tag = resistor.Tag;
        IsDisposable = resistor.IsDisposable;
        ActiveTime = resistor.ActiveTime;
        CallChangesWhileActive = resistor.CallChangesWhileActive;
        IsContinious = resistor.IsContinious;
        Resistance = resistor.Resistance;
        RegenerateSpeed = resistor.RegenerateSpeed;
        OnResistancePunctured = resistor.OnResistancePunctured;
        OnResistanceRecovered = resistor.OnResistanceRecovered;
        OnResistanceChanged = resistor.OnResistanceChanged;
        OnActiveTimeEnded = resistor.OnActiveTimeEnded;
        OnActiveTimeChanged = resistor.OnActiveTimeChanged;
        _disposed = false;
        _recovered = false;
        _currentResistance = 0f;
    }

    public void Tick(float deltaTime)
    {
        if (!IsContinious)
            return;

        if (_activeTimer > 0f)
        {
            _activeTimer -= deltaTime;
            OnActiveTimeChanged?.Invoke();
            if (_activeTimer <= 0f)
            {
                _activeTimer = 0f;
                OnActiveTimeEnded?.Invoke();
            }
        }

        if (_disposed)
            return;

        if (_currentResistance > 0f)
        {
            _currentResistance -= RegenerateSpeed * deltaTime;
            _currentResistance = Mathf.Clamp(_currentResistance, 0f, Resistance);

            if (CallChangesWhileActive)
                OnResistanceChanged?.Invoke();
            else if (!CallChangesWhileActive && _activeTimer == 0f)
                OnResistanceChanged?.Invoke();

            if (_currentResistance <= 0f && !_recovered)
            {
                _recovered = true;
                if (CallChangesWhileActive)
                    OnResistanceRecovered?.Invoke();
                else if (!CallChangesWhileActive && _activeTimer == 0f)
                    OnResistanceRecovered?.Invoke();  
            }
        }
    }

    public bool Compare(Impact impact)
    {
        if (impact.Tag != Tag || _disposed || _activeTimer > 0f)
            return false;

        _currentResistance += impact.Amount;
        _currentResistance = Mathf.Clamp(_currentResistance, 0f, Resistance);

        OnResistanceChanged?.Invoke();
        
        if (_currentResistance >= Resistance && !_disposed)
        {   
            Puncture();
            _recovered = false;
            return true;
        }

        if (!IsContinious)
        {
            _currentResistance = 0f;
            OnResistanceRecovered?.Invoke();
        } else
        {
            _recovered = false;
        }

        return true;
    }

    private void Puncture()
    {
        if (IsDisposable)
            _disposed = true;

        if (ActiveTime > 0f)
            _activeTimer = ActiveTime;

        OnResistancePunctured?.Invoke();
    }
}

public struct ImpactResult
{
    public bool IsReacted => Reactions != null && Reactions.Count > 0;
    public List<ImpactReaction> Reactions;

    public struct ImpactReaction
    {
        public ImpactTag Tag;
        public float Amount;
    }
}

public struct ImpactContext
{
    public GameObject Source;
    public GameObject Instigator;

    public Vector2 Point;
    public Vector2 Direction;
    public float Distance => Vector2.Distance(Point, Source.transform.position);
    public float InstigatorDistance => Vector2.Distance(Point, Instigator.transform.position);

    public Impact[] Impacts;
}

[System.Serializable]
public struct Impact
{
    public ImpactTag Tag;
    public float Amount;
}

public enum ImpactTag
{
    Use,
    Kinetic,
    Penetrating,
    Electrical,
    Disrupting,
    Cutting,
    Repairing,
    SurfaceScan,
    InternalScan,
    Attracting,
    Manipulating,
    Barrier,
    Synchronizing,
    ReactorPulse,
    Reconstructing
}