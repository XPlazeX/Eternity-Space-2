using UnityEngine;
using UnityEngine.UI;

public class EnemyCountUI : MonoBehaviour
{
    [SerializeField] private Text _textLabel;
    [SerializeField] private Text _bonusTextLabel;
    [SerializeField] private Text _reinforcementsTextLabel;
    [SerializeField] private GameObject _headLabel;

    public void Clear()
    {
        _textLabel.color = Color.clear;
        _bonusTextLabel.color = Color.clear;
        _reinforcementsTextLabel.color = Color.clear;
        _headLabel.GetComponent<Text>().color = Color.clear;
    }

    public void SetCount(int val) => _textLabel.text = val.ToString();
    public void SetBonusCount(int val) => _bonusTextLabel.text = "+" + val.ToString();
    public void HideHead() => _headLabel.SetActive(false);
    public void SetHead(string text) => _headLabel.GetComponent<Text>().text = text;
    public void ShowReinforcements() => _reinforcementsTextLabel.gameObject.SetActive(true);
    public void SetReinforcementsDelay(int secs) => _reinforcementsTextLabel.text = SceneLocalizator.GetLocalizedString("Game", 0, 2) + secs.ToString() + "s";
}
