using UnityEngine;
using System.Collections;
using DamageSystem;

public class EmissionHandler : MonoBehaviour
{
    [SerializeField] private EmissionUI _emissionUI;
    [Header("Изменения не будут приняты. Контролируйте через ShipStats")]
    [SerializeField] private float _chargeTime = 30f;
    [SerializeField] private float _emissionDuration;
    [SerializeField] private EmissionVisualizer _emissionVisualizerSample;

    public float FirerateBoost {get; set;} = 2f;
    public static bool EmissionReady {get; private set;} = false;
    public bool Workable {get; private set;} = false;

    private float _currentValue;
    private IEnumerator _chargingIE;
    private EmissionVisualizer _emissionVisualizer;

    private void Start() 
    {
        if (!Workable)
        {
            _emissionUI.Hide();
            return;
        }
        PlayerShipData.PlayerDeath += StopWork;
        Player.PlayerChanged += BindVisualizer;
        _emissionVisualizer = Instantiate(_emissionVisualizerSample);
        BindVisualizer();

        PlayerDamageBody playerDamageBody = GameObject.FindWithTag("Player").GetComponent<PlayerDamageBody>();
        PlayerShipData.TakeHealthDamage += ResetEmission;
        PlayerShipData.TakeArmorDamage += ResetEmission;

        _chargeTime = ShipStats.GetValue("EmissionFillTime");
        _emissionDuration = ShipStats.GetValue("EmissionDuration");

        _chargingIE = Charging();
        StartCoroutine(_chargingIE);
    }

    private void OnDisable() {
        if (!Workable)
            return;
        PlayerShipData.TakeHealthDamage -= ResetEmission;
        PlayerShipData.TakeArmorDamage -= ResetEmission;
        PlayerShipData.PlayerDeath -= StopWork;
        Player.PlayerChanged -= BindVisualizer;
    }

    private void ResetEmission(int dmg)
    {
        _currentValue -= _currentValue * ShipStats.GetValue("EmissionResetPart");
        _emissionUI.FillEmission(_currentValue);

        if (_currentValue <= 0f)
            {
                EmissionReady = false;
            }
    }

    private void BindVisualizer()
    {
        _emissionVisualizer.Bind(Player.PlayerTransform);
    }

    private IEnumerator Charging()
    {
        while (true)
        {
            _currentValue = Mathf.Clamp((_currentValue + 0.5f), 0, _chargeTime);
            _emissionUI.FillEmission(_currentValue / _chargeTime);

            if (_currentValue >= _chargeTime)
                EmissionReady = true;

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void Emit()
    {
        if (!Workable)
            return;
        _emissionUI.Emit();
        EmissionReady = false;
        StopCoroutine(_chargingIE);
        StartCoroutine(Emitting());
    }

    private IEnumerator Emitting()
    {
        float timer = _emissionDuration;

        ShipStats.IncreaseStat("MainWeaponFirerateMultiplier", ShipStats.GetValue("EmissionFirerateBoost"));
        _emissionVisualizer.TogglePS(true);

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            _emissionUI.FillEmission(timer / _emissionDuration);
            _emissionVisualizer.ColorProgress(timer / _emissionDuration);

            yield return null;
        }

        ShipStats.IncreaseStat("MainWeaponFirerateMultiplier", -ShipStats.GetValue("EmissionFirerateBoost"));
        _emissionVisualizer.TogglePS(false);
        _currentValue = 0f;
        StartCoroutine(_chargingIE);
    }

    private void StopWork()
    {
        if (!Workable)
            return;
        ResetEmission(0);

        StopAllCoroutines();
    }
}