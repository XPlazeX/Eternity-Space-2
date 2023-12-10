using UnityEngine;

public class ArmorUpgrade : Module
{
    [SerializeField] private int _armorCapBuff;
    [SerializeField] private int _addingArmorPoints;

    public override void Load()
    {
        PlayerShipData.UpgradeArmor(_armorCapBuff, _addingArmorPoints);
    }
}
