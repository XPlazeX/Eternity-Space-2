using UnityEngine;

public class ShieldGetter : Device
{
    [SerializeField] private int _gettingValue;

    private int _moddedGettingValue;

    public override void Load()
    {
        base.Load();
        _moddedGettingValue = _gettingValue * ShipStats.GetIntValue("ShieldCapacityMultiplier");
    }

    public override void Fire()
    {
        GetShield();
    }

    protected virtual void GetShield()
    {
        if (PlayerShipData.ShieldPoints < _moddedGettingValue)
            PlayerShipData.GetShield(_moddedGettingValue);
    }

}
