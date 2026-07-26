using UnityEngine.UI;
using UnityEngine;

public class EnemyAlarmUI : MonoBehaviour
{
    [SerializeField] private Image[] warningFillImages;
    [SerializeField] private GameObject alarmObject;

    private EnemyHQ _hq;
    private bool _alarmed = false;

    private void OnEnable() {
        _hq = FindAnyObjectByType<EnemyHQ>();

        _hq.HQCalmed += OnHQCalmed;
        _hq.HQWarningUpdated += OnHQWarningUpdated;
        _hq.HQAlarmed += OnHQAlarmed;
        _hq.FarDoomActivated += OnFarDoomActivated;

        alarmObject.SetActive(_alarmed);
        OnHQWarningUpdated(0f);
    }

    void OnDisable()
    {
        _hq.HQCalmed -= OnHQCalmed;
        _hq.HQWarningUpdated -= OnHQWarningUpdated;
        _hq.HQAlarmed -= OnHQAlarmed;
        _hq.FarDoomActivated -= OnFarDoomActivated;
    }

    private void OnHQWarningUpdated(float warningLevel)
    {
        float f = _hq.WarningLevel01;

        for (int i = 0; i < warningFillImages.Length; i++)
        {
            warningFillImages[i].fillAmount = f;
        }
    }

    private void OnHQCalmed()
    {
        
    }

    private void OnHQAlarmed()
    {
        _alarmed = true;
        alarmObject.SetActive(true);

        for (int i = 0; i < warningFillImages.Length; i++)
        {
            warningFillImages[i].gameObject.SetActive(false);
        }
    }

    private void OnFarDoomActivated()
    {
        
    }
}
