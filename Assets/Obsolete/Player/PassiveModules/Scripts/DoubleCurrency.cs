using UnityEngine;

public class DoubleCurrency : Module
{
    [SerializeField] private BankSystem.Currency _currency;
    [SerializeField] private float _multiplier;

    public override void Asquiring()
    {
        Bank.PutCash(_currency, Mathf.CeilToInt((float)Bank.GetCurrencyAmount(_currency) * _multiplier));
    }
}
