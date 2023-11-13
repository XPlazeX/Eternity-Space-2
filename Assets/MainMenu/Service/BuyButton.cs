using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using BankSystem;

public class BuyButton : MonoBehaviour
{
    [SerializeField] private Currency _currency;
    [SerializeField] private int _price;
    [SerializeField] private UnityEvent _successBuyEvent;
    [SerializeField] private UnityEvent _failedBuyEvent;
    [SerializeField] private Text _priceLabel;

    // private void Start() {
    //     _priceLabel.text = _price.ToString();
    // }
    
    public void TryBuy()
    {
        if (Bank.EnoughtCash(_currency, _price))
        {
            Bank.ConsumeCash(_currency, _price);
            _successBuyEvent?.Invoke();
        } else {
            _failedBuyEvent?.Invoke();
        }
    }

    public void ScalePrice(int newPrice)
    {
        _price = newPrice;
        _priceLabel.text = _price.ToString();
        //print($"price scale {newPrice}");
    }

    public void SetPriceText(string text)
    {
        _priceLabel.text = text;
        //print($"set text {text}");
    }

    public void DefFailure(Animator animator)
    {
        DefaultFailure.Failure(animator);
    }

    public void DefAsquire()
    {
        DefaultFailure.Asquire();
    }
}
