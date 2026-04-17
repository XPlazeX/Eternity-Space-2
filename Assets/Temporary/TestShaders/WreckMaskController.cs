using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WreckMaskController : MonoBehaviour
{
    [Header("Shader params")]
    [Range(0.01f, 1f)] public float breachRadius = 0.22f;
    [Range(0.001f, 0.25f)] public float edgeWidth = 0.04f;
    [Range(1f, 40f)] public float noiseScale = 12f;
    [Range(0f, 0.5f)] public float noiseStrength = 0.07f;
    [Range(0f, 1f)] public float burnAmount = 0.75f;
    [Range(0f, 3f)] public float heatAmount = 1.2f;
    [Range(0f, 1f)] public float alpha = 1f;

    [Header("Alpha sampling")]
    [Range(0.01f, 1f)] public float opaqueThreshold = 0.15f;
    [Tooltip("Если world-hit попал в прозрачный пиксель, пробуем найти рядом непрозрачный.")]
    public bool snapWorldHitToOpaque = true;

    [Tooltip("Максимум попыток для случайного поиска точки внутри силуэта.")]
    [Range(8, 512)] public int randomOpaqueSearchAttempts = 96;

    [Header("Optional animation")]
    public bool animateHeatFade = true;
    public float heatFadeSpeed = 1.2f;
    public bool animateBurnIncrease = false;
    public float burnIncreaseSpeed = 0.25f;
    public bool randimizeObStart;

    private SpriteRenderer _renderer;
    private MaterialPropertyBlock _mpb;
    private float _noiseSeed;

    private static readonly int SpriteUVRectId = Shader.PropertyToID("_SpriteUVRect");
    private static readonly int BreachCenterId = Shader.PropertyToID("_BreachCenter");
    private static readonly int BreachRadiusId = Shader.PropertyToID("_BreachRadius");
    private static readonly int EdgeWidthId = Shader.PropertyToID("_EdgeWidth");
    private static readonly int NoiseScaleId = Shader.PropertyToID("_NoiseScale");
    private static readonly int NoiseStrengthId = Shader.PropertyToID("_NoiseStrength");
    private static readonly int NoiseSeedId = Shader.PropertyToID("_NoiseSeed");
    private static readonly int BurnAmountId = Shader.PropertyToID("_BurnAmount");
    private static readonly int HeatAmountId = Shader.PropertyToID("_HeatAmount");
    private static readonly int GlobalAlphaId = Shader.PropertyToID("_GlobalAlpha");

    /// <summary>
    /// Кэш непрозрачных UV-точек по instanceID текстуры + rect спрайта.
    /// Чтобы не перебирать пиксели заново для каждого wreck.
    /// </summary>
    private static readonly Dictionary<SpriteCacheKey, List<Vector2>> OpaqueUvCache = new();

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _mpb = new MaterialPropertyBlock();
        _noiseSeed = Random.Range(0f, 9999f);

        ApplyCurrentValues(new Vector2(0.5f, 0.5f));
    }

    private void Start() {
        RandomizeBreach();
    }

    private void Update()
    {
        bool changed = false;

        if (animateHeatFade && heatAmount > 0f)
        {
            heatAmount = Mathf.Max(0f, heatAmount - heatFadeSpeed * Time.deltaTime);
            changed = true;
        }

        if (animateBurnIncrease && burnAmount < 1f)
        {
            burnAmount = Mathf.Min(1f, burnAmount + burnIncreaseSpeed * Time.deltaTime);
            changed = true;
        }

        if (changed)
        {
            _renderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat(HeatAmountId, heatAmount);
            _mpb.SetFloat(BurnAmountId, burnAmount);
            _renderer.SetPropertyBlock(_mpb);
        }
    }

    /// <summary>
    /// Установить центр пробоины в UV координатах [0..1].
    /// </summary>
    public void SetBreachUV(Vector2 uvCenter, float radius01)
    {
        breachRadius = radius01;
        ApplyCurrentValues(uvCenter);
    }

    /// <summary>
    /// Ставит пробоину по мировой точке. Если она пришлась в пустоту,
    /// можно сдвинуть её на ближайшую/подходящую непрозрачную область.
    /// </summary>
    public void SetBreachWorld(Vector3 worldPoint, float radius01)
    {
        Vector2 uv = WorldToSpriteUV(worldPoint);

        if (snapWorldHitToOpaque && !IsOpaqueAtUV(uv))
        {
            if (TryFindNearbyOpaqueUV(uv, out Vector2 fixedUv))
                uv = fixedUv;
            else if (TryGetRandomOpaqueUV(out Vector2 randomUv))
                uv = randomUv;
        }

        SetBreachUV(uv, radius01);
    }

    /// <summary>
    /// Полностью случайная пробоина, но только по непрозрачной части спрайта.
    /// </summary>
    [ContextMenu("Randomize Breach")]
    public void RandomizeBreach()
    {
        Vector2 uv;
        if (!TryGetRandomOpaqueUV(out uv))
            uv = new Vector2(0.5f, 0.5f);

        breachRadius = Random.Range(0.12f, 0.32f);
        noiseStrength = Random.Range(0.04f, 0.11f);
        edgeWidth = Random.Range(0.02f, 0.06f);
        burnAmount = Random.Range(0.55f, 0.95f);
        heatAmount = Random.Range(0.8f, 1.6f);
        _noiseSeed = Random.Range(0f, 9999f);

        ApplyCurrentValues(uv);
    }

    /// <summary>
    /// Удобный метод для смерти "от попадания".
    /// worldHitPoint можно передать из raycast / collision / damage event.
    /// </summary>
    public void ApplyHitDrivenWreck(Vector3 worldHitPoint, float minRadius = 0.14f, float maxRadius = 0.30f)
    {
        breachRadius = Random.Range(minRadius, maxRadius);
        noiseStrength = Random.Range(0.04f, 0.11f);
        edgeWidth = Random.Range(0.02f, 0.06f);
        burnAmount = Random.Range(0.55f, 0.95f);
        heatAmount = Random.Range(0.8f, 1.6f);
        _noiseSeed = Random.Range(0f, 9999f);

        SetBreachWorld(worldHitPoint, breachRadius);
    }

    private void ApplyCurrentValues(Vector2 uvCenter)
    {
        _renderer.GetPropertyBlock(_mpb);

        _mpb.SetVector(BreachCenterId, new Vector4(uvCenter.x, uvCenter.y, 0f, 0f));
        _mpb.SetFloat(BreachRadiusId, breachRadius);
        _mpb.SetFloat(EdgeWidthId, edgeWidth);
        _mpb.SetFloat(NoiseScaleId, noiseScale);
        _mpb.SetFloat(NoiseStrengthId, noiseStrength);
        _mpb.SetFloat(NoiseSeedId, _noiseSeed);
        _mpb.SetFloat(BurnAmountId, burnAmount);
        _mpb.SetFloat(HeatAmountId, heatAmount);
        _mpb.SetFloat(GlobalAlphaId, alpha);

        ApplySpriteUVRect();

        _renderer.SetPropertyBlock(_mpb);
    }

    private void ApplySpriteUVRect()
    {
        Sprite sprite = _renderer.sprite;
        if (sprite == null || sprite.texture == null)
        {
            _mpb.SetVector(SpriteUVRectId, new Vector4(0f, 0f, 1f, 1f));
            return;
        }

        Texture2D tex = sprite.texture;

        // outer UV уже в координатах всей текстуры [0..1]
        Vector2[] uv = sprite.uv;
        if (uv == null || uv.Length == 0)
        {
            _mpb.SetVector(SpriteUVRectId, new Vector4(0f, 0f, 1f, 1f));
            return;
        }

        float minX = uv[0].x;
        float minY = uv[0].y;
        float maxX = uv[0].x;
        float maxY = uv[0].y;

        for (int i = 1; i < uv.Length; i++)
        {
            Vector2 p = uv[i];
            if (p.x < minX) minX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.x > maxX) maxX = p.x;
            if (p.y > maxY) maxY = p.y;
        }

        _mpb.SetVector(SpriteUVRectId, new Vector4(
            minX,
            minY,
            maxX - minX,
            maxY - minY
        ));
    }

    private Vector2 WorldToSpriteUV(Vector3 worldPoint)
    {
        Bounds bounds = _renderer.bounds;
        Vector3 local = worldPoint - bounds.min;

        float u = bounds.size.x > 0.0001f ? local.x / bounds.size.x : 0.5f;
        float v = bounds.size.y > 0.0001f ? local.y / bounds.size.y : 0.5f;

        return new Vector2(Mathf.Clamp01(u), Mathf.Clamp01(v));
    }

    /// <summary>
    /// Проверка прозрачности с учётом rect спрайта внутри текстуры/атласа.
    /// </summary>
    private bool IsOpaqueAtUV(Vector2 uv)
    {
        Sprite sprite = _renderer.sprite;
        if (sprite == null || sprite.texture == null)
            return false;

        Texture2D tex = sprite.texture;
        Rect rect = sprite.textureRect;

        int x = Mathf.Clamp(Mathf.FloorToInt(rect.x + uv.x * rect.width), 0, tex.width - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(rect.y + uv.y * rect.height), 0, tex.height - 1);

        try
        {
            Color c = tex.GetPixel(x, y);
            return c.a >= opaqueThreshold;
        }
        catch
        {
            Debug.LogWarning(
                $"[{nameof(WreckMaskController)}] Can't read sprite texture pixels. " +
                $"Enable Read/Write on texture: {tex.name}", this);
            return false;
        }
    }

    /// <summary>
    /// Ищем непрозрачную точку рядом с заданной UV.
    /// Это полезно, если hit-point попал в пустоту между "крыльями".
    /// </summary>
    private bool TryFindNearbyOpaqueUV(Vector2 centerUv, out Vector2 result)
    {
        Sprite sprite = _renderer.sprite;
        result = centerUv;

        if (sprite == null || sprite.texture == null)
            return false;

        // Спираль/кольца вокруг исходной точки.
        const int rings = 8;
        const int samplesPerRing = 16;
        const float maxRadius = 0.20f;

        for (int ring = 1; ring <= rings; ring++)
        {
            float r = maxRadius * (ring / (float)rings);

            for (int i = 0; i < samplesPerRing; i++)
            {
                float t = i / (float)samplesPerRing;
                float angle = t * Mathf.PI * 2f;

                Vector2 candidate = centerUv + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r;
                candidate.x = Mathf.Clamp01(candidate.x);
                candidate.y = Mathf.Clamp01(candidate.y);

                if (IsOpaqueAtUV(candidate))
                {
                    result = candidate;
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Получить случайную UV-точку только по непрозрачной части спрайта.
    /// Сначала пытаемся взять из кэша, если кэш пуст — строим его.
    /// </summary>
    public bool TryGetRandomOpaqueUV(out Vector2 uv)
    {
        uv = new Vector2(0.5f, 0.5f);

        Sprite sprite = _renderer.sprite;
        if (sprite == null || sprite.texture == null)
            return false;

        if (TryGetCachedOpaquePoints(sprite, out List<Vector2> opaquePoints) && opaquePoints.Count > 0)
        {
            uv = opaquePoints[Random.Range(0, opaquePoints.Count)];
            return true;
        }

        // Фолбэк: если кэш почему-то не собрался, просто несколько раз попробуем случайные точки.
        for (int i = 0; i < randomOpaqueSearchAttempts; i++)
        {
            Vector2 candidate = new Vector2(Random.value, Random.value);
            if (IsOpaqueAtUV(candidate))
            {
                uv = candidate;
                return true;
            }
        }

        return false;
    }

    private bool TryGetCachedOpaquePoints(Sprite sprite, out List<Vector2> opaquePoints)
    {
        opaquePoints = null;

        Texture2D tex = sprite.texture;
        if (tex == null)
            return false;

        SpriteCacheKey key = new SpriteCacheKey(sprite);

        if (OpaqueUvCache.TryGetValue(key, out opaquePoints))
            return true;

        Rect rect = sprite.textureRect;
        List<Vector2> points = new List<Vector2>(256);

        try
        {
            int xMin = Mathf.FloorToInt(rect.x);
            int yMin = Mathf.FloorToInt(rect.y);
            int width = Mathf.FloorToInt(rect.width);
            int height = Mathf.FloorToInt(rect.height);

            Color[] pixels = tex.GetPixels(xMin, yMin, width, height);

            // Чтобы не хранить вообще все пиксели 1:1, можно брать не каждый.
            // Сейчас берём сеткой, чтобы кэш не раздувался.
            int step = ComputeSamplingStep(width, height);

            for (int y = 0; y < height; y += step)
            {
                for (int x = 0; x < width; x += step)
                {
                    int idx = y * width + x;
                    if (idx < 0 || idx >= pixels.Length)
                        continue;

                    if (pixels[idx].a >= opaqueThreshold)
                    {
                        float u = width > 1 ? x / (float)(width - 1) : 0.5f;
                        float v = height > 1 ? y / (float)(height - 1) : 0.5f;
                        points.Add(new Vector2(u, v));
                    }
                }
            }

            // Если сетка слишком грубая и ничего не нашла — пройдёмся плотнее
            if (points.Count == 0)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = y * width + x;
                        if (pixels[idx].a >= opaqueThreshold)
                        {
                            float u = width > 1 ? x / (float)(width - 1) : 0.5f;
                            float v = height > 1 ? y / (float)(height - 1) : 0.5f;
                            points.Add(new Vector2(u, v));
                        }
                    }
                }
            }

            opaquePoints = points;
            OpaqueUvCache[key] = opaquePoints;
            return true;
        }
        catch
        {
            Debug.LogWarning(
                $"[{nameof(WreckMaskController)}] Failed to build opaque UV cache for sprite '{sprite.name}'. " +
                $"Make sure texture Read/Write is enabled.",
                this);

            opaquePoints = null;
            return false;
        }
    }

    private int ComputeSamplingStep(int width, int height)
    {
        int maxDim = Mathf.Max(width, height);

        if (maxDim <= 64) return 1;
        if (maxDim <= 128) return 2;
        if (maxDim <= 256) return 3;
        if (maxDim <= 512) return 4;
        return 6;
    }

    private readonly struct SpriteCacheKey
    {
        private readonly int textureId;
        private readonly int x;
        private readonly int y;
        private readonly int w;
        private readonly int h;

        public SpriteCacheKey(Sprite sprite)
        {
            textureId = sprite.texture != null ? sprite.texture.GetInstanceID() : 0;

            Rect rect = sprite.textureRect;
            x = Mathf.RoundToInt(rect.x);
            y = Mathf.RoundToInt(rect.y);
            w = Mathf.RoundToInt(rect.width);
            h = Mathf.RoundToInt(rect.height);
        }

        public override bool Equals(object obj)
        {
            return obj is SpriteCacheKey other &&
                   textureId == other.textureId &&
                   x == other.x &&
                   y == other.y &&
                   w == other.w &&
                   h == other.h;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = textureId;
                hash = hash * 397 ^ x;
                hash = hash * 397 ^ y;
                hash = hash * 397 ^ w;
                hash = hash * 397 ^ h;
                return hash;
            }
        }
    }
}