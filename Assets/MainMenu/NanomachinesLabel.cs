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
        CheckGameValue(0);
        PlayerShipData.ChangeHealth += CheckGameValue;
        GameSessionInfoHandler.SavingAll += CheckValue;
    }

    private void OnDisable() {
        PlayerShipData.ChangeHealth -= CheckGameValue;
        GameSessionInfoHandler.SavingAll -= CheckValue;
    }

    private void CheckValue()
    {
        if (_label != null && !Player.Alive)
        {
            _label.text = $"{GameSessionInfoHandler.GetSessionSave().HealthPoints} / {GameSessionInfoHandler.GetSessionSave().MaxHealth}";
        }
    }

    private void CheckGameValue(int value)
    {
        if (_label != null && Player.Alive)
        {
            _label.text = $"{PlayerShipData.HitPoints} / {GameSessionInfoHandler.GetSessionSave().MaxHealth}";
        }
    }
}
