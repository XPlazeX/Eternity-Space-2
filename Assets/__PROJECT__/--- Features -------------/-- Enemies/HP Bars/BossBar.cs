using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    private const float x_position = -72.125f;
    private const float y_position = -110f;
    private const float y_step = -74f;

    [SerializeField] private Image _fillImage;
    [SerializeField] private Text _percentageLabel;
    [SerializeField] private Gradient _textGradient;
    [SerializeField] private Color _ramReadyColor;

    private int _maxHp;
    private int _bindedPosition = 0;

    public void Initialize(int maxHp, int order)
    {
        _maxHp = maxHp;

        UpdateLabel(maxHp);

        transform.SetParent(GameObject.FindWithTag("PassiveUI").transform);
        transform.localScale = Vector3.one;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(x_position, y_position + y_step * order);

        _bindedPosition = order;
    }

    public void UpdateLabel(int hp)
    {
        _fillImage.fillAmount = (float)hp / _maxHp;
        _percentageLabel.text = $"{(((float)hp / _maxHp) * 100).ToString("0.0", System.Globalization.CultureInfo.GetCultureInfo("en-US"))}%";

        if (hp <= ShipStats.GetIntValue("DecadesBlockForRam") * 10)
        {
            _percentageLabel.color = _ramReadyColor;
            _fillImage.color = _ramReadyColor;
            return;
        }

        _percentageLabel.color = _textGradient.Evaluate(1f - (float)hp / _maxHp);
        _fillImage.color = _textGradient.Evaluate(1f - (float)hp / _maxHp);
    }

    public void Death()
    {
        UpdateLabel(0);
        GetComponent<Animator>().SetTrigger("Death");
        SceneStatics.SceneCore.GetComponent<BossDistributor>().UnregisterBossBar(_bindedPosition);
        //print("empty BB Death");
    }
}
