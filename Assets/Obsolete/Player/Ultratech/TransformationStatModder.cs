using System.Collections;
using UnityEngine;
using StatsManipulating;

public class TransformationStatModder : MonoBehaviour
{
    [SerializeField] private StatOperator[] _onEnableOperators;
    [SerializeField] private StatOperator[] _onDisableOperators;

    public void OnEnable()
    {
        for (int i = 0; i < _onEnableOperators.Length; i++)
        {
            _onEnableOperators[i].Enforce();
        }
    }

    public void OnDisable()
    {
        for (int i = 0; i < _onDisableOperators.Length; i++)
        {
            _onDisableOperators[i].Enforce();
        }
    }
}
