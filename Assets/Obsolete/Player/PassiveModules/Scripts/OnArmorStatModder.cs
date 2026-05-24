using UnityEngine;
using StatsManipulating;

public class OnArmorStatModder : Module
{
    [SerializeField] private StatOperator[] _operators;

    private bool _active = false;
    private bool _loaded = false;

    public override void Load()
    {
        // PlayerShipData.ChangeArmor += OnArmorChanged;
        _loaded = true;

        // OnArmorChanged(PlayerShipData.ArmorPoints);
    }

    private void OnDisable() {
        // if (_loaded)
        //  PlayerShipData.ChangeArmor -= OnArmorChanged;
    }

    public void OnArmorChanged(int arm)
    {
        if (arm > 0 && !_active)
        {
            for (int i = 0; i < _operators.Length; i++)
            {
                _operators[i].Enforce();
            }
        }
        else if (arm <= 0 && _active)
        {
           for (int i = 0; i < _operators.Length; i++)
            {
                _operators[i].Negative();
            } 
        }

        _active = arm > 0;
    }
}
