using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class NanomachinesLabel : MonoBehaviour
{
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
            _label.text = $"{GameSessionInfoHandler.GetSessionSave().HealthPoints} / {GameSessionInfoHandler.GetSessionSave().MaxHealth}";
    }
}
