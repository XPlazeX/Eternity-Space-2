using UnityEngine;
using StatsManipulating;

public class ConstStatIncreaser : MonoBehaviour
{
    [SerializeField] private StatOperator[] _operators;
    [SerializeField] private HealthStatOperator[] _healthOperators;

    public void Start()
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
