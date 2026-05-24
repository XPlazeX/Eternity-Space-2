using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthMonitorUI : MonoBehaviour
{
    private const int CRITICAL_COLOR_NORMAL_INDEX = 0;
    private const int CRITICAL_COLOR_CRITICAL_INDEX = 1;

    [Header("Health")]
    [SerializeField] private bool useFillImage;
    [SerializeField] private Image fillImage;
    [SerializeField] private bool useAmountLabel;
    [SerializeField] private TMP_Text amountLabel;
    [SerializeField] private float visualChangeSpeed = 5f; // Lerp
    [SerializeField] private bool changeColorOverHealth;
    [SerializeField] private Gradient healthGradient;
    [SerializeField] private Graphic[] coloringGraphics;

    [Header("Structural Damage")]
    [SerializeField] private bool observeStructuralDamage;
    [SerializeField] private CanvasGroup structuralIndicator;
    [SerializeField] private Image structuralFillAmount;

    // Пока не используем
    [Header("Ramming")]
    [SerializeField] private bool observeRamming;
    [SerializeField] private bool useRamFillImage;
    [SerializeField] private Image ramFillImage;
    [SerializeField] private bool useRamAmountLabel;
    [SerializeField] private TMP_Text ramAmountLabel;
    [SerializeField] private float visualRamChangeSpeed = 5f; // Lerp

    [Header("Shield")] // пока не используем
    [SerializeField] private bool observeShield;
    [SerializeField] private CanvasGroup shieldGroup;
    [SerializeField] private Image shieldColoringImage;
    [SerializeField] private bool useShieldAmountLabel;
    [SerializeField] private TMP_Text shieldAmountLabel; 
    [SerializeField] private float visualShieldChangeSpeed = 5f; // Lerp

    [Header("Critical state")]
    [SerializeField] private bool useCriticalColoring;
    [Tooltip("Цвет 0 - по умолчанию, Цвет 1 - в критическом режиме")]
    [SerializeField] private ColoringGraphicsContainer[] criticalColoringGraphics;
    [SerializeField] private float criticalTransitionAnimationTime = 1f; // Move

    [Header("Other indicators")]
    [SerializeField] private bool observeDamageBuffers;
    [SerializeField] private CanvasGroup bufferIndicator;
    [SerializeField] private TMP_Text bufferCountLabel;

    private int _targetAnimatedHealth;
    private float _currentAnimatedHealth;
    private float _criticalTransitionTimer = 0f;
    private int _targetCriticalColorIndex;

    private void OnEnable()
    {
        PlayerShipData.HealthChanged += OnHealthChanged;

        if (observeStructuralDamage)
        {
            PlayerShipData.StructuralDamageChanged += OnStructuralDamageChanged;
        }
        if (observeShield)
        {
            PlayerShipData.ShieldCreated += OnShieldCreated;
            PlayerShipData.ShieldDamageTaked += OnShieldDamageTaked;
            PlayerShipData.ShieldBreaked += OnShieldBreaked;
        }
        if (useCriticalColoring)
        {
            PlayerShipData.CriticalStateChanged += OnCriticalStateChanged;
        }
        if (observeDamageBuffers)
        {
            PlayerShipData.DamageBufferGetted += OnBufferGetted;
            PlayerShipData.DamageBuffer1Losed += OnBuffer1Losed;
        }
    }

    void OnDisable()
    {
        PlayerShipData.HealthChanged -= OnHealthChanged;

        if (observeStructuralDamage)
        {
            PlayerShipData.StructuralDamageChanged -= OnStructuralDamageChanged;
        }
        if (observeShield)
        {
            PlayerShipData.ShieldCreated -= OnShieldCreated;
            PlayerShipData.ShieldDamageTaked -= OnShieldDamageTaked;
            PlayerShipData.ShieldBreaked -= OnShieldBreaked;
        }
        if (useCriticalColoring)
        {
            PlayerShipData.CriticalStateChanged -= OnCriticalStateChanged;
        }
        if (observeDamageBuffers)
        {
            PlayerShipData.DamageBufferGetted -= OnBufferGetted;
            PlayerShipData.DamageBuffer1Losed -= OnBuffer1Losed;
        }
    }

    void Update()
    {
        // HEALTH

        _currentAnimatedHealth = Mathf.Lerp(_currentAnimatedHealth, _targetAnimatedHealth, visualChangeSpeed * Time.unscaledDeltaTime);

        if (Mathf.Abs(_currentAnimatedHealth - _targetAnimatedHealth) < 0.01f)
        {
            _currentAnimatedHealth = _targetAnimatedHealth;
        }

        if (useFillImage)
        {
            fillImage.fillAmount = _currentAnimatedHealth / PlayerShipData.MaxHP;
        }
        if (useAmountLabel)
        {
            amountLabel.text = Mathf.FloorToInt(_currentAnimatedHealth).ToString();
        }

        // CRITICAL

        if (_criticalTransitionTimer > 0f)
        {
            _criticalTransitionTimer -= Time.unscaledDeltaTime;

            for (int i = 0; i < criticalColoringGraphics.Length; i++)
            {
                criticalColoringGraphics[i].SetColor(_targetCriticalColorIndex, 1f - (_criticalTransitionTimer / criticalTransitionAnimationTime));
            }
        }
    }

    private void OnHealthChanged(int newHealth)
    {
        float healthPart = (float)newHealth / PlayerShipData.MaxHP;

        _targetAnimatedHealth = newHealth;

        if (changeColorOverHealth)
        {
            for (int i = 0; i < coloringGraphics.Length; i++)
            {
                coloringGraphics[i].color = healthGradient.Evaluate(1f - healthPart);
            }
        }
    }

    private void OnCriticalStateChanged(bool isCritical)
    {
        _targetCriticalColorIndex = isCritical ? CRITICAL_COLOR_CRITICAL_INDEX : CRITICAL_COLOR_NORMAL_INDEX;
        _criticalTransitionTimer = criticalTransitionAnimationTime;
    }

    private void OnShieldCreated()
    {
        
    }

    private void OnShieldDamageTaked(int dmg)
    {
        int shieldPoints = PlayerShipData.ShieldPoints;
    }

    private void OnShieldBreaked()
    {
        
    }

    private void OnBufferGetted()
    {
        
    }

    private void OnBuffer1Losed()
    {
        
    }

    private void OnStructuralDamageChanged(int newStructural)
    {
        
    }
}

[System.Serializable]
public class ColoringGraphicsContainer
{
    [SerializeField] private Graphic[] graphics;
    [SerializeField] private Color[] colors;

    public void SetColor(int colorIndex)
    {
        for (int i = 0; i < graphics.Length; i++)
        {
            graphics[i].color = colors[colorIndex];
        }   
    }

    public void SetColor(int colorIndex, float t)
    {
        for (int i = 0; i < graphics.Length; i++)
        {
            graphics[i].color = Color.Lerp(graphics[i].color, colors[colorIndex], t);
        }   
    }
}