using System.Collections;
using UnityEngine;

public class StatSpinup : Module
{
    [SerializeField] private string _statName;
    [SerializeField] private float _topValue;
    [SerializeField] private float _growthSpeed = 1f;

    private bool _loaded;
    private bool _spinning;
    private float _currentValue;
    private bool _negativeIsBetter;

    public override void Load()
    {
        if (_topValue < 0)
            _negativeIsBetter = true;

        TimeHandler.TimeSlow += EndSpin;
        TimeHandler.TimeResume += StartSpin;

        _loaded = true;
    }

    private void OnDisable() 
    {
        if (!_loaded)
            return;

        TimeHandler.TimeSlow -= EndSpin;
        TimeHandler.TimeResume -= StartSpin;
    }

    private IEnumerator Spinup()
    {
        while (_spinning)
        {
            if (_negativeIsBetter ? (_currentValue > _topValue) : (_currentValue < _topValue))
            {
                _currentValue += _growthSpeed * ESTime.worldDeltaTime;
                ShipStats.IncreaseStat(_statName, _growthSpeed * ESTime.worldDeltaTime);
                //_weaponRoot.FireReload = FireReload;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void StartSpin()
    {
        _spinning = true;
        StartCoroutine(Spinup());
    }

    private void EndSpin()
    {
        _spinning = false;
        ShipStats.IncreaseStat(_statName, -_currentValue);
        _currentValue = 0;
        //_weaponRoot.FireReload = FireReload;
    }
}
