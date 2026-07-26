using UnityEngine;

public class Cheat : MonoBehaviour
{
    public void AddAurite(int val)
    {
        Bank.PutCash(BankSystem.Currency.Aurite, val);
    }

    public void ConsumeAurite(int val)
    {
        Bank.ConsumeCash(BankSystem.Currency.Aurite, val);
    }
}
