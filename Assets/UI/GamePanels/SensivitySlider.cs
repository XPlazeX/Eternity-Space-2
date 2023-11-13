using UnityEngine;
using UnityEngine.UI;

public class SensivitySlider : MonoBehaviour
{
    const float _multiplier = 1f;

    [SerializeField] private Text _label;

    private Slider _slider;

    private void Start() 
    {
        float val = PlayerPrefs.GetFloat("Sensivity", _multiplier);

        _slider = GetComponent<Slider>();

        _slider.value = val * 10f;
        _label.text = val.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    public void ValueChange()
    {
        float val = _slider.value / 10f;

        PlayerPrefs.SetFloat("Sensivity", val);
        //print($"set sensivity : {val}");
        GameObject.FindWithTag("Sensor").GetComponent<PlayerController>().ChangeSensivity(val * _multiplier);
        _label.text = val.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
}
