using UnityEngine;

[System.Serializable]
public class ShipStatModifier : TypeModifier
{
    [SerializeField] private string _statName;
    [SerializeField] private float _value;

    public void Enforce()
    {
        ShipStats.MultiplyStat(_statName, _value);
    }

    public void Negative()
    {
        ShipStats.MultiplyStat(_statName, 1f / _value);
    }
}
