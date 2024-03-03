using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class NanomachinesLabel : MonoBehaviour
{
    private Text _label;

    private bool _inGame;

    void Start()
    {
        _label = GetComponent<Text>();

        _inGame = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Game";
        
        if (_inGame)
        {
            CheckGameValue(0);
            PlayerShipData.ChangeHealth += CheckGameValue;
        }

        CheckValue();
        GameSessionInfoHandler.SavingAll += CheckValue;
    }

    private void OnDisable() 
    {
        if (_inGame)  
            PlayerShipData.ChangeHealth -= CheckGameValue;
        
        GameSessionInfoHandler.SavingAll -= CheckValue;
    }

    private void CheckValue()
    {
        if (_label != null)
        {
            _label.text = $"{GameSessionInfoHandler.GetSessionSave().HealthPoints} / {GameSessionInfoHandler.GetSessionSave().MaxHealth}";
        }
    }

    private void CheckGameValue(int value)
    {
        if (_label != null)
        {
            _label.text = $"{PlayerShipData.HitPoints} / {GameSessionInfoHandler.GetSessionSave().MaxHealth}";
        }
    }
}
