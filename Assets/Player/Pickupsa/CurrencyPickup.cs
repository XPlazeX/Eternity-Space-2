using UnityEngine;

public class CurrencyPickup : Pickup
{
    [SerializeField] private BankSystem.Currency _currency;
    [SerializeField] private int _addingValue;

    protected override void Picked()
    {
        switch (_currency)
        {
            case BankSystem.Currency.Aurite:
                SceneStatics.CharacterCore.GetComponent<VictoryHandler>().AddAurite(_addingValue);
                break;
            case BankSystem.Currency.Cosmilite:
                SceneStatics.CharacterCore.GetComponent<VictoryHandler>().AddCosmilite(_addingValue);
                break;
            case BankSystem.Currency.Positronium:
                SceneStatics.CharacterCore.GetComponent<VictoryHandler>().AddPositronium(_addingValue);
                break;
            default:
                break;
        }
        base.Picked();
    }
}
