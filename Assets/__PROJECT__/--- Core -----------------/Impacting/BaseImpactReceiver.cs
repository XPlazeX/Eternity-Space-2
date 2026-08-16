using UnityEngine;

public class BaseImpactReceiver : MonoBehaviour, IImpactReceiver, IRuntimeImapctResistorsHandler
{
    [SerializeField] private ImpactResistor[] resistors;

    private RuntimeImpactResistor[] _runtimeResistors;
    private bool _toleranted = false;

    public RuntimeImpactResistor[] Resistors {get {return _runtimeResistors;}}
    public bool Toleranted {get {return _toleranted;}}

    public void Awake()
    {
        _runtimeResistors = new RuntimeImpactResistor[resistors.Length];
        for (int i = 0; i < resistors.Length; i++)
        {
            _runtimeResistors[i] = resistors[i].CreateRuntimeResistor();
        }
    }

    public void SetToleranted(bool toleranted)
    {
        _toleranted = toleranted;
    }

    public float GetResistance01(int index)
    {
        if (index < 0 || index >= _runtimeResistors.Length)
            return 0f;

        return _runtimeResistors[index].CurrentResistance / _runtimeResistors[index].Resistance;
    }

    public ImpactResult ReceiveImpact(in ImpactContext context)
    {
        if (_toleranted) return new ImpactResult();

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
