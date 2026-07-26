using UnityEngine;
using StatsManipulating;

public class StatOnEnemyKill : Module
{
    [SerializeField] private StatOperator[] _modifiers;
    [SerializeField] private float _effectDuration;

    public override void Load()
    {
        Spawner.DamageBodyDeathed += OnEnemyKill;
    }

    private void OnDisable() {
        Spawner.DamageBodyDeathed -= OnEnemyKill;
    }

    protected void OnEnemyKill()
    {
        for (int i = 0; i < _modifiers.Length; i++)
        {
            _modifiers[i].Enforce();
        }
        
        GameObject.FindWithTag("BetweenScenes").GetComponent<TimedDelegator>().FuseAction(NegativeEffect, _effectDuration);
    }

    private void NegativeEffect()
    {
        for (int i = 0; i < _modifiers.Length; i++)
        {
            _modifiers[i].Negative();
        }
    }
}
