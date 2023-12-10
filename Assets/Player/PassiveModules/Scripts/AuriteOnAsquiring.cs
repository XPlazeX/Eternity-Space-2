using UnityEngine;

public class AuriteOnAsquiring : Module
{
    [SerializeField] private Vector2Int _gainRange;
    [SerializeField] private bool _useArray;
    [SerializeField] private int[] _gainVariants;

    public override void Asquiring()
    {
        int addValue = Random.Range(_gainRange.x, _gainRange.y + 1);

        if (_useArray)
            addValue = _gainRange[Random.Range(0, _gainVariants.Length)];

        if (addValue > 0)
            Bank.PutCash(BankSystem.Currency.Aurite, addValue);

        else if (addValue < 0 && Bank.EnoughtCash(BankSystem.Currency.Aurite, addValue))
        {
            Bank.ConsumeCash(BankSystem.Currency.Aurite, addValue);
        } else if (addValue < 0 && !Bank.EnoughtCash(BankSystem.Currency.Aurite, addValue))
        {
            Bank.CancelAurite();
        }
    }
}
