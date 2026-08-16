using System.Collections;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthMonitorUI : MonoBehaviour
{
    private const int LayerSize = 100;

    [Header("Animation")]
    [SerializeField, Min(0f)] private float _animationTime = 0.25f;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _hurtingColor = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private Color _regeneratingColor = new Color(0.2f, 1f, 0.45f);
    [SerializeField] private Color _sealedActiveColor = Color.white;
    [SerializeField] private Color _sealedInactiveColor = new Color(1f, 1f, 1f, 0.25f);

    [Header("Shell")]
    [SerializeField] private ColoringGraphicsContainer _shellAnimatedGraphics;
    [SerializeField] private Image _shellFill;
    [SerializeField] private TMP_Text _shellLabel;
    [SerializeField] private Image _shellSealedIndicator;
    [SerializeField] private Image _shellStructureDamageFill;
    [SerializeField] private CanvasGroup _shellBrokenIndicator;

    [Header("Systems")]
    [SerializeField] private ColoringGraphicsContainer _systemsAnimatedGraphics;
    [SerializeField] private Image _systemsFill;
    [SerializeField] private TMP_Text _systemsLabel;
    [SerializeField] private Image _systemsSealedIndicator;
    [SerializeField] private CanvasGroup[] _systemsBrokenIndicators = new CanvasGroup[3];

    [Header("Core")]
    [SerializeField] private ColoringGraphicsContainer _coreAnimatedGraphics;
    [SerializeField] private Image _coreFill;
    [SerializeField] private TMP_Text _coreLabel;
    [SerializeField] private Image _coreSealedIndicator;
    [SerializeField] private CanvasGroup _coreBrokenIndicator;

    [Header("Damage Buffer")]
    [SerializeField] private ColoringGraphicsContainer _bufferAnimatedGraphics;
    [SerializeField] private CanvasGroup _bufferCanvasGroup;
    [SerializeField] private Color _bufferNormalColor = Color.white;
    [SerializeField] private Color _bufferLosingColor = new Color(1f, 0.2f, 0.2f);

    [Header("Shield")]
    [SerializeField] private ColoringGraphicsContainer _shieldAnimatedGraphics;
    [SerializeField] private CanvasGroup _shieldCanvasGroup;
    [SerializeField] private Color _shieldNormalColor = Color.white;
    [SerializeField] private Color _shieldGettingColor = new Color(0.2f, 1f, 0.45f);
    [SerializeField] private Color _shieldHurtingColor = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private Image _shieldFill;
    [SerializeField] private TMP_Text _shieldLabel;

    private int _healthBeforeLastChange;
    private int _lastKnownHealth;

    private Coroutine _shellFillAnimation;
    private Coroutine _systemsFillAnimation;
    private Coroutine _coreFillAnimation;
    private Coroutine _shellBrokenAnimation;
    private Coroutine _coreBrokenAnimation;
    private readonly Coroutine[] _systemsBrokenAnimations = new Coroutine[3];
    private Coroutine _shellColorAnimation;
    private Coroutine _systemsColorAnimation;
    private Coroutine _coreColorAnimation;
    private Coroutine _bufferAlphaAnimation;
    private Coroutine _bufferColorAnimation;
    private Coroutine _shieldAlphaAnimation;
    private Coroutine _shieldColorAnimation;
    private Coroutine _shieldFillAnimation;

    private void OnEnable()
    {
        PlayerShipData.HealthChanged += OnHealthChanged;
        PlayerShipData.StructuralDamageChanged += OnStructuralDamageChanged;
        PlayerShipData.HealthDamageTaked += OnHealthDamageTaken;
        PlayerShipData.Regenerated += OnRegenerated;
        PlayerShipData.SystemsExposed += OnSystemsExposed;
        PlayerShipData.CoreExposed += OnCoreExposed;
        PlayerShipData.SystemsSealed += OnSystemsSealed;
        PlayerShipData.CoreSealed += OnCoreSealed;
        PlayerShipData.ShellSealed += OnShellSealed;
        PlayerShipData.DamageBufferGetted += OnDamageBufferGained;
        PlayerShipData.DamageBufferLosed += OnDamageBufferLost;
        PlayerShipData.ShieldCreated += OnShieldCreated;
        PlayerShipData.ShieldBreaked += OnShieldBroken;
        PlayerShipData.ShieldDamageTaked += OnShieldDamageTaken;

        _lastKnownHealth = PlayerShipData.HitPoints;
        _healthBeforeLastChange = _lastKnownHealth;
        RefreshHealth(PlayerShipData.HitPoints, false);
        OnStructuralDamageChanged(PlayerShipData.StructuralDamage);
        RefreshSealedIndicators();
        SetInitialColors();
        InitializeBuffer();
        InitializeShield();
    }

    private void OnDisable()
    {
        PlayerShipData.HealthChanged -= OnHealthChanged;
        PlayerShipData.StructuralDamageChanged -= OnStructuralDamageChanged;
        PlayerShipData.HealthDamageTaked -= OnHealthDamageTaken;
        PlayerShipData.Regenerated -= OnRegenerated;
        PlayerShipData.SystemsExposed -= OnSystemsExposed;
        PlayerShipData.CoreExposed -= OnCoreExposed;
        PlayerShipData.SystemsSealed -= OnSystemsSealed;
        PlayerShipData.CoreSealed -= OnCoreSealed;
        PlayerShipData.ShellSealed -= OnShellSealed;
        PlayerShipData.DamageBufferGetted -= OnDamageBufferGained;
        PlayerShipData.DamageBufferLosed -= OnDamageBufferLost;
        PlayerShipData.ShieldCreated -= OnShieldCreated;
        PlayerShipData.ShieldBreaked -= OnShieldBroken;
        PlayerShipData.ShieldDamageTaked -= OnShieldDamageTaken;
    }

    private void OnHealthChanged(int health)
    {
        _healthBeforeLastChange = _lastKnownHealth;
        _lastKnownHealth = health;

        RefreshSealedIndicators();
        RefreshHealth(health, true);
    }

    private void RefreshHealth(int health, bool animated)
    {
        int coreHealth = Mathf.Clamp(health, 0, LayerSize);
        int systemsHealth = Mathf.Clamp(health - PlayerShipData.CORE_BORDER, 0, LayerSize);
        int shellHealth = Mathf.Clamp(health - PlayerShipData.SHELL_BORDER, 0, LayerSize);

        SetFill(_coreFill, coreHealth / (float)LayerSize, animated, ref _coreFillAnimation);
        SetFill(_systemsFill, systemsHealth / (float)LayerSize, animated, ref _systemsFillAnimation);
        SetFill(_shellFill, shellHealth / (float)LayerSize, animated, ref _shellFillAnimation);

        SetLabel(_coreLabel, coreHealth);
        SetLabel(_systemsLabel, systemsHealth);
        SetLabel(_shellLabel, shellHealth);

        SetBroken(_coreBrokenIndicator, coreHealth <= 1, animated, ref _coreBrokenAnimation);
        SetBroken(_shellBrokenIndicator, shellHealth <= 0, animated, ref _shellBrokenAnimation);

        // From low to high: each indicator represents one destroyed third.
        SetSystemBroken(0, systemsHealth <= 0, animated);
        SetSystemBroken(1, systemsHealth <= 33, animated);
        SetSystemBroken(2, systemsHealth <= 66, animated);

    }

    private void OnStructuralDamageChanged(int structuralDamage)
    {
        structuralDamage = PlayerShipData.StructuralDamage;
        if (_shellStructureDamageFill != null)
        {
            _shellStructureDamageFill.fillAmount = Mathf.Clamp01(
                structuralDamage / (float)PlayerShipData.MaxStructuralDamage);
        }

        RefreshSealedIndicators();
    }

    private void OnHealthDamageTaken(int damage)
    {
        FlashLayersBetween(_healthBeforeLastChange, _lastKnownHealth, _hurtingColor);
    }

    private void OnRegenerated(int amount)
    {
        FlashLayersBetween(_healthBeforeLastChange, PlayerShipData.HitPoints, _regeneratingColor);
    }

    private void FlashLayersBetween(int fromHealth, int toHealth, Color flashColor)
    {
        int min = Mathf.Min(fromHealth, toHealth);
        int max = Mathf.Max(fromHealth, toHealth);

        if (min < PlayerShipData.CORE_BORDER && max > 0)
            AnimateLayerColor(_coreAnimatedGraphics, flashColor, () => CoreRestingColor, ref _coreColorAnimation);
        if (min < PlayerShipData.SHELL_BORDER && max > PlayerShipData.CORE_BORDER)
            AnimateLayerColor(_systemsAnimatedGraphics, flashColor, () => SystemsRestingColor, ref _systemsColorAnimation);
        if (max > PlayerShipData.SHELL_BORDER)
            AnimateLayerColor(_shellAnimatedGraphics, flashColor, () => ShellRestingColor, ref _shellColorAnimation);
    }

    private void OnSystemsExposed()
    {
        RefreshSealedIndicators();
        SetBroken(_shellBrokenIndicator, true, true, ref _shellBrokenAnimation);
        AnimateLayerColor(_shellAnimatedGraphics, _hurtingColor, () => ShellRestingColor, ref _shellColorAnimation);
    }

    private void OnCoreExposed()
    {
        RefreshSealedIndicators();
    }

    private void OnSystemsSealed()
    {
        RefreshSealedIndicators();
        SetBroken(_shellBrokenIndicator, false, true, ref _shellBrokenAnimation);
        AnimateLayerColor(_shellAnimatedGraphics, _shellAnimatedGraphics.CurrentColor, () => ShellRestingColor, ref _shellColorAnimation);
    }

    private void OnCoreSealed()
    {
        RefreshSealedIndicators();
    }

    private void OnShellSealed()
    {
        RefreshSealedIndicators();
    }

    private void InitializeBuffer()
    {
        _bufferAnimatedGraphics?.SetColor(_bufferNormalColor);
        SetBroken(_bufferCanvasGroup, PlayerShipData.DamageBuffers > 0, false, ref _bufferAlphaAnimation);
    }

    private void OnDamageBufferGained()
    {
        if (_bufferColorAnimation != null)
        {
            StopCoroutine(_bufferColorAnimation);
            _bufferColorAnimation = null;
        }
        _bufferAnimatedGraphics?.SetColor(_bufferNormalColor);
        SetBroken(_bufferCanvasGroup, true, true, ref _bufferAlphaAnimation);
    }

    private void OnDamageBufferLost()
    {
        Color start = _bufferAnimatedGraphics != null
            ? _bufferAnimatedGraphics.CurrentColor
            : _bufferNormalColor;

        SetBroken(_bufferCanvasGroup, false, true, ref _bufferAlphaAnimation);
        AnimateLayerColor(
            _bufferAnimatedGraphics,
            start,
            () => _bufferLosingColor,
            ref _bufferColorAnimation);
    }

    private void InitializeShield()
    {
        _shieldAnimatedGraphics?.SetColor(_shieldNormalColor);
        SetBroken(_shieldCanvasGroup, PlayerShipData.HasShield, false, ref _shieldAlphaAnimation);
        RefreshShield(false);
    }

    private void OnShieldCreated()
    {
        SetBroken(_shieldCanvasGroup, true, true, ref _shieldAlphaAnimation);
        RefreshShield(true);
        AnimateLayerColor(
            _shieldAnimatedGraphics,
            _shieldGettingColor,
            () => _shieldNormalColor,
            ref _shieldColorAnimation);
    }

    private void OnShieldBroken()
    {
        SetBroken(_shieldCanvasGroup, false, true, ref _shieldAlphaAnimation);
        SetLabel(_shieldLabel, 0);
        SetFill(_shieldFill, 0f, true, ref _shieldFillAnimation);
    }

    private void OnShieldDamageTaken(int damage)
    {
        RefreshShield(true);
        AnimateLayerColor(
            _shieldAnimatedGraphics,
            _shieldHurtingColor,
            () => _shieldNormalColor,
            ref _shieldColorAnimation);
    }

    private void RefreshShield(bool animated)
    {
        int shieldPoints = Mathf.Max(0, PlayerShipData.ShieldPoints);
        int maxShieldPoints = PlayerShipData.CurrentMaxShieldPoints;
        float fill = maxShieldPoints > 0 ? shieldPoints / (float)maxShieldPoints : 0f;

        SetLabel(_shieldLabel, shieldPoints);
        SetFill(_shieldFill, fill, animated, ref _shieldFillAnimation);
    }

    private void RefreshSealedIndicators()
    {
        SetIndicatorColor(_coreSealedIndicator, PlayerShipData.IsCoreSealed);
        SetIndicatorColor(_systemsSealedIndicator, PlayerShipData.IsSystemsSealed);
        SetIndicatorColor(_shellSealedIndicator, PlayerShipData.IsShellSealed);
    }

    private void SetInitialColors()
    {
        _coreAnimatedGraphics?.SetColor(_normalColor);
        _systemsAnimatedGraphics?.SetColor(_normalColor);
        _shellAnimatedGraphics?.SetColor(_normalColor);
    }

    private Color CoreRestingColor => PlayerShipData.HitPoints <= 1 ? _hurtingColor : _normalColor;
    private Color SystemsRestingColor => PlayerShipData.HitPoints <= PlayerShipData.CORE_BORDER ? _hurtingColor : _normalColor;
    private Color ShellRestingColor =>
        !PlayerShipData.IsSystemsSealed || PlayerShipData.HitPoints <= PlayerShipData.SHELL_BORDER
            ? _hurtingColor
            : _normalColor;

    private void SetFill(Image image, float target, bool animated, ref Coroutine animation)
    {
        if (image == null) return;
        if (animation != null) StopCoroutine(animation);
        if (!animated || _animationTime <= 0f)
        {
            image.fillAmount = target;
            animation = null;
            return;
        }
        animation = StartCoroutine(AnimateFill(image, target));
    }

    private IEnumerator AnimateFill(Image image, float target)
    {
        float start = image.fillAmount;
        float elapsed = 0f;
        while (elapsed < _animationTime)
        {
            elapsed += Time.unscaledDeltaTime;
            image.fillAmount = Mathf.Lerp(start, target, elapsed / _animationTime);
            yield return null;
        }
        image.fillAmount = target;
    }

    private void SetBroken(CanvasGroup indicator, bool shown, bool animated, ref Coroutine animation)
    {
        if (indicator == null) return;
        if (animation != null) StopCoroutine(animation);
        float target = shown ? 1f : 0f;
        if (!animated || _animationTime <= 0f)
        {
            indicator.alpha = target;
            animation = null;
            return;
        }
        animation = StartCoroutine(AnimateAlpha(indicator, target));
    }

    private void SetSystemBroken(int index, bool shown, bool animated)
    {
        if (_systemsBrokenIndicators == null || index >= _systemsBrokenIndicators.Length) return;
        SetBroken(_systemsBrokenIndicators[index], shown, animated, ref _systemsBrokenAnimations[index]);
    }

    private IEnumerator AnimateAlpha(CanvasGroup group, float target)
    {
        float start = group.alpha;
        float elapsed = 0f;
        while (elapsed < _animationTime)
        {
            elapsed += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(start, target, elapsed / _animationTime);
            yield return null;
        }
        group.alpha = target;
    }

    private void AnimateLayerColor(ColoringGraphicsContainer graphics, Color start, Func<Color> target, ref Coroutine animation)
    {
        if (graphics == null) return;
        if (animation != null) StopCoroutine(animation);
        graphics.SetColor(start);
        animation = StartCoroutine(AnimateColor(graphics, start, target));
    }

    private IEnumerator AnimateColor(ColoringGraphicsContainer graphics, Color start, Func<Color> target)
    {
        float elapsed = 0f;
        while (elapsed < _animationTime)
        {
            elapsed += Time.unscaledDeltaTime;
            graphics.SetColor(Color.Lerp(start, target(), elapsed / _animationTime));
            yield return null;
        }
        graphics.SetColor(target());
    }

    private void SetIndicatorColor(Image indicator, bool active)
    {
        if (indicator != null) indicator.color = active ? _sealedActiveColor : _sealedInactiveColor;
    }

    private static void SetLabel(TMP_Text label, int value)
    {
        if (label != null) label.SetText("{0}", value);
    }
}

[System.Serializable]
public class ColoringGraphicsContainer
{
    [SerializeField] private Graphic[] graphics;

    public Color CurrentColor
    {
        get
        {
            if (graphics == null || graphics.Length == 0 || graphics[0] == null) return Color.white;
            return graphics[0].color;
        }
    }

    public void SetColor(Color color)
    {
        if (graphics == null) return;
        for (int i = 0; i < graphics.Length; i++)
        {
            if (graphics[i] != null) graphics[i].color = color;
        }
    }
}
