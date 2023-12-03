using UnityEngine;
using StatsManipulating;

public class StatPickup : Pickup
{
    [SerializeField] private StatOperator[] _modifiers;

    protected override void Picked()
    {
        for (int i = 0; i < _modifiers.Length; i++)
        {
            _modifiers[i].Enforce();
        }
        
        GameObject.FindWithTag("BetweenScenes").GetComponent<TimedDelegator>().FuseAction(NegativeEffect, _effectDuration);

        base.Picked();
    }

    private void NegativeEffect()
    {
        for (int i = 0; i < _modifiers.Length; i++)
        {
            _modifiers[i].Negative();
        }
    }
}
