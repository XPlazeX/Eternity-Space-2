using System;
using UnityEngine;

public class FGBWeaponMonitor : MonoBehaviour
{
    [Serializable]
    private class GearIcon
    {
        public GearSlot gear;
        public RectTransform rect;
        public RectTransform deviceIcon;
    }

    [Header("Gearbox")]
    [SerializeField] private FGB gearbox;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform neutralIcon;
    [SerializeField] private GearIcon[] gearIcons;

    [Header("Devices")]
    [SerializeField] private RectTransform visiblePivot;
    [SerializeField] private RectTransform hiddenPivot;
    [SerializeField, Min(0f)] private float deviceAnimationSpeed = 10f;

    [Header("Movement")]
    [Tooltip("Ход полотна от центра до крайней передачи в пикселях.")]
    [SerializeField] private Vector2 canvasTravel = new Vector2(100f, 100f);

    [Header("Scale")]
    [SerializeField, Range(0.01f, 1f)] private float clutchCanvasScale = 0.75f;
    [SerializeField, Min(1f)] private float selectedIconScale = 1.5f;
    [SerializeField, Min(0f)] private float animationSpeed = 14f;

    private Vector2 _canvasRestPosition;
    private Vector2 _neutralRestPosition;
    private Vector3 _canvasRestScale;
    private Vector3 _neutralRestScale;
    private Vector3[] _gearRestScales;
    private Vector2 _animatedLeverPosition;

    private void Awake()
    {
        if (canvas != null)
        {
            _canvasRestPosition = canvas.anchoredPosition;
            _canvasRestScale = canvas.localScale;
        }

        if (neutralIcon != null)
        {
            _neutralRestPosition = neutralIcon.anchoredPosition;
            _neutralRestScale = neutralIcon.localScale;
        }

        _gearRestScales = new Vector3[gearIcons != null ? gearIcons.Length : 0];
        for (int i = 0; i < _gearRestScales.Length; i++)
        {
            if (gearIcons[i] != null && gearIcons[i].rect != null)
            {
                _gearRestScales[i] = gearIcons[i].rect.localScale;
                gearIcons[i].deviceIcon.gameObject.SetActive(true);
            }
                
        }

        SetDevicePositionsImmediately();
    }

    void Start()
    {
        if (Player.PlayerObject != null)
        {
            gearbox = Player.PlayerObject.GetComponentInChildren<FGB>();
            _animatedLeverPosition = gearbox.NormalizedLeverPosition;
        } else
        {
            Player.PlayerChanged += OnPlayerChanged;
        }    
    }

    private void OnPlayerChanged()
    {
        gearbox = Player.PlayerObject.GetComponentInChildren<FGB>();
        _animatedLeverPosition = gearbox.NormalizedLeverPosition;
        Player.PlayerChanged -= OnPlayerChanged;
    }

    private void LateUpdate()
    {
        if (gearbox == null || canvas == null)
            return;

        Vector2 lever = gearbox.NormalizedLeverPosition;
        GearSlot selectedGear = gearbox.IsOpen ? gearbox.HoveredGear : gearbox.CurrentGear;
        float selectedMultiplier = gearbox.IsOpen ? selectedIconScale : 1f;
        float blend = animationSpeed <= 0f
            ? 1f
            : 1f - Mathf.Exp(-animationSpeed * ESTime.unscaledDeltaTime);
        float deviceBlend = deviceAnimationSpeed <= 0f
            ? 1f
            : 1f - Mathf.Exp(-deviceAnimationSpeed * ESTime.unscaledDeltaTime);
        _animatedLeverPosition = Vector2.Lerp(_animatedLeverPosition, lever, blend);

        float canvasScaleMultiplier = gearbox.IsOpen ? clutchCanvasScale : 1f;
        Vector3 canvasTargetScale = _canvasRestScale * canvasScaleMultiplier;
        canvas.localScale = Vector3.Lerp(canvas.localScale, canvasTargetScale, blend);

        // При уменьшении полотна визуальное расстояние между иконками тоже уменьшается.
        // Компенсация должна учитывать фактический (в том числе анимируемый) масштаб.
        float currentScaleMultiplier = Mathf.Approximately(_canvasRestScale.x, 0f)
            ? canvasScaleMultiplier
            : canvas.localScale.x / _canvasRestScale.x;
        Vector2 scaledTravel = canvasTravel * currentScaleMultiplier;
        canvas.anchoredPosition =
            _canvasRestPosition - Vector2.Scale(_animatedLeverPosition, scaledTravel);

        if (neutralIcon != null)
        {
            // Компенсирует X полотна: нейтраль следует за рычагом только по горизонтали.
            neutralIcon.anchoredPosition = _neutralRestPosition +
                Vector2.right * (_animatedLeverPosition.x * canvasTravel.x);

            Vector3 neutralTargetScale = _neutralRestScale *
                (selectedGear == GearSlot.Neutral ? selectedMultiplier : 1f);
            neutralIcon.localScale = Vector3.Lerp(neutralIcon.localScale, neutralTargetScale, blend);
        }

        for (int i = 0; i < _gearRestScales.Length; i++)
        {
            GearIcon icon = gearIcons[i];
            if (icon == null)
                continue;

            if (icon.rect != null)
            {
                Vector3 targetScale = _gearRestScales[i] *
                    (icon.gear == selectedGear ? selectedMultiplier : 1f);
                icon.rect.localScale = Vector3.Lerp(icon.rect.localScale, targetScale, blend);
            }

            if (icon.deviceIcon != null && visiblePivot != null && hiddenPivot != null)
            {
                Vector3 targetPosition = icon.gear == selectedGear
                    ? visiblePivot.position
                    : hiddenPivot.position;
                icon.deviceIcon.position = Vector3.Lerp(
                    icon.deviceIcon.position,
                    targetPosition,
                    deviceBlend
                );
            }
        }
    }

    private void SetDevicePositionsImmediately()
    {
        if (gearbox == null || gearIcons == null || visiblePivot == null || hiddenPivot == null)
            return;

        GearSlot selectedGear = gearbox.IsOpen ? gearbox.HoveredGear : gearbox.CurrentGear;

        foreach (GearIcon icon in gearIcons)
        {
            if (icon == null || icon.deviceIcon == null)
                continue;

            icon.deviceIcon.position = icon.gear == selectedGear
                ? visiblePivot.position
                : hiddenPivot.position;
        }
    }
}
