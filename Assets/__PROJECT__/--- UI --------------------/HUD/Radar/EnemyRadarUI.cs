using System.Collections.Generic;
using UnityEngine;

public class EnemyRadarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform radarRect;
    [SerializeField] private Transform iconParent;

    [Tooltip("Если не задан, попробует взять Player.PlayerTransform.")]
    [SerializeField] private Transform playerTransform;

    [Header("Icon Prefabs")]
    [SerializeField] private RectTransform enemyIconPrefab;
    [SerializeField] private RectTransform bossEnemyIconPrefab;
    [SerializeField] private RectTransform allyIconPrefab;
    [SerializeField] private RectTransform obstacleIconPrefab;
    [SerializeField] private RectTransform undefinedIconPrefab;

    [Header("Radar Scale")]
    [Tooltip("Сколько world units помещается от центра радара до края внутреннего круга.")]
    [SerializeField] private float worldRadius = 30f;

    [Tooltip("Отступ от края RectTransform, чтобы иконки не вылезали за круг.")]
    [SerializeField] private float radarEdgePaddingPixels = 8f;

    [Tooltip("Отступ дальнего кольца от края радара.")]
    [SerializeField] private float farRingPaddingPixels = 4f;

    [Header("Find")]
    [SerializeField] private float findTargetsInterval = 0.5f;
    [SerializeField] private int maxEnemies = 128;
    [SerializeField] private int maxAllies = 32;
    [SerializeField] private int maxObstacles = 128;
    [SerializeField] private int maxUndefined = 64;

    [Header("Enemy Filter")]
    [Tooltip("Если true, AsteroidBody не считается врагом, даже если он наследуется от DamageBody.")]
    [SerializeField] private bool excludeAsteroidBodies = true;

    [Tooltip("Если true, берутся только объекты ровно типа DamageBody, без наследников. Обычно лучше оставить false.")]
    [SerializeField] private bool exactDamageBodyTypeOnly = false;

    [Tooltip("Если true, Boss можно искать на родителях объекта с DamageBody.")]
    [SerializeField] private bool findBossInParent = false;

    [Header("Icon View")]
    [SerializeField] private bool rotateFarIconsToDirection = true;
    [SerializeField] private bool rotateInsideIconsToDirection = false;

    [Tooltip("Если иконка-стрелка смотрит вверх, оставь -90. Если вправо — поставь 0.")]
    [SerializeField] private float iconAngleOffset = -90f;

    [SerializeField] private Vector3 insideIconScale = Vector3.one;
    [SerializeField] private Vector3 farIconScale = Vector3.one;

    private readonly List<DamageBody> _enemies = new List<DamageBody>();
    private readonly List<Transform> _allies = new List<Transform>();
    private readonly List<Transform> _obstacles = new List<Transform>();
    private readonly List<Transform> _undefined = new List<Transform>();

    private readonly List<RectTransform> _enemyIcons = new List<RectTransform>();
    private readonly List<RectTransform> _bossEnemyIcons = new List<RectTransform>();
    private readonly List<RectTransform> _allyIcons = new List<RectTransform>();
    private readonly List<RectTransform> _obstacleIcons = new List<RectTransform>();
    private readonly List<RectTransform> _undefinedIcons = new List<RectTransform>();

    private float _findTimer;

    private void Reset()
    {
        radarRect = transform as RectTransform;
        iconParent = transform;
    }

    private void Awake()
    {
        if (radarRect == null)
            radarRect = transform as RectTransform;

        if (iconParent == null)
            iconParent = radarRect;

        ResolvePlayer();
        RefreshTargets();
    }

    private void OnEnable()
    {
        ResolvePlayer();
        RefreshTargets();
    }

    private void Update()
    {
        ResolvePlayer();

        _findTimer -= Time.deltaTime;

        if (_findTimer <= 0f)
        {
            RefreshTargets();
            _findTimer = findTargetsInterval;
        }

        UpdateIcons();
    }

    private void ResolvePlayer()
    {
        if (playerTransform != null)
            return;

        if (Player.PlayerTransform != null)
            playerTransform = Player.PlayerTransform;
    }

    private void RefreshTargets()
    {
        RefreshEnemies();
        RefreshAllies();
        RefreshObstacles();
        RefreshUndefined();
    }

    private void RefreshEnemies()
    {
        _enemies.Clear();

        DamageBody[] found = FindObjectsByType<DamageBody>(FindObjectsSortMode.None);

        for (int i = 0; i < found.Length; i++)
        {
            DamageBody body = found[i];

            if (body == null)
                continue;

            if (!body.gameObject.activeInHierarchy)
                continue;

            if (playerTransform != null && body.transform == playerTransform)
                continue;

            if (excludeAsteroidBodies && body is AsteroidBody)
                continue;

            if (exactDamageBodyTypeOnly && body.GetType() != typeof(DamageBody))
                continue;

            _enemies.Add(body);

            if (_enemies.Count >= maxEnemies)
                break;
        }
    }

    private void RefreshAllies()
    {
        _allies.Clear();

        // Твой текущий поиск союзников.
        // Потом можешь расширять этот метод как угодно.

        SledgeRepairer sledge = FindAnyObjectByType<SledgeRepairer>();

        if (sledge != null)
            _allies.Add(sledge.transform);

        PowerupFabricDrone fabricDrone = FindAnyObjectByType<PowerupFabricDrone>();

        if (fabricDrone != null)
            _allies.Add(fabricDrone.transform);

        TrimList(_allies, maxAllies);
    }

    private void RefreshObstacles()
    {
        _obstacles.Clear();

        // TODO:
        // Напиши свой поиск obstacles здесь.
        //
        // Пример:
        // ObstacleBody[] found = FindObjectsByType<ObstacleBody>(FindObjectsSortMode.None);
        // for (int i = 0; i < found.Length; i++)
        // {
        //     if (found[i] == null)
        //         continue;
        //
        //     if (!found[i].gameObject.activeInHierarchy)
        //         continue;
        //
        //     _obstacles.Add(found[i].transform);
        //
        //     if (_obstacles.Count >= maxObstacles)
        //         break;
        // }
    }

    private void RefreshUndefined()
    {
        _undefined.Clear();

        // TODO:
        // Напиши свой поиск undefined здесь.
        //
        // Например, сюда можно класть неизвестные сигнатуры,
        // квестовые точки, неопознанные объекты, временные маркеры и т.п.
        //
        // Главное — добавляй Transform:
        //
        // _undefined.Add(someObject.transform);
    }

    private void UpdateIcons()
    {
        if (radarRect == null || playerTransform == null)
        {
            HideAllIcons();
            return;
        }

        float radarRadiusPixels = GetRadarRadiusPixels();
        float farRingRadiusPixels = Mathf.Max(0f, radarRadiusPixels - farRingPaddingPixels);

        int enemyVisibleCount = 0;
        int bossVisibleCount = 0;
        int allyVisibleCount = 0;
        int obstacleVisibleCount = 0;
        int undefinedVisibleCount = 0;

        UpdateEnemyIcons(
            radarRadiusPixels,
            farRingRadiusPixels,
            ref enemyVisibleCount,
            ref bossVisibleCount
        );

        UpdateTransformLayerIcons(
            _allies,
            allyIconPrefab,
            _allyIcons,
            radarRadiusPixels,
            farRingRadiusPixels,
            ref allyVisibleCount
        );

        UpdateTransformLayerIcons(
            _obstacles,
            obstacleIconPrefab,
            _obstacleIcons,
            radarRadiusPixels,
            farRingRadiusPixels,
            ref obstacleVisibleCount
        );

        UpdateTransformLayerIcons(
            _undefined,
            undefinedIconPrefab,
            _undefinedIcons,
            radarRadiusPixels,
            farRingRadiusPixels,
            ref undefinedVisibleCount
        );

        HideUnusedIcons(_enemyIcons, enemyVisibleCount);
        HideUnusedIcons(_bossEnemyIcons, bossVisibleCount);
        HideUnusedIcons(_allyIcons, allyVisibleCount);
        HideUnusedIcons(_obstacleIcons, obstacleVisibleCount);
        HideUnusedIcons(_undefinedIcons, undefinedVisibleCount);
    }

    private void UpdateEnemyIcons(
        float radarRadiusPixels,
        float farRingRadiusPixels,
        ref int enemyVisibleCount,
        ref int bossVisibleCount)
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            DamageBody enemy = _enemies[i];

            if (enemy == null)
                continue;

            if (!enemy.gameObject.activeInHierarchy)
                continue;

            bool isBoss = IsBoss(enemy);

            RectTransform prefab = isBoss && bossEnemyIconPrefab != null
                ? bossEnemyIconPrefab
                : enemyIconPrefab;

            if (prefab == null)
                continue;

            List<RectTransform> pool = isBoss && bossEnemyIconPrefab != null
                ? _bossEnemyIcons
                : _enemyIcons;

            int index;

            if (isBoss && bossEnemyIconPrefab != null)
            {
                index = bossVisibleCount;
                bossVisibleCount++;
            }
            else
            {
                index = enemyVisibleCount;
                enemyVisibleCount++;
            }

            RectTransform icon = GetOrCreateIcon(pool, prefab, index);

            ApplyIconPosition(
                icon,
                enemy.transform.position,
                radarRadiusPixels,
                farRingRadiusPixels
            );
        }
    }

    private void UpdateTransformLayerIcons(
        List<Transform> targets,
        RectTransform prefab,
        List<RectTransform> pool,
        float radarRadiusPixels,
        float farRingRadiusPixels,
        ref int visibleCount)
    {
        if (prefab == null)
            return;

        for (int i = 0; i < targets.Count; i++)
        {
            Transform target = targets[i];

            if (target == null)
                continue;

            if (!target.gameObject.activeInHierarchy)
                continue;

            RectTransform icon = GetOrCreateIcon(pool, prefab, visibleCount);
            visibleCount++;

            ApplyIconPosition(
                icon,
                target.position,
                radarRadiusPixels,
                farRingRadiusPixels
            );
        }
    }

    private void ApplyIconPosition(
        RectTransform icon,
        Vector3 targetWorldPosition,
        float radarRadiusPixels,
        float farRingRadiusPixels)
    {
        Vector2 worldDelta = targetWorldPosition - playerTransform.position;
        float worldDistance = worldDelta.magnitude;

        Vector2 direction = worldDistance > 0.0001f
            ? worldDelta / worldDistance
            : Vector2.zero;

        bool isFar = worldDistance > worldRadius;

        float iconDistancePixels;

        if (isFar)
        {
            // Дальнее кольцо: направление учитывается, дистанция игнорируется.
            iconDistancePixels = farRingRadiusPixels;
        }
        else
        {
            // Внутренний круг: направление + дистанция в масштабе.
            float normalizedDistance = Mathf.Clamp01(worldDistance / Mathf.Max(0.0001f, worldRadius));
            iconDistancePixels = normalizedDistance * radarRadiusPixels;
        }

        icon.gameObject.SetActive(true);
        icon.anchoredPosition = direction * iconDistancePixels;
        icon.localScale = isFar ? farIconScale : insideIconScale;

        bool shouldRotate =
            isFar && rotateFarIconsToDirection ||
            !isFar && rotateInsideIconsToDirection;

        if (shouldRotate && direction.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + iconAngleOffset;
            icon.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            icon.localRotation = Quaternion.identity;
        }
    }

    private bool IsBoss(DamageBody enemy)
    {
        if (enemy == null)
            return false;

        if (enemy.GetComponent<Boss>() != null)
            return true;

        if (findBossInParent && enemy.GetComponentInParent<Boss>() != null)
            return true;

        return false;
    }

    private RectTransform GetOrCreateIcon(
        List<RectTransform> pool,
        RectTransform prefab,
        int index)
    {
        while (pool.Count <= index)
        {
            RectTransform icon = Instantiate(
                prefab,
                iconParent != null ? iconParent : radarRect
            );

            SetupIconTransform(icon);
            icon.gameObject.SetActive(false);

            pool.Add(icon);
        }

        return pool[index];
    }

    private void SetupIconTransform(RectTransform icon)
    {
        icon.anchorMin = new Vector2(0.5f, 0.5f);
        icon.anchorMax = new Vector2(0.5f, 0.5f);
        icon.pivot = new Vector2(0.5f, 0.5f);
        icon.anchoredPosition = Vector2.zero;
        icon.localRotation = Quaternion.identity;
        icon.localScale = Vector3.one;
    }

    private float GetRadarRadiusPixels()
    {
        Rect rect = radarRect.rect;

        float minSize = Mathf.Min(rect.width, rect.height);
        float radius = minSize * 0.5f - radarEdgePaddingPixels;

        return Mathf.Max(0f, radius);
    }

    private void HideUnusedIcons(List<RectTransform> pool, int fromIndex)
    {
        for (int i = fromIndex; i < pool.Count; i++)
        {
            if (pool[i] != null)
                pool[i].gameObject.SetActive(false);
        }
    }

    private void HideAllIcons()
    {
        HideUnusedIcons(_enemyIcons, 0);
        HideUnusedIcons(_bossEnemyIcons, 0);
        HideUnusedIcons(_allyIcons, 0);
        HideUnusedIcons(_obstacleIcons, 0);
        HideUnusedIcons(_undefinedIcons, 0);
    }

    private void TrimList<T>(List<T> list, int maxCount)
    {
        if (maxCount < 0)
            return;

        if (list.Count <= maxCount)
            return;

        list.RemoveRange(maxCount, list.Count - maxCount);
    }
}