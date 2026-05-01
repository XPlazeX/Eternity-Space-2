using UnityEngine;

public class RamDriller : Module
{
    [SerializeField] private bool _drillMode;
    [SerializeField] private bool _constantEnemyCount;
    [SerializeField] private int _additiveConstantEnemyCount;

    public override void Load()
    {
        RamShield ramShield = Player.PlayerObject.GetComponentInChildren<RamShield>();
        if (ramShield == null)
            return;//throw new System.Exception("Не найден RamShield для PlayerRamsHandler");

        if (_drillMode)
            ramShield.DrillMode = true;

        if (_constantEnemyCount)
        {
            ramShield.UseConstantEnemyCount = true;
            ramShield.ConstantEnemyCount += _additiveConstantEnemyCount;
        }
    }
}
