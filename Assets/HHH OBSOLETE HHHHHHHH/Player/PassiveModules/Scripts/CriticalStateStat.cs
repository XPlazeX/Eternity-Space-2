using UnityEngine;
using StatsManipulating;

public class CriticalStateStat : Module
{
    [SerializeField] private StatOperator[] _statsOperators;

    private bool _enforced;

    public override void Load()
    {
        // PlayerShipData.ChangeHealth += CheckConditions;
    }

    private void OnDisable() {
        // PlayerShipData.ChangeHealth -= CheckConditions;
    }

    public void CheckConditions(int v)
    {
        // if (PlayerShipData.CriticalState && !_enforced)
        // {
        //     for (int i = 0; i < _statsOperators.Length; i++)
        //     {
        //         _statsOperators[i].Enforce();
        //     }
        //     _enforced = true;
        // } else if (!PlayerShipData.CriticalState && _enforced)
        // {
        //     for (int i = 0; i < _statsOperators.Length; i++)
        //     {
        //         _statsOperators[i].Negative();
        //     }
        //     _enforced = false;
        // }
    }
}
