using UnityEngine;

public class Tocamak : Core
{
    [SerializeField] private bool _loseOnDamage;
    [SerializeField] private bool _multiplyByContagion;
    [SerializeField] private float _perContagionMultiplier;

    private void OnEnable() {
        // if (_loseOnDamage)
        //     PlayerShipData.TakeAnyDamage += OnDamageTaken;
    }

    protected override void OnDisable() {
        base.OnDisable();
        // if (_loseOnDamage)
        //     PlayerShipData.TakeAnyDamage -= OnDamageTaken;
    }

    private void Update()
    {
        float growth = _megawattsGrowth;

        if (_multiplyByContagion)
        {
            growth *= _perContagionMultiplier * ContagionHandler.ContagionLevel;
        }

        PlayerCore.AddEnergy(growth * ESTime.worldDeltaTime * Effeciency);
    }

    private void OnDamageTaken(int points)
    {
        PlayerCore.NullifyEnergy();
    }
}
