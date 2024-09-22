using UnityEngine;

public class Rage : Core
{
    private void OnEnable() {
        PlayerShipData.TakeHealthDamage += OnHealthDamaged;
    }

    private void OnDisable() {
        PlayerShipData.TakeHealthDamage -= OnHealthDamaged;
    }

    private void OnHealthDamaged(int amount)
    {
        PlayerCore.AddEnergy(_megawattsGrowth * amount);
    }
}
