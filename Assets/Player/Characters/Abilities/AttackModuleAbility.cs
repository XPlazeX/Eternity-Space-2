using UnityEngine;

public class AttackModuleAbility : Ability
{
    [SerializeField] private AttackModule _attackModule;
    [SerializeField] private float _waitTime;
    [SerializeField] private bool _consumeHP;
    [SerializeField] private int _hpCost;

    public override void Use()
    {
        if (_consumeHP)
        {
            PlayerShipData.ConsumeHP(_hpCost);
        }
        _attackModule.HandFireSeries(0, _waitTime);
    }
}
