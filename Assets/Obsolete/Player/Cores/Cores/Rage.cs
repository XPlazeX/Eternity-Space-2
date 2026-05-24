using UnityEngine;

public class Rage : Core
{
    private void OnEnable() {
        // PlayerShipData.TakeHealthDamage += OnHealthDamaged;
    }

    protected override void OnDisable() {
        base.OnDisable();
        // PlayerShipData.TakeHealthDamage -= OnHealthDamaged;
    }

    private void OnHealthDamaged(int amount)
    {
        PlayerCore.AddEnergy(_megawattsGrowth * amount * Effeciency);
    }
}
