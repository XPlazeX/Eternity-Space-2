using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class WreckMaskController : MonoBehaviour
{
    [Header("Breach")]
    public Vector2 breachCenterUV = new Vector2(0.5f, 0.5f);

    [Tooltip("Радиус бреши в пикселях текущего нарезанного спрайта.")]
    [Range(0.5f, 128f)] public float breachRadiusPixels = 6f;

    [Tooltip("Ширина обожжённого края в пикселях спрайта.")]
    [Range(0.1f, 32f)] public float edgeWidthPixels = 1.5f;

    [Header("Noise in sprite pixels")]
    [Tooltip("Размер пятен шума в пикселях. Чем больше, тем крупнее форма.")]
    [Range(0.5f, 128f)] public float noiseScalePixels = 8f;

    [Tooltip("Сила локального шума в пикселях.")]
    [Range(0f, 32f)] public float noiseStrengthPixels = 2f;

    [Tooltip("Сила шума по углу в пикселях. Именно он ломает круг.")]
    [Range(0f, 32f)] public float angularNoiseStrengthPixels = 3f;

    [Range(0.1f, 32f)] public float angularNoiseScale = 6f;

    [Header("Warp in sprite pixels")]
    [Range(0f, 32f)] public float warpStrengthPixels = 1f;
    [Range(0.5f, 128f)] public float warpScalePixels = 10f;

    [Header("Visual")]
    [Range(0f, 1f)] public float burnAmount = 0.75f;
    [Range(0f, 3f)] public float heatAmount = 1.2f;
    [Range(0f, 1f)] public float ashDarkness = 0.15f;
    [Range(0f, 1f)] public float alpha = 1f;

    [Header("Random")]
    public bool randomizeOnStart = true;

    [Range(0.5f, 128f)] public float randomMinRadiusPixels = 4f;
    [Range(0.5f, 128f)] public float randomMaxRadiusPixels = 10f;

    [Range(0f, 32f)] public float randomMinNoiseStrengthPixels = 1f;
    [Range(0f, 32f)] public float randomMaxNoiseStrengthPixels = 3f;

    [Range(0.1f, 32f)] public float randomMinEdgeWidthPixels = 1f;
    [Range(0.1f, 32f)] public float randomMaxEdgeWidthPixels = 2f;

    [Range(0, 1f)] public float randomMinAshDarkness = 0.3f;
    [Range(0, 1f)] public float randomMaxAshDarkness = 0.5f;

    [Header("Alpha sampling")]
    [Range(0.01f, 1f)] public float opaqueThreshold = 0.15f;

    [Tooltip("Если world-hit попал в прозрачный пиксель, пробуем найти рядом непрозрачный.")]
    public bool snapWorldHitToOpaque = true;

    [Tooltip("Радиус поиска непрозрачного пикселя рядом с попаданием. В пикселях спрайта.")]
    [Range(1f, 64f)] public float snapSearchRadiusPixels = 8f;

    [Tooltip("Максимум попыток для случайного поиска точки внутри силуэта.")]
    [Range(8, 512)] public int randomOpaqueSearchAttempts = 96;

    [Header("Optional animation")]
    public bool animateHeatFade = true;
    public float heatFadeSpeed = 1.2f;

    public bool animateBurnIncrease = false;
    public float burnIncreaseSpeed = 0.25f;

    [Header("Safety")]
    [Tooltip("Минимальный радиус видимой бреши после всех шумов.")]
    [Range(0.5f, 32f)]
    public float minVisibleRadiusPixels = 2f;

    [Tooltip("Не даёт шуму съесть весь радиус бреши.")]
    public bool clampNoiseToRadius = true;

    [Tooltip("Сколько от радиуса максимум может занимать warp.")]
    [Range(0f, 1f)]
    public float maxWarpPartOfRadius = 0.35f;

    [Header("Editor preview")]
    public bool drawScenePreview = true;
    public bool previewOnlyOpaquePixels = true;

    [Tooltip("Ограничение количества preview-клеток, чтобы не убить Scene View на больших спрайтах.")]
    [Range(64, 8192)] public int previewMaxCells = 2048;

    public Color previewHoleColor = new Color(1f, 0.05f, 0f, 0.25f);
    public Color previewEdgeColor = new Color(1f, 0.75f, 0f, 0.75f);

    [Header("Seed")]
    public float noiseSeed = 0f;

    private SpriteRenderer _renderer;
    private MaterialPropertyBlock _mpb;
    public float BreachRadiusPixels
    {
        get => breachRadiusPixels;
        set => SetBreachRadiusPixels(value);
    }

    // Alias, чтобы можно было писать короче.
    public float BreachRadius
    {
        get => breachRadiusPixels;
        set => SetBreachRadiusPixels(value);
    }

    private static readonly int SpriteUVRectId = Shader.PropertyToID("_SpriteUVRect");
    private static readonly int SpriteSizePixelsId = Shader.PropertyToID("_SpriteSizePixels");

    private static readonly int BreachCenterId = Shader.PropertyToID("_BreachCenter");
    private static readonly int BreachRadiusPixelsId = Shader.PropertyToID("_BreachRadiusPixels");
    private static readonly int EdgeWidthPixelsId = Shader.PropertyToID("_EdgeWidthPixels");

    private static readonly int NoiseScalePixelsId = Shader.PropertyToID("_NoiseScalePixels");
    private static readonly int NoiseStrengthPixelsId = Shader.PropertyToID("_NoiseStrengthPixels");
    private static readonly int NoiseSeedId = Shader.PropertyToID("_NoiseSeed");

    private static readonly int AngularNoiseStrengthPixelsId = Shader.PropertyToID("_AngularNoiseStrengthPixels");
    private static readonly int AngularNoiseScaleId = Shader.PropertyToID("_AngularNoiseScale");

    private static readonly int WarpStrengthPixelsId = Shader.PropertyToID("_WarpStrengthPixels");
    private static readonly int WarpScalePixelsId = Shader.PropertyToID("_WarpScalePixels");
    private static readonly int MinVisibleRadiusPixelsId = Shader.PropertyToID("_MinVisibleRadiusPixels");

    private static readonly int BurnAmountId = Shader.PropertyToID("_BurnAmount");
    private static readonly int HeatAmountId = Shader.PropertyToID("_HeatAmount");
    private static readonly int AshDarknessId = Shader.PropertyToID("_AshDarkness");
    private static readonly int GlobalAlphaId = Shader.PropertyToID("_GlobalAlpha");

    private static readonly Dictionary<SpriteCacheKey, List<Vector2>> OpaqueUvCache =
        new Dictionary<SpriteCacheKey, List<Vector2>>();

    private void Reset()
    {
        Ensure();
        noiseSeed = Random.Range(0f, 9999f);
        ApplyCurrentValues();
    }

    private void Awake()
    {
        Ensure();

        if (Application.isPlaying && Mathf.Approximately(noiseSeed, 0f))
            noiseSeed = Random.Range(0f, 9999f);

        ApplyCurrentValues();
    }

    private void OnEnable()
    {
        Ensure();
        ApplyCurrentValues();
    }

    private void Start()
    {
        if (Application.isPlaying && randomizeOnStart)
            RandomizeBreach();
        else
            ApplyCurrentValues();
    }

    public void SetBreachRadiusPixels(float radiusPixels)
    {
        breachRadiusPixels = Mathf.Max(0.01f, radiusPixels);
        ApplyCurrentValues();
    }

    public void AddBreachRadiusPixels(float deltaPixels)
    {
        SetBreachRadiusPixels(breachRadiusPixels + deltaPixels);
    }

    public void SetBreachRadius01(float radius01)
    {
        Vector2 size = GetCurrentSpriteSizePixels();
        float minSize = Mathf.Max(1f, Mathf.Min(size.x, size.y));

        SetBreachRadiusPixels(radius01 * minSize);
    }

    private void OnValidate()
    {
        Ensure();
        ClampValues();
        ApplyCurrentValues();
    }

    private void Update()
    {
        Ensure();

        if (!Application.isPlaying)
        {
            ApplyCurrentValues();
            return;
        }

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
            ApplyCurrentValues();
    }

    public void SetBreachUV(Vector2 uvCenter, float radiusPixels)
    {
        breachCenterUV = new Vector2(Mathf.Clamp01(uvCenter.x), Mathf.Clamp01(uvCenter.y));
        breachRadiusPixels = Mathf.Max(0.01f, radiusPixels);

        ApplyCurrentValues();
    }

    public void SetBreachWorld(Vector3 worldPoint, float radiusPixels)
    {
        Vector2 uv = WorldToSpriteUV(worldPoint);

        if (snapWorldHitToOpaque && !IsOpaqueAtUV(uv))
        {
            if (TryFindNearbyOpaqueUV(uv, out Vector2 fixedUv))
                uv = fixedUv;
            else if (TryGetRandomOpaqueUV(out Vector2 randomUv))
                uv = randomUv;
        }

        SetBreachUV(uv, radiusPixels);
    }

    /// <summary>
    /// Если где-то в старом коде ещё удобно передавать радиус как 0..1 от размера спрайта.
    /// </summary>
    public void SetBreachWorld01(Vector3 worldPoint, float radius01)
    {
        Vector2 size = GetCurrentSpriteSizePixels();
        float minSize = Mathf.Max(1f, Mathf.Min(size.x, size.y));
        SetBreachWorld(worldPoint, radius01 * minSize);
    }

    /// <summary>
    /// Если где-то в старом коде ещё удобно передавать радиус как 0..1 от размера спрайта.
    /// </summary>
    public void SetBreachUV01(Vector2 uvCenter, float radius01)
    {
        Vector2 size = GetCurrentSpriteSizePixels();
        float minSize = Mathf.Max(1f, Mathf.Min(size.x, size.y));
        SetBreachUV(uvCenter, radius01 * minSize);
    }

    [ContextMenu("Randomize Breach")]
    public Vector3 RandomizeBreach()
    {
        Ensure();

        Vector2 uv;

        if (!TryGetRandomOpaqueUV(out uv))
            uv = new Vector2(0.5f, 0.5f);

        breachCenterUV = uv;

        breachRadiusPixels = Random.Range(
            Mathf.Min(randomMinRadiusPixels, randomMaxRadiusPixels),
            Mathf.Max(randomMinRadiusPixels, randomMaxRadiusPixels)
        );

        noiseStrengthPixels = Random.Range(
            Mathf.Min(randomMinNoiseStrengthPixels, randomMaxNoiseStrengthPixels),
            Mathf.Max(randomMinNoiseStrengthPixels, randomMaxNoiseStrengthPixels)
        );

        edgeWidthPixels = Random.Range(
            Mathf.Min(randomMinEdgeWidthPixels, randomMaxEdgeWidthPixels),
            Mathf.Max(randomMinEdgeWidthPixels, randomMaxEdgeWidthPixels)
        );

        ashDarkness = Random.Range(
            Mathf.Min(randomMinAshDarkness, randomMaxAshDarkness),
            Mathf.Max(randomMinAshDarkness, randomMaxAshDarkness)
        );

        angularNoiseStrengthPixels = Random.Range(1f, 4f);
        warpStrengthPixels = Random.Range(0.25f, 2f);

        burnAmount = Random.Range(0.55f, 0.95f);
        heatAmount = Random.Range(0.8f, 1.6f);
        noiseSeed = Random.Range(0f, 9999f);

        ClampValues();

        if (clampNoiseToRadius)
            ClampNoiseSoBreachStaysVisible();

        ApplyCurrentValues();

        return SpriteUVToWorldPoint(breachCenterUV);
    }

    public Vector3 SpriteUVToWorldPoint(Vector2 uv)
    {
        Ensure();

        if (_renderer == null || _renderer.sprite == null)
            return transform.position;

        Sprite sprite = _renderer.sprite;
        Bounds localBounds = sprite.bounds;

        float u = Mathf.Clamp01(uv.x);
        float v = Mathf.Clamp01(uv.y);

        // В shader-е breachCenterUV хранится в логике спрайта.
        // А для позиции в мире надо учесть визуальный flip SpriteRenderer-а.
        if (_renderer.flipX)
            u = 1f - u;

        if (_renderer.flipY)
            v = 1f - v;

        Vector3 localPoint = new Vector3(
            Mathf.Lerp(localBounds.min.x, localBounds.max.x, u),
            Mathf.Lerp(localBounds.min.y, localBounds.max.y, v),
            0f
        );

        return transform.TransformPoint(localPoint);
    }

    public void ApplyHitDrivenWreck(
        Vector3 worldHitPoint,
        float minRadiusPixels = 4f,
        float maxRadiusPixels = 10f)
    {
        breachRadiusPixels = Random.Range(
            Mathf.Min(minRadiusPixels, maxRadiusPixels),
            Mathf.Max(minRadiusPixels, maxRadiusPixels)
        );

        noiseStrengthPixels = Random.Range(
            Mathf.Min(randomMinNoiseStrengthPixels, randomMaxNoiseStrengthPixels),
            Mathf.Max(randomMinNoiseStrengthPixels, randomMaxNoiseStrengthPixels)
        );

        edgeWidthPixels = Random.Range(
            Mathf.Min(randomMinEdgeWidthPixels, randomMaxEdgeWidthPixels),
            Mathf.Max(randomMinEdgeWidthPixels, randomMaxEdgeWidthPixels)
        );

        angularNoiseStrengthPixels = Random.Range(1f, 4f);
        warpStrengthPixels = Random.Range(0.25f, 2f);

        burnAmount = Random.Range(0.55f, 0.95f);
        heatAmount = Random.Range(0.8f, 1.6f);
        noiseSeed = Random.Range(0f, 9999f);

        SetBreachWorld(worldHitPoint, breachRadiusPixels);
    }

    private void ApplyCurrentValues()
    {
        if (_renderer == null)
            return;

        if (_mpb == null)
            _mpb = new MaterialPropertyBlock();

        ClampValues();

        if (clampNoiseToRadius)
            ClampNoiseSoBreachStaysVisible();

        _renderer.GetPropertyBlock(_mpb);

        _mpb.SetVector(BreachCenterId, new Vector4(breachCenterUV.x, breachCenterUV.y, 0f, 0f));

        _mpb.SetFloat(BreachRadiusPixelsId, breachRadiusPixels);
        _mpb.SetFloat(EdgeWidthPixelsId, edgeWidthPixels);

        _mpb.SetFloat(NoiseScalePixelsId, noiseScalePixels);
        _mpb.SetFloat(NoiseStrengthPixelsId, noiseStrengthPixels);
        _mpb.SetFloat(NoiseSeedId, noiseSeed);

        _mpb.SetFloat(AngularNoiseStrengthPixelsId, angularNoiseStrengthPixels);
        _mpb.SetFloat(AngularNoiseScaleId, angularNoiseScale);

        _mpb.SetFloat(WarpStrengthPixelsId, warpStrengthPixels);
        _mpb.SetFloat(WarpScalePixelsId, warpScalePixels);
        _mpb.SetFloat(MinVisibleRadiusPixelsId, minVisibleRadiusPixels);

        _mpb.SetFloat(BurnAmountId, burnAmount);
        _mpb.SetFloat(HeatAmountId, heatAmount);
        _mpb.SetFloat(AshDarknessId, ashDarkness);
        _mpb.SetFloat(GlobalAlphaId, alpha);

        ApplySpriteDataToMaterialPropertyBlock();

        _renderer.SetPropertyBlock(_mpb);
    }

    private void ClampNoiseSoBreachStaysVisible()
    {
        minVisibleRadiusPixels = Mathf.Max(0.5f, minVisibleRadiusPixels);

        // Минимальный радиус не должен быть больше основного радиуса.
        minVisibleRadiusPixels = Mathf.Min(minVisibleRadiusPixels, breachRadiusPixels);

        float maxNoiseBudget = Mathf.Max(0f, breachRadiusPixels - minVisibleRadiusPixels);

        float currentNoiseBudget =
            Mathf.Abs(noiseStrengthPixels) +
            Mathf.Abs(angularNoiseStrengthPixels);

        if (currentNoiseBudget > maxNoiseBudget && currentNoiseBudget > 0.0001f)
        {
            float scale = maxNoiseBudget / currentNoiseBudget;

            noiseStrengthPixels *= scale;
            angularNoiseStrengthPixels *= scale;
        }

        // Warp тоже может визуально утащить форму, особенно на маленьких спрайтах.
        float maxWarp = breachRadiusPixels * maxWarpPartOfRadius;
        warpStrengthPixels = Mathf.Min(warpStrengthPixels, maxWarp);
    }

    private void ApplySpriteDataToMaterialPropertyBlock()
    {
        Sprite sprite = _renderer.sprite;

        if (sprite == null || sprite.texture == null)
        {
            _mpb.SetVector(SpriteUVRectId, new Vector4(0f, 0f, 1f, 1f));
            _mpb.SetVector(SpriteSizePixelsId, new Vector4(32f, 32f, 0f, 0f));
            return;
        }

        Texture2D texture = sprite.texture;
        Rect rect = GetSpriteTextureRect(sprite);

        Vector4 uvRect = new Vector4(
            rect.x / texture.width,
            rect.y / texture.height,
            rect.width / texture.width,
            rect.height / texture.height
        );

        Vector4 sizePixels = new Vector4(
            rect.width,
            rect.height,
            0f,
            0f
        );

        _mpb.SetVector(SpriteUVRectId, uvRect);
        _mpb.SetVector(SpriteSizePixelsId, sizePixels);
    }

    private Vector2 WorldToSpriteUV(Vector3 worldPoint)
    {
        Sprite sprite = _renderer.sprite;

        if (sprite == null)
            return new Vector2(0.5f, 0.5f);

        Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
        Bounds localBounds = sprite.bounds;

        float u = localBounds.size.x > 0.0001f
            ? (localPoint.x - localBounds.min.x) / localBounds.size.x
            : 0.5f;

        float v = localBounds.size.y > 0.0001f
            ? (localPoint.y - localBounds.min.y) / localBounds.size.y
            : 0.5f;

        if (_renderer.flipX)
            u = 1f - u;

        if (_renderer.flipY)
            v = 1f - v;

        return new Vector2(Mathf.Clamp01(u), Mathf.Clamp01(v));
    }

    private bool IsOpaqueAtUV(Vector2 uv)
    {
        return IsOpaqueAtUVInternal(uv, true);
    }

    private bool IsOpaqueAtUVInternal(Vector2 uv, bool logWarning)
    {
        Sprite sprite = _renderer.sprite;

        if (sprite == null || sprite.texture == null)
            return false;

        Texture2D texture = sprite.texture;
        Rect rect = GetSpriteTextureRect(sprite);

        int x = Mathf.Clamp(
            Mathf.FloorToInt(rect.x + uv.x * rect.width),
            0,
            texture.width - 1
        );

        int y = Mathf.Clamp(
            Mathf.FloorToInt(rect.y + uv.y * rect.height),
            0,
            texture.height - 1
        );

        try
        {
            Color color = texture.GetPixel(x, y);
            return color.a >= opaqueThreshold;
        }
        catch
        {
            if (logWarning)
            {
                Debug.LogWarning(
                    $"[{nameof(WreckMaskController)}] Can't read sprite texture pixels. " +
                    $"Enable Read/Write on texture: {texture.name}",
                    this
                );
            }

            return false;
        }
    }

    private bool TryFindNearbyOpaqueUV(Vector2 centerUv, out Vector2 result)
    {
        result = centerUv;

        Sprite sprite = _renderer.sprite;

        if (sprite == null || sprite.texture == null)
            return false;

        Vector2 size = GetCurrentSpriteSizePixels();

        if (size.x <= 0f || size.y <= 0f)
            return false;

        const int rings = 8;
        const int samplesPerRing = 16;

        for (int ring = 1; ring <= rings; ring++)
        {
            float radiusPixels = snapSearchRadiusPixels * (ring / (float)rings);

            for (int i = 0; i < samplesPerRing; i++)
            {
                float t = i / (float)samplesPerRing;
                float angle = t * Mathf.PI * 2f;

                Vector2 pixelOffset = new Vector2(
                    Mathf.Cos(angle),
                    Mathf.Sin(angle)
                ) * radiusPixels;

                Vector2 uvOffset = new Vector2(
                    pixelOffset.x / size.x,
                    pixelOffset.y / size.y
                );

                Vector2 candidate = centerUv + uvOffset;
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

        Texture2D texture = sprite.texture;

        if (texture == null)
            return false;

        SpriteCacheKey key = new SpriteCacheKey(sprite);

        if (OpaqueUvCache.TryGetValue(key, out opaquePoints))
            return true;

        Rect rect = GetSpriteTextureRect(sprite);
        List<Vector2> points = new List<Vector2>(256);

        try
        {
            int xMin = Mathf.FloorToInt(rect.x);
            int yMin = Mathf.FloorToInt(rect.y);
            int width = Mathf.FloorToInt(rect.width);
            int height = Mathf.FloorToInt(rect.height);

            Color[] pixels = texture.GetPixels(xMin, yMin, width, height);

            int step = ComputeSamplingStep(width, height);

            for (int y = 0; y < height; y += step)
            {
                for (int x = 0; x < width; x += step)
                {
                    int index = y * width + x;

                    if (index < 0 || index >= pixels.Length)
                        continue;

                    if (pixels[index].a >= opaqueThreshold)
                    {
                        float u = width > 1 ? x / (float)(width - 1) : 0.5f;
                        float v = height > 1 ? y / (float)(height - 1) : 0.5f;

                        points.Add(new Vector2(u, v));
                    }
                }
            }

            if (points.Count == 0)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = y * width + x;

                        if (index < 0 || index >= pixels.Length)
                            continue;

                        if (pixels[index].a >= opaqueThreshold)
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
                this
            );

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

    private void OnDrawGizmosSelected()
    {
        if (!drawScenePreview)
            return;

        Ensure();

        if (_renderer == null || _renderer.sprite == null)
            return;

        Sprite sprite = _renderer.sprite;
        Rect rect = GetSpriteTextureRect(sprite);

        int width = Mathf.Max(1, Mathf.RoundToInt(rect.width));
        int height = Mathf.Max(1, Mathf.RoundToInt(rect.height));

        int totalPixels = width * height;
        int step = Mathf.Max(1, Mathf.CeilToInt(Mathf.Sqrt(totalPixels / (float)Mathf.Max(1, previewMaxCells))));

        Bounds localBounds = sprite.bounds;

        float cellWidth = localBounds.size.x / width * step;
        float cellHeight = localBounds.size.y / height * step;

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        for (int y = 0; y < height; y += step)
        {
            for (int x = 0; x < width; x += step)
            {
                float pixelX = x + 0.5f;
                float pixelY = y + 0.5f;

                Vector2 uv = new Vector2(pixelX / width, pixelY / height);

                if (previewOnlyOpaquePixels && !IsOpaqueAtUVInternal(uv, false))
                    continue;

                float sdf = ComputeSdfPixels(new Vector2(pixelX, pixelY));

                bool isHole = sdf < 0f;
                bool isEdge = !isHole && sdf <= edgeWidthPixels;

                if (!isHole && !isEdge)
                    continue;

                float drawU = uv.x;
                float drawV = uv.y;

                if (_renderer.flipX)
                    drawU = 1f - drawU;

                if (_renderer.flipY)
                    drawV = 1f - drawV;

                Vector3 localPos = new Vector3(
                    Mathf.Lerp(localBounds.min.x, localBounds.max.x, drawU),
                    Mathf.Lerp(localBounds.min.y, localBounds.max.y, drawV),
                    0f
                );

                Gizmos.color = isHole ? previewHoleColor : previewEdgeColor;

                Gizmos.DrawCube(
                    localPos,
                    new Vector3(cellWidth, cellHeight, 0.001f)
                );
            }
        }

        Gizmos.matrix = oldMatrix;
    }

    private float ComputeSdfPixels(Vector2 spritePixel)
    {
        Vector2 size = GetCurrentSpriteSizePixels();

        Vector2 centerPixel = new Vector2(
            breachCenterUV.x * size.x,
            breachCenterUV.y * size.y
        );

        Vector2 warpedPixel = DomainWarpPixels(
            spritePixel,
            warpScalePixels,
            warpStrengthPixels
        );

        Vector2 toCenter = warpedPixel - centerPixel;
        float distPixels = toCenter.magnitude;

        float angle = Mathf.Atan2(toCenter.y, toCenter.x);
        float angle01 = angle / (Mathf.PI * 2f) + 0.5f;

        float angularNoise = Fbm(new Vector2(
            angle01 * angularNoiseScale,
            noiseSeed * 0.017f
        ));

        angularNoise = angularNoise * 2f - 1f;

        float safeNoiseScale = Mathf.Max(noiseScalePixels, 0.0001f);

        float localNoise = Fbm(
            spritePixel / safeNoiseScale + Vector2.one * (noiseSeed * 0.013f)
        );

        localNoise = localNoise * 2f - 1f;

        float noisyRadiusPixels =
            breachRadiusPixels
            + angularNoise * angularNoiseStrengthPixels
            + localNoise * noiseStrengthPixels;

        return distPixels - noisyRadiusPixels;
    }

    private Vector2 DomainWarpPixels(Vector2 pixelPos, float scalePixels, float strengthPixels)
    {
        float safeScale = Mathf.Max(scalePixels, 0.0001f);
        Vector2 noiseUV = pixelPos / safeScale;

        Vector2 q = new Vector2(
            Fbm(noiseUV + new Vector2(3.1f, 7.2f)),
            Fbm(noiseUV + new Vector2(8.3f, 2.8f))
        );

        q = q * 2f - Vector2.one;

        return pixelPos + q * strengthPixels;
    }

    private float Fbm(Vector2 uv)
    {
        float value = 0f;
        float amplitude = 0.5f;

        value += ValueNoise(uv) * amplitude;
        uv *= 2.03f;
        amplitude *= 0.5f;

        value += ValueNoise(uv) * amplitude;
        uv *= 2.01f;
        amplitude *= 0.5f;

        value += ValueNoise(uv) * amplitude;
        uv *= 2.02f;
        amplitude *= 0.5f;

        value += ValueNoise(uv) * amplitude;

        return value;
    }

    private float ValueNoise(Vector2 uv)
    {
        Vector2 i = new Vector2(Mathf.Floor(uv.x), Mathf.Floor(uv.y));
        Vector2 f = new Vector2(Frac(uv.x), Frac(uv.y));

        float a = Hash21(i);
        float b = Hash21(i + new Vector2(1f, 0f));
        float c = Hash21(i + new Vector2(0f, 1f));
        float d = Hash21(i + new Vector2(1f, 1f));

        float ux = f.x * f.x * (3f - 2f * f.x);
        float uy = f.y * f.y * (3f - 2f * f.y);

        return Mathf.Lerp(
            Mathf.Lerp(a, b, ux),
            Mathf.Lerp(c, d, ux),
            uy
        );
    }

    private float Hash21(Vector2 p)
    {
        p = new Vector2(
            Frac(p.x * 123.34f),
            Frac(p.y * 456.21f)
        );

        float d = Vector2.Dot(
            p,
            p + Vector2.one * (45.32f + noiseSeed)
        );

        p += Vector2.one * d;

        return Frac(p.x * p.y);
    }

    private float Frac(float value)
    {
        return value - Mathf.Floor(value);
    }

    private void Ensure()
    {
        if (_renderer == null)
            _renderer = GetComponent<SpriteRenderer>();

        if (_mpb == null)
            _mpb = new MaterialPropertyBlock();
    }

    private void ClampValues()
    {
        breachCenterUV.x = Mathf.Clamp01(breachCenterUV.x);
        breachCenterUV.y = Mathf.Clamp01(breachCenterUV.y);

        breachRadiusPixels = Mathf.Max(0.01f, breachRadiusPixels);
        edgeWidthPixels = Mathf.Max(0.01f, edgeWidthPixels);

        noiseScalePixels = Mathf.Max(0.0001f, noiseScalePixels);
        angularNoiseScale = Mathf.Max(0.0001f, angularNoiseScale);
        warpScalePixels = Mathf.Max(0.0001f, warpScalePixels);

        alpha = Mathf.Clamp01(alpha);
        burnAmount = Mathf.Clamp01(burnAmount);
        ashDarkness = Mathf.Clamp01(ashDarkness);
        heatAmount = Mathf.Max(0f, heatAmount);
    }

    private Vector2 GetCurrentSpriteSizePixels()
    {
        if (_renderer == null || _renderer.sprite == null)
            return new Vector2(32f, 32f);

        Rect rect = GetSpriteTextureRect(_renderer.sprite);

        return new Vector2(
            Mathf.Max(1f, rect.width),
            Mathf.Max(1f, rect.height)
        );
    }

    private Rect GetSpriteTextureRect(Sprite sprite)
    {
        if (sprite == null)
            return new Rect(0f, 0f, 32f, 32f);

        try
        {
            return sprite.textureRect;
        }
        catch
        {
            return sprite.rect;
        }
    }

    private struct SpriteCacheKey
    {
        private readonly int textureId;
        private readonly int x;
        private readonly int y;
        private readonly int w;
        private readonly int h;

        public SpriteCacheKey(Sprite sprite)
        {
            textureId = sprite.texture != null ? sprite.texture.GetInstanceID() : 0;

            Rect rect;

            try
            {
                rect = sprite.textureRect;
            }
            catch
            {
                rect = sprite.rect;
            }

            x = Mathf.RoundToInt(rect.x);
            y = Mathf.RoundToInt(rect.y);
            w = Mathf.RoundToInt(rect.width);
            h = Mathf.RoundToInt(rect.height);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SpriteCacheKey))
                return false;

            SpriteCacheKey other = (SpriteCacheKey)obj;

            return textureId == other.textureId &&
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