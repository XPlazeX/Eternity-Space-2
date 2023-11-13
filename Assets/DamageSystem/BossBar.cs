using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    private const float x_position = -100f;
    private const float y_position = 190f;
    private const float y_step = 80f;

    [SerializeField] private Image _fillImage;
    [SerializeField] private Text _percentageLabel;
    [SerializeField] private Gradient _textGradient;

    private int _maxHp;

    public void Initialize(int maxHp, int order)
    {
        _maxHp = maxHp;

        UpdateLabel(maxHp);

        transform.SetParent(GameObject.FindWithTag("PassiveUI").transform);
        transform.localScale = Vector3.one;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(x_position, y_position + y_step * order);
    }

    public void UpdateLabel(int hp)
    {
        _fillImage.fillAmount = (float)hp / _maxHp;
        _percentageLabel.text = $"{(((float)hp / _maxHp) * 100).ToString("0.0", System.Globalization.CultureInfo.GetCultureInfo("en-US"))}%";
        _percentageLabel.color = _textGradient.Evaluate(1f - (float)hp / _maxHp);
    }

    public void Death()
    {
        UpdateLabel(0);
        GetComponent<Animator>().SetTrigger("Death");
        //print("empty BB Death");
    }
}
