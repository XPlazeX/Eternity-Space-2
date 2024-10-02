using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class NanomachinesLabel : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Image _fill;
    [SerializeField] private GameObject _disconnectIndicator;

    private Text _label;

    private bool _inGame;
    private bool _connected = true;

    void Start()
    {
        _label = GetComponent<Text>();

        _inGame = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Game";
        
        if (_inGame)
        {
            CheckGameValue(0);
            PlayerShipData.ChangeHealth += CheckGameValue;
            return;
        }

        CheckValue();
        GameSessionInfoHandler.SavingAll += CheckValue;
    }

    private void OnDisable() 
    {
        if (_inGame) 
        {
            PlayerShipData.ChangeHealth -= CheckGameValue;
            return;
        }
        
        GameSessionInfoHandler.SavingAll -= CheckValue;
    }

    public void CheckValue()
    {
        if (!_connected)
            return;
        if (_label != null)
        {
            _label.text = $"{GameSessionInfoHandler.GetSessionSave().HealthPoints} / {GameSessionInfoHandler.GetSessionSave().MaxHealth}";
        }
    }

    public void CheckGameValue(int value)
    {
        if (!_connected)
            return;
        if (_label != null)
        {
            _label.text = $"{PlayerShipData.HitPoints} / {GameSessionInfoHandler.GetSessionSave().MaxHealth}";
        }
    }

    public void SetSafetyValue(int value, int maxValue)
    {
        if (!_connected)
            return;
        if (_label != null)
        {
            _label.text = $"{value} / {maxValue}";
        }
    }

    public void Disconnect()
    {
        _connected = false;
        _label.text = SceneLocalizator.GetLocalizedString("MissionMenu", 7, 0);
        if (_icon != null)
            _icon.gameObject.SetActive(false);

        if (_disconnectIndicator != null)
            _disconnectIndicator.SetActive(true);
    }

    public void SetIcon(Sprite icon)
    {
        if (_icon == null)
            return;

        _icon.sprite = icon;
    }

    public void SetFillColor(Color color)
    {
        if (_fill == null)
            return;

        _fill.color = color;
    }

    public void SetTextColor(Color color)
    {
        _label.color = color;
    }
}
