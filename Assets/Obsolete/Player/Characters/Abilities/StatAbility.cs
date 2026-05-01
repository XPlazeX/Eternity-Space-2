using StatsManipulating;
using UnityEngine;

public class StatAbility : Ability
{
    [SerializeField] private StatOperator[] _modifiers;
    [SerializeField] private float _effectDuration;
    [SerializeField] private Color _effectColor;

    public override void Use()
    {
        for (int i = 0; i < _modifiers.Length; i++)
        {
            _modifiers[i].Enforce();
        }
        
        GameObject.FindWithTag("BetweenScenes").GetComponent<TimedDelegator>().FuseAction(NegativeEffect, _effectDuration);
        SceneStatics.UICore.GetComponent<PlayerUI>().PlayAbility(_effectColor, _effectDuration);
    }

    private void NegativeEffect()
    {
        for (int i = 0; i < _modifiers.Length; i++)
        {
            _modifiers[i].Negative();
        }
    }
}
