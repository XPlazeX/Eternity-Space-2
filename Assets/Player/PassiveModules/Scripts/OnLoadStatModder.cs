using System.Collections;
using UnityEngine;
using StatsManipulating;

public class OnLoadStatModder : Module
{
    [SerializeField] private StatOperator[] _operators;
    [SerializeField] private HealthStatOperator[] _healthOperators;

    public override void Load()
    {
        for (int i = 0; i < _operators.Length; i++)
        {
            _operators[i].Enforce();
        }

        for (int i = 0; i < _healthOperators.Length; i++)
        {
            _healthOperators[i].Enforce();
        }
    }
}
