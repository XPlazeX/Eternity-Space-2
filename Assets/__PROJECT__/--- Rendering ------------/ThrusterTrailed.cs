using UnityEngine;

[DisallowMultipleComponent]
public sealed class ThrusterTrailed : MonoBehaviour
{
    [Header("Movement Source")]
    [Tooltip("Transform whose movement will be tracked. If null, this transform is used.")]
    [SerializeField] private Transform movementSource;

    [Tooltip("Reference for angle calculation. If null, this transform is used. Movement angle is calculated relative to reference.up.")]
    [SerializeField] private Transform angleReference;

    [Tooltip("If enabled, movement direction is converted into angleReference local space.")]
    [SerializeField] private bool useReferenceLocalSpace = true;

    [Tooltip("Tiny movement below this value is treated as no movement.")]
    [SerializeField] private float minMovementDelta = 0.0001f;

    [Header("Angle Range")]
    [Tooltip("Start of full-power angle range, in degrees, relative to reference.up.")]
    [SerializeField] private float fullPowerAngleMin = 0f;

    [Tooltip("End of full-power angle range, in degrees, relative to reference.up.")]
    [SerializeField] private float fullPowerAngleMax = 90f;

    [Tooltip("Fade range outside full-power sector. Example: 20 means power fades from 1 to 0 in -20..0 and 90..110.")]
    [Min(0f)]
    [SerializeField] private float fadeAngle = 20f;

    [Header("Power")]
    [Tooltip("How quickly visual power follows target power. 0 = instant.")]
    [Min(0f)]
    [SerializeField] private float smoothingSpeed = 20f;

    [Tooltip("Optional multiplier for final power.")]
    [Range(0f, 2f)]
    [SerializeField] private float powerMultiplier = 1f;

    [Header("High Power Color")]
    [SerializeField] private bool useHighPowerColor = true;

    [Tooltip("Color that thruster starts blending to at high movement delta.")]
    [SerializeField] private Color highPowerColor = new Color(0.4f, 0.8f, 1f, 1f);

    [Tooltip("Movement delta where high power color starts appearing.")]
    [Min(0f)]
    [SerializeField] private float highPowerDeltaMin = 0.08f;

    [Tooltip("Movement delta where high power color reaches full strength.")]
    [Min(0f)]
    [SerializeField] private float highPowerDeltaMax = 0.25f;

    [Tooltip("Angle power needed before high power color can appear. 1 = only when thruster works exactly in its full-power sector.")]
    [Range(0f, 1f)]
    [SerializeField] private float highPowerMinAnglePower = 0.85f;

    [Tooltip("How strongly high power color overrides normal gradient color.")]
    [Range(0f, 1f)]
    [SerializeField] private float highPowerColorStrength = 1f;

    [Tooltip("How quickly high power color follows target. 0 = instant.")]
    [Min(0f)]
    [SerializeField] private float highPowerColorSmoothingSpeed = 25f;

    [Header("Sprite Renderer")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Tooltip("Sprite color by power.")]
    [SerializeField] private Gradient spriteColorByPower = DefaultThrusterGradient();

    [Header("Trail Renderer")]
    [SerializeField] private TrailRenderer trailRenderer;

    [Tooltip("Trail head color by power. Evaluated color is applied to the first color key.")]
    [SerializeField] private Gradient trailHeadColorByPower = DefaultThrusterGradient();

    [Tooltip("Maximum trail lifetime at power = 1.")]
    [Min(0f)]
    [SerializeField] private float maxTrailTime = 0.2f;

    [Tooltip("If true, trail is cleared when power reaches zero.")]
    [SerializeField] private bool clearTrailWhenPowerZero = true;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem particles;

    [Tooltip("Emission rateOverTime at power = 1.")]
    [Min(0f)]
    [SerializeField] private float maxEmissionRate = 40f;

    [Header("Debug")]
    [SerializeField, Range(0f, 1f)] private float debugPower;
    [SerializeField] private bool drawGizmos = true;
    [SerializeField] private float gizmoLength = 1.5f;
    [SerializeField, Range(0f, 1f)] private float debugHighPowerColorPower;
    [SerializeField] private float debugMovementDelta;
    [SerializeField, Range(0f, 1f)] private float debugAnglePower;

    private Vector3 previousPosition;
    private float currentPower;
    private bool hadPreviousPosition;
    private float currentHighPowerColorPower;

    public float CurrentPower => currentPower;

    private void Reset()
    {
        movementSource = transform.root != null ? transform.root : transform;
        angleReference = transform;

        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        particles = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        CacheInitialPosition();
        ApplyPower(0f, forceClearTrail: true);
    }

    private void Update()
    {
        Transform source = movementSource != null ? movementSource : transform;

        if (!hadPreviousPosition)
        {
            CacheInitialPosition();
            return;
        }

        Vector3 currentPosition = source.position;
        Vector3 delta = currentPosition - previousPosition;
        previousPosition = currentPosition;

        float movementDelta = delta.magnitude;

        float anglePower = CalculateTargetPower(delta);
        float targetPower = Mathf.Clamp01(anglePower * powerMultiplier);

        float targetHighPowerColorPower = CalculateHighPowerColorPower(
            movementDelta,
            anglePower
        );

        if (smoothingSpeed <= 0f)
        {
            currentPower = targetPower;
        }
        else
        {
            float t = 1f - Mathf.Exp(-smoothingSpeed * Time.deltaTime);
            currentPower = Mathf.Lerp(currentPower, targetPower, t);
        }

        if (highPowerColorSmoothingSpeed <= 0f)
        {
            currentHighPowerColorPower = targetHighPowerColorPower;
        }
        else
        {
            float t = 1f - Mathf.Exp(-highPowerColorSmoothingSpeed * Time.deltaTime);
            currentHighPowerColorPower = Mathf.Lerp(currentHighPowerColorPower, targetHighPowerColorPower, t);
        }

        debugMovementDelta = movementDelta;
        debugAnglePower = anglePower;
        debugHighPowerColorPower = currentHighPowerColorPower;

        ApplyPower(currentPower, forceClearTrail: false);
    }

    private float CalculateHighPowerColorPower(float movementDelta, float anglePower)
    {
        if (!useHighPowerColor)
            return 0f;

        if (highPowerDeltaMax <= highPowerDeltaMin)
            return 0f;

        // Насколько большая дельта перемещения.
        float speedPower = Mathf.InverseLerp(
            highPowerDeltaMin,
            highPowerDeltaMax,
            movementDelta
        );

        // Насколько этот конкретный ускоритель работает "в свою сторону".
        // Если anglePower низкий, цвет мощности не включается.
        float angleGate = Mathf.InverseLerp(
            highPowerMinAnglePower,
            1f,
            anglePower
        );

        return Mathf.Clamp01(speedPower * angleGate * highPowerColorStrength);
    }

    private void CacheInitialPosition()
    {
        Transform source = movementSource != null ? movementSource : transform;
        previousPosition = source.position;
        hadPreviousPosition = true;
    }

    private float CalculateTargetPower(Vector3 worldDelta)
    {
        if (worldDelta.sqrMagnitude < minMovementDelta * minMovementDelta)
            return 0f;

        Transform reference = angleReference != null ? angleReference : transform;

        Vector2 direction;

        if (useReferenceLocalSpace)
        {
            Vector3 localDelta = reference.InverseTransformDirection(worldDelta);
            direction = new Vector2(localDelta.x, localDelta.y).normalized;
        }
        else
        {
            direction = new Vector2(worldDelta.x, worldDelta.y).normalized;
        }

        if (direction.sqrMagnitude <= 0.000001f)
            return 0f;

        // Angle relative to Vector2.up.
        // Up = 0 degrees.
        // Right = -90 degrees.
        // Left = 90 degrees.
        float angle = Vector2.SignedAngle(Vector2.up, direction);

        return EvaluateAngleRange(angle, fullPowerAngleMin, fullPowerAngleMax, fadeAngle);
    }

    private static float EvaluateAngleRange(float angle, float min, float max, float fade)
    {
        angle = NormalizeAngle180(angle);
        min = NormalizeAngle180(min);
        max = NormalizeAngle180(max);

        // Simple non-wrapping case, e.g. 0..90.
        if (min <= max)
            return EvaluateNonWrappingRange(angle, min, max, fade);

        // Wrapping case, e.g. 135..-135.
        // Treat as two ranges: min..180 and -180..max.
        float a = EvaluateNonWrappingRange(angle, min, 180f, fade);
        float b = EvaluateNonWrappingRange(angle, -180f, max, fade);
        return Mathf.Max(a, b);
    }

    private static float EvaluateNonWrappingRange(float angle, float min, float max, float fade)
    {
        if (angle >= min && angle <= max)
            return 1f;

        if (fade <= 0f)
            return 0f;

        if (angle < min && angle >= min - fade)
            return Mathf.InverseLerp(min - fade, min, angle);

        if (angle > max && angle <= max + fade)
            return Mathf.InverseLerp(max + fade, max, angle);

        return 0f;
    }

    private static float NormalizeAngle180(float angle)
    {
        angle %= 360f;

        if (angle > 180f)
            angle -= 360f;
        else if (angle < -180f)
            angle += 360f;

        return angle;
    }

    private void ApplyPower(float power, bool forceClearTrail)
    {
        debugPower = power;

        ApplySprite(power);
        ApplyTrail(power, forceClearTrail);
        ApplyParticles(power);
    }

    private void ApplySprite(float power)
    {
        if (spriteRenderer == null)
            return;

        Color baseColor = spriteColorByPower.Evaluate(power);
        Color finalColor = ApplyHighPowerColor(baseColor);

        spriteRenderer.color = finalColor;
    }

    private Color ApplyHighPowerColor(Color baseColor)
    {
        float t = currentHighPowerColorPower;

        if (t <= 0f)
            return baseColor;

        Color targetColor = highPowerColor;

        // Альфу лучше оставить от базового градиента, чтобы highPowerColor
        // не ломал прозрачность ускорителя.
        targetColor.a = baseColor.a;

        return Color.Lerp(baseColor, targetColor, t);
    }

    private void ApplyTrail(float power, bool forceClearTrail)
    {
        if (trailRenderer == null)
            return;

        trailRenderer.time = maxTrailTime * power;

        Gradient gradient = trailRenderer.colorGradient;
        GradientColorKey[] colorKeys = gradient.colorKeys;
        GradientAlphaKey[] alphaKeys = gradient.alphaKeys;

        if (colorKeys == null || colorKeys.Length == 0)
        {
            colorKeys = new[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            };
        }

        Color trailHeadColor = trailHeadColorByPower.Evaluate(power);
        trailHeadColor = ApplyHighPowerColor(trailHeadColor);

        colorKeys[0].color = trailHeadColor;

        gradient.SetKeys(colorKeys, alphaKeys);
        trailRenderer.colorGradient = gradient;

        if ((forceClearTrail || power <= 0.001f) && clearTrailWhenPowerZero)
            trailRenderer.Clear();
    }

    private void ApplyParticles(float power)
    {
        if (particles == null)
            return;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = maxEmissionRate * power;
    }

    private static Gradient DefaultThrusterGradient()
    {
        Gradient gradient = new Gradient();

        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(0f, 0f, 0f, 0f), 0f),
                new GradientColorKey(new Color(0.35f, 0.65f, 1f, 1f), 0.5f),
                new GradientColorKey(new Color(1f, 1f, 1f, 1f), 1f),
            },
            new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(1f, 1f),
            }
        );

        return gradient;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos)
            return;

        Transform reference = angleReference != null ? angleReference : transform;

        DrawAngleRay(reference, 180 + fullPowerAngleMin, Color.green);
        DrawAngleRay(reference, 180 + fullPowerAngleMax, Color.green);

        DrawAngleRay(reference, 180 + fullPowerAngleMin - fadeAngle, Color.yellow);
        DrawAngleRay(reference, 180 + fullPowerAngleMax + fadeAngle, Color.yellow);
    }

    private void DrawAngleRay(Transform reference, float angle, Color color)
    {
        Vector3 dir;

        if (useReferenceLocalSpace)
        {
            Quaternion rotation = Quaternion.AngleAxis(angle, reference.forward);
            dir = rotation * reference.up;
        }
        else
        {
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            dir = rotation * Vector3.up;
        }

        Gizmos.color = color;
        Gizmos.DrawLine(transform.position, transform.position + dir.normalized * gizmoLength);
    }
#endif
}