using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MoneyLabel : MonoBehaviour
{
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _minusColor;
    private Text _label;

    void Start()
    {
        _label = GetComponent<Text>();
        
        CheckValue();
        GameSessionInfoHandler.SavingAll += CheckValue;
        //PlayerShipData.ChangeHealth += ChangeValue;
    }

    private void OnDisable() {
        GameSessionInfoHandler.SavingAll -= CheckValue;
    }


    private void CheckValue()
    {
        if (_label != null)
        {
            int money = GameSessionInfoHandler.GetSessionSave().Money;

            _label.color = money >= 0 ? _normalColor : _minusColor; 

            _label.text = $"{money}";
        }
    }
}
