using UnityEngine;
using UnityEngine.UI;

public class EnemyCountUI : MonoBehaviour
{
    [SerializeField] private Text _textLabel;
    [SerializeField] private Text _bonusTextLabel;
    [SerializeField] private GameObject _headLabel;

    public void SetCount(int val) => _textLabel.text = val.ToString();
    public void SetBonusCount(int val) => _bonusTextLabel.text = "+" + val.ToString();
    public void HideHead() => _headLabel.SetActive(false);
    public void SetHead(string text) => _headLabel.GetComponent<Text>().text = text;
}
