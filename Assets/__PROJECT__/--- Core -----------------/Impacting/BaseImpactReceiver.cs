using UnityEngine;

public class BaseImpactReceiver : MonoBehaviour, IImpactReceiver
{
    [SerializeField] private ImpactResistor[] resistors;

    private RuntimeImpactResistor[] _runtimeResistors;

    public void Awake()
    {
        _runtimeResistors = new RuntimeImpactResistor[resistors.Length];
        for (int i = 0; i < resistors.Length; i++)
        {
            _runtimeResistors[i] = resistors[i].CreateRuntimeResistor();
        }
    }

    public float GetResistance01(int index)
    {
        if (index < 0 || index >= _runtimeResistors.Length)
            return 0f;

        return _runtimeResistors[index].CurrentResistance / _runtimeResistors[index].Resistance;
    }

    public ImpactResult ReceiveImpact(in ImpactContext context)
    {
        ImpactResult result = new ImpactResult();
        result.Reactions = new System.Collections.Generic.List<ImpactResult.ImpactReaction>();

        foreach (var resistor in _runtimeResistors)
        {

            for (int i = 0; i < context.Impacts.Length; i++)
            {
                if (!resistor.Compare(context.Impacts[i]))
                    continue;

                result.Reactions.Add(new ImpactResult.ImpactReaction
                {
                    Tag = context.Impacts[i].Tag,
                    Amount = context.Impacts[i].Amount
                });
            }
        }

        return result;
    }

    public virtual void Update()
    {
        float deltaTime = Time.deltaTime;
        foreach (var resistor in _runtimeResistors)
        {
            resistor.Tick(deltaTime);
        }
    }
}
