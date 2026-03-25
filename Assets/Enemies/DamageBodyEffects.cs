using UnityEngine;

[DisallowMultipleComponent]
public class DamageBodyEffects : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DamageBody damageBody;
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private ParticleSystem stunParticleSystem;

    [Header("Base Colors")]
    [SerializeField] private Color defaultColor = Color.black;
    [SerializeField] private Color onDamageColor = new Color(1f, 0.16f, 0f, 1f);
    [SerializeField] private Color onDamageFlatArmorColor = new Color(0.58f, 0.28f, 0f, 1f);
    [SerializeField] private Color onDamageReductionColor = new Color(0.16f, 0.39f, 0.5f, 1f);
    [SerializeField] private Color onRegeneratedColor = new Color(0.28f, 1f, 0f, 1f);
    [SerializeField] private Color ramReadyColorPulse = new Color(0.15f, 0f, 0.05f, 1f);

    [Header("One-shot Pulse")]
    [SerializeField] [Min(0.01f)] private float effectPulseDuration = 0.18f;

    [Header("Ram Ready Pulse")]
    [SerializeField] [Min(0.01f)] private float ramPulseFrequency = 1.2f;
    [SerializeField] [Range(0f, 1f)] private float ramPulseStrength = 0.25f;
    [SerializeField] [Min(0.01f)] private float ramFadeInSpeed = 6f;
    [SerializeField] [Min(0.01f)] private float ramFadeOutSpeed = 4f;

    [Header("Debug")]
    [SerializeField] private bool autoResolveOnAwake = true;

    private float _oneShotTimer;
    private Color _oneShotColor;
    private bool _hasOneShot;

    private float _ramBlend;
    private float _ramTime;

    private bool _subscribed;

    private void Awake()
    {
        if (autoResolveOnAwake)
        {
            ResolveReferences();
        }

        ApplyImmediateColor(defaultColor);
    }

    private void OnEnable()
    {
        ResolveReferences();
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Reset()
    {
        ResolveReferences();
    }

    private void OnValidate()
    {
        effectPulseDuration = Mathf.Max(0.01f, effectPulseDuration);
        ramPulseFrequency = Mathf.Max(0.01f, ramPulseFrequency);
        ramFadeInSpeed = Mathf.Max(0.01f, ramFadeInSpeed);
        ramFadeOutSpeed = Mathf.Max(0.01f, ramFadeOutSpeed);
    }

    private void Update()
    {
        UpdateRamPulse();
        UpdateOneShot();
        UpdateFinalColor();
    }

    public void ResolveReferences()
    {
        if (damageBody == null)
        {
            damageBody = GetComponentInParent<DamageBody>();
        }

        bool needsAutoResolveSR =
            spriteRenderers == null ||
            spriteRenderers.Length == 0 ||
            AllEntriesNull(spriteRenderers);

        if (needsAutoResolveSR && damageBody != null)
        {
            SpriteRenderer resolved = damageBody.GetComponent<SpriteRenderer>();

            if (resolved == null)
            {
                resolved = damageBody.GetComponentInChildren<SpriteRenderer>(true);
            }

            if (resolved != null)
            {
                spriteRenderers = new[] { resolved };
            }
        }
    }

    private void Subscribe()
    {
        if (_subscribed || damageBody == null)
            return;

        damageBody.DamageTaking += OnDamageTaking;
        damageBody.Regenerated += OnRegenerated;
        damageBody.Stunned += OnStunned;
        damageBody.Unstunned += OnUnstunned;

        _subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_subscribed || damageBody == null)
            return;

        damageBody.DamageTaking -= OnDamageTaking;
        damageBody.Regenerated -= OnRegenerated;
        damageBody.Stunned -= OnStunned;
        damageBody.Unstunned -= OnUnstunned;

        _subscribed = false;
    }

    private void OnDamageTaking(int damage)
    {
        if (damageBody == null)
            return;

        // Приоритет: обычный урон < FlatArmor < DamageReduction
        Color chosen = onDamageColor;

        if (damageBody.FlatArmor > 0)
        {
            chosen = onDamageFlatArmorColor;
        }

        if (damageBody.DamageReduction > 0f)
        {
            chosen = onDamageReductionColor;
        }

        PlayOneShot(chosen);
    }

    private void OnRegenerated(int amount)
    {
        PlayOneShot(onRegeneratedColor);
    }

    private void OnStunned()
    {
        if (stunParticleSystem == null)
            return;

        stunParticleSystem.Play(true);
    }

    private void OnUnstunned()
    {
        if (stunParticleSystem == null)
            return;

        stunParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void PlayOneShot(Color color)
    {
        _oneShotColor = color;
        _oneShotTimer = effectPulseDuration;
        _hasOneShot = true;
    }

    private void UpdateOneShot()
    {
        if (!_hasOneShot)
            return;

        _oneShotTimer -= Time.deltaTime;
        if (_oneShotTimer <= 0f)
        {
            _oneShotTimer = 0f;
            _hasOneShot = false;
        }
    }

    private void UpdateRamPulse()
    {
        bool ramReady = damageBody != null && damageBody.RamReady;

        float targetBlend = ramReady ? 1f : 0f;
        float speed = ramReady ? ramFadeInSpeed : ramFadeOutSpeed;
        _ramBlend = Mathf.MoveTowards(_ramBlend, targetBlend, speed * Time.deltaTime);

        _ramTime += Time.deltaTime;
    }

    private void UpdateFinalColor()
    {
        Color baseColor = defaultColor;

        // Базовая мягкая пульсация тарана
        if (_ramBlend > 0f)
        {
            float wave = (Mathf.Sin(_ramTime * ramPulseFrequency * Mathf.PI * 2f) + 1f) * 0.5f;
            float pulseT = wave * ramPulseStrength * _ramBlend;
            baseColor = Color.Lerp(defaultColor, ramReadyColorPulse, pulseT);
        }

        Color finalColor = baseColor;

        // Одноразовый эффект лежит поверх текущей базы.
        // Не смешиваем между собой разные one-shot эффекты — только один активный.
        if (_hasOneShot && effectPulseDuration > 0f)
        {
            float t = _oneShotTimer / effectPulseDuration;
            finalColor = Color.Lerp(baseColor, _oneShotColor, t);
        }

        ApplyImmediateColor(finalColor);
    }

    private void ApplyImmediateColor(Color color)
    {
        if (spriteRenderers == null)
            return;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            SpriteRenderer sr = spriteRenderers[i];
            if (sr == null)
                continue;

            sr.color = color;
        }
    }

    private static bool AllEntriesNull(SpriteRenderer[] array)
    {
        if (array == null || array.Length == 0)
            return true;

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != null)
                return false;
        }

        return true;
    }
}