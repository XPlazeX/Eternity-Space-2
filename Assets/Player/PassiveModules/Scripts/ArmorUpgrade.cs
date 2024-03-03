using UnityEngine;

public class ArmorUpgrade : Module
{
    [SerializeField] private int _armorCapBuff;
    [SerializeField] private int _addingArmorPoints;
    [SerializeField] private bool _breakArmoring;

    public override void Load()
    {
        if (_breakArmoring)
        {
            PlayerShipData.BreakArmoring();
            return;
        }
        PlayerShipData.UpgradeArmor(_armorCapBuff, _addingArmorPoints);
    }
}
