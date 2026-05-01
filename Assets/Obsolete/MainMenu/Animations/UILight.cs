using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UILight : MonoBehaviour
{
    [SerializeField] private Image[] _lightningImages;
    [SerializeField] private Text[] _lightningTextes;

    public void SetLightPower(float power = 0f)
    {
        Color color = new Color(1f * power, 1f * power, 1f * power);

        for (int i = 0; i < _lightningImages.Length; i++)
        {
            _lightningImages[i].color = color;
        }

        for (int i = 0; i < _lightningTextes.Length; i++)
        {
            _lightningTextes[i].color = new Color (_lightningTextes[i].color.r, _lightningTextes[i].color.g, _lightningTextes[i].color.b, 1f * power);
        }
    }
}
