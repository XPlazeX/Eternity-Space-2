using UnityEngine;

public class Impact : Core
{
    private void OnEnable() {
        PlayerRamsHandler.RamSuccess += OnRamSuccess;
    }

    private void OnDisable() {
        PlayerRamsHandler.RamSuccess -= OnRamSuccess;
    }

    private void OnRamSuccess()
    {
        PlayerCore.AddEnergy(_megawattsGrowth);
    }
}
