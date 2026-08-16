using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SledgeHealthUI : MonoBehaviour
{
    [SerializeField] private SledgeBody sledgeBody;
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text healthLabel;

    private void OnEnable() {
        sledgeBody.HealthChanged += OnHealthChanged;
        OnHealthChanged(0);
    }

    void OnDisable()
    {
        sledgeBody.HealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int h)
    {
        fillImage.fillAmount = (float)sledgeBody.HitPoints / sledgeBody.MaxHP;
        healthLabel.text = $"{sledgeBody.HitPoints}/{sledgeBody.CurrentMaxHitPoints}";
    }
}
