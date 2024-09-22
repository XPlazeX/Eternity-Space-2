using UnityEngine;
using UnityEngine.UI;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private Text[] _auriteLabels;
    [SerializeField] private Text[] _cosmiliteLabels;
    [SerializeField] private Text[] _positroniumLabels;
    [SerializeField] private GameObject _gameAuriteCounter;
    [SerializeField] private GameObject _gameCosmiliteCounter;

    private VictoryHandler _victoryHandler;

    public void Initialize() {
        _victoryHandler = SceneStatics.CharacterCore.GetComponent<VictoryHandler>();
        VictoryHandler.AddTempCurrency += OnAddTempCurrency;

        if (GameSessionInfoHandler.FinalLevel)
        {
            _gameAuriteCounter.SetActive(false);
            _gameCosmiliteCounter.SetActive(true);
        }

        OnAddTempCurrency();
    }

    private void OnDisable() {
        VictoryHandler.AddTempCurrency -= OnAddTempCurrency;
    }

    private void OnAddTempCurrency()
    {
        if (_victoryHandler == null)
            return;

        for (int i = 0; i < _auriteLabels.Length; i++)
        {
            _auriteLabels[i].text = $"+{_victoryHandler.TempAurite}";
        }
        for (int i = 0; i < _cosmiliteLabels.Length; i++)
        {
            _cosmiliteLabels[i].text = $"+{_victoryHandler.TempCosmilite}";
        }
        for (int i = 0; i < _positroniumLabels.Length; i++)
        {
            _positroniumLabels[i].text = $"+{_victoryHandler.TempPositronium}";
        }
    }
}
