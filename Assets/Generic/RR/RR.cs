using System;
using System.Collections.Generic;
using UnityEngine;

public static class RR
{
    public enum RoundMode
    {
        Floor = -1,
        Round = 0,
        Ceil = 1
    }

    private struct TimedModifier
    {
        public int id;
        public RuntimeStat stat;
        public ModifierMode mode;
        public float value;
        public float remainingTime;
        public int order;
    }

    public enum ModifierMode
    {
        Add,
        Multiply,
        Override
    }

    private static bool _initialized;

    private static float[] _baseValues;
    private static float[] _computedValues;
    private static RuntimeStatGroup[] _groups;

    private static readonly List<TimedModifier> _timedModifiers = new();
    private static int _nextModifierId = 1;

    public static RuntimeValues RuntimeValue { get; } = new RuntimeValues();

    public static bool IsInitialized => _initialized;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void DomainReset()
    {
        _initialized = false;
        _baseValues = null;
        _computedValues = null;
        _groups = null;
        _timedModifiers.Clear();
        _nextModifierId = 1;
    }

    public static void Initialize(RuntimeStatDefaultsAsset defaultsAsset)
    {
        int count = Enum.GetValues(typeof(RuntimeStat)).Length;

        _baseValues = new float[count];
        _computedValues = new float[count];
        _groups = new RuntimeStatGroup[count];

        for (int i = 0; i < count; i++)
        {
            _baseValues[i] = 1f;
            _computedValues[i] = 1f;
            _groups[i] = RuntimeStatGroup.Player;
        }

        if (defaultsAsset != null)
        {
            foreach (var entry in defaultsAsset.Entries)
            {
                int index = (int)entry.stat;
                _baseValues[index] = entry.value;
                _groups[index] = entry.group;
            }
        }
        else
        {
            SetHardcodedFallbackDefaults();
        }

        Array.Copy(_baseValues, _computedValues, count);
        _timedModifiers.Clear();
        _nextModifierId = 1;
        _initialized = true;
    }

    private static void SetHardcodedFallbackDefaults()
    {
        SetFallback(RuntimeStat.MainWeaponFirerateMultiplier, 1f, RuntimeStatGroup.Player);
        SetFallback(RuntimeStat.TakingDamageMultiplier, 1f, RuntimeStatGroup.Player);
        SetFallback(RuntimeStat.TimeSlowMultiplier, 1f, RuntimeStatGroup.Player);
        SetFallback(RuntimeStat.CriticalTimeSlowMultiplier, 0.33f, RuntimeStatGroup.Player);
        SetFallback(RuntimeStat.PrepareTimeMultiplier, 1f, RuntimeStatGroup.Player);

        SetFallback(RuntimeStat.RamHealValue, 0f, RuntimeStatGroup.Ram);
        SetFallback(RuntimeStat.RamFirerateBoost, 0.5f, RuntimeStatGroup.Ram);
        SetFallback(RuntimeStat.RamBoostDuration, 0.5f, RuntimeStatGroup.Ram);

        SetFallback(RuntimeStat.SpreadMultiplier, 1f, RuntimeStatGroup.Weapon);
        SetFallback(RuntimeStat.ExplosionDamageMultiplier, 1f, RuntimeStatGroup.Weapon);
        SetFallback(RuntimeStat.ExplosionAreaMultiplier, 1f, RuntimeStatGroup.Weapon);
        SetFallback(RuntimeStat.PlayerShotSpeedMultiplier, 1f, RuntimeStatGroup.Weapon);
        SetFallback(RuntimeStat.DeviceDamageMultiplier, 1f, RuntimeStatGroup.Weapon);

        SetFallback(RuntimeStat.EnemyHealthMultiplier, 1f, RuntimeStatGroup.Enemy);
        SetFallback(RuntimeStat.EnemyBulletSpeedMultiplier, 1f, RuntimeStatGroup.Enemy);
        SetFallback(RuntimeStat.EnemyAggresionMultiplier, 1f, RuntimeStatGroup.Enemy);
        SetFallback(RuntimeStat.EnemyMobilityMultiplier, 1f, RuntimeStatGroup.Enemy);

        SetFallback(RuntimeStat.AsteroidSpawnRateMultiplier, 1f, RuntimeStatGroup.Environment);
        SetFallback(RuntimeStat.AsteroidSpeedMultiplier, 1f, RuntimeStatGroup.Environment);

        SetFallback(RuntimeStat.TimeBetweenWaves, 4f, RuntimeStatGroup.Waves);
        SetFallback(RuntimeStat.EnemySpawnReloadMultiplier, 0.45f, RuntimeStatGroup.Waves);
    }

    private static void SetFallback(RuntimeStat stat, float value, RuntimeStatGroup group)
    {
        int index = (int)stat;
        _baseValues[index] = value;
        _groups[index] = group;
    }

    private static void EnsureInitialized()
    {
        if (_initialized)
            return;

        Initialize(null);
    }

    public static float Get(RuntimeStat stat)
    {
        EnsureInitialized();
        return _computedValues[(int)stat];
    }

    public static int GetInt(RuntimeStat stat, RoundMode roundMode = RoundMode.Ceil)
    {
        float value = Get(stat);
        return roundMode switch
        {
            RoundMode.Floor => Mathf.FloorToInt(value),
            RoundMode.Round => Mathf.RoundToInt(value),
            _ => Mathf.CeilToInt(value)
        };
    }

    public static float GetBase(RuntimeStat stat)
    {
        EnsureInitialized();
        return _baseValues[(int)stat];
    }

    public static RuntimeStatGroup GetGroup(RuntimeStat stat)
    {
        EnsureInitialized();
        return _groups[(int)stat];
    }

    public static void Set(RuntimeStat stat, float value)
    {
        EnsureInitialized();
        _computedValues[(int)stat] = value;
    }

    public static void SetBase(RuntimeStat stat, float value, bool alsoResetComputed = true)
    {
        EnsureInitialized();
        int index = (int)stat;
        _baseValues[index] = value;

        if (alsoResetComputed)
            RebuildStat(stat);
    }

    public static void Add(RuntimeStat stat, float delta)
    {
        EnsureInitialized();
        _computedValues[(int)stat] += delta;
    }

    public static void Mul(RuntimeStat stat, float multiplier)
    {
        EnsureInitialized();
        _computedValues[(int)stat] *= multiplier;
    }

    public static void Reset(RuntimeStat stat)
    {
        EnsureInitialized();
        RebuildStat(stat);
    }

    public static void ResetGroup(RuntimeStatGroup group)
    {
        EnsureInitialized();

        int count = _computedValues.Length;
        for (int i = 0; i < count; i++)
        {
            if (group == RuntimeStatGroup.All || _groups[i] == group)
                RebuildStat((RuntimeStat)i);
        }
    }

    public static void ResetAll()
    {
        ResetGroup(RuntimeStatGroup.All);
    }

    public static int AddTimedModifier(RuntimeStat stat, ModifierMode mode, float value, float duration, int order = 0)
    {
        EnsureInitialized();

        if (duration <= 0f)
        {
            ApplySingleModifier(stat, mode, value);
            return -1;
        }

        int id = _nextModifierId++;
        _timedModifiers.Add(new TimedModifier
        {
            id = id,
            stat = stat,
            mode = mode,
            value = value,
            remainingTime = duration,
            order = order
        });

        RebuildStat(stat);
        return id;
    }

    public static bool RemoveTimedModifier(int modifierId)
    {
        EnsureInitialized();

        for (int i = 0; i < _timedModifiers.Count; i++)
        {
            if (_timedModifiers[i].id != modifierId)
                continue;

            RuntimeStat stat = _timedModifiers[i].stat;
            _timedModifiers.RemoveAt(i);
            RebuildStat(stat);
            return true;
        }

        return false;
    }

    public static void Tick(float dt)
    {
        EnsureInitialized();

        if (_timedModifiers.Count == 0)
            return;

        HashSet<RuntimeStat> dirtyStats = null;

        for (int i = _timedModifiers.Count - 1; i >= 0; i--)
        {
            TimedModifier modifier = _timedModifiers[i];
            modifier.remainingTime -= dt;

            if (modifier.remainingTime <= 0f)
            {
                dirtyStats ??= new HashSet<RuntimeStat>();
                dirtyStats.Add(modifier.stat);
                _timedModifiers.RemoveAt(i);
                continue;
            }

            _timedModifiers[i] = modifier;
        }

        if (dirtyStats == null)
            return;

        foreach (RuntimeStat stat in dirtyStats)
            RebuildStat(stat);
    }

    public static void ClearTimedModifiers(RuntimeStat stat)
    {
        EnsureInitialized();

        bool removedAny = false;
        for (int i = _timedModifiers.Count - 1; i >= 0; i--)
        {
            if (_timedModifiers[i].stat != stat)
                continue;

            _timedModifiers.RemoveAt(i);
            removedAny = true;
        }

        if (removedAny)
            RebuildStat(stat);
    }

    public static void ClearAllTimedModifiers()
    {
        EnsureInitialized();

        if (_timedModifiers.Count == 0)
            return;

        _timedModifiers.Clear();
        ResetAll();
    }

    private static void ApplySingleModifier(RuntimeStat stat, ModifierMode mode, float value)
    {
        switch (mode)
        {
            case ModifierMode.Add:
                Add(stat, value);
                break;

            case ModifierMode.Multiply:
                Mul(stat, value);
                break;

            case ModifierMode.Override:
                Set(stat, value);
                break;
        }
    }

    private static void RebuildStat(RuntimeStat stat)
    {
        int index = (int)stat;
        float result = _baseValues[index];

        if (_timedModifiers.Count == 0)
        {
            _computedValues[index] = result;
            return;
        }

        // Чтобы поведение было стабильным:
        // 1. Add
        // 2. Multiply
        // 3. Override (по order, последний приоритетнее)
        float additive = 0f;
        float multiplicative = 1f;

        bool hasOverride = false;
        float overrideValue = result;
        int overrideOrder = int.MinValue;

        for (int i = 0; i < _timedModifiers.Count; i++)
        {
            TimedModifier mod = _timedModifiers[i];
            if (mod.stat != stat)
                continue;

            switch (mod.mode)
            {
                case ModifierMode.Add:
                    additive += mod.value;
                    break;

                case ModifierMode.Multiply:
                    multiplicative *= mod.value;
                    break;

                case ModifierMode.Override:
                    if (!hasOverride || mod.order >= overrideOrder)
                    {
                        hasOverride = true;
                        overrideValue = mod.value;
                        overrideOrder = mod.order;
                    }
                    break;
            }
        }

        result = (result + additive) * multiplicative;

        if (hasOverride)
            result = overrideValue;

        _computedValues[index] = result;
    }

    public sealed class RuntimeValues
    {
        public float MainWeaponFirerateMultiplier => Get(RuntimeStat.MainWeaponFirerateMultiplier);
        public float TakingDamageMultiplier => Get(RuntimeStat.TakingDamageMultiplier);
        public float TimeSlowMultiplier => Get(RuntimeStat.TimeSlowMultiplier);
        public float CriticalTimeSlowMultiplier => Get(RuntimeStat.CriticalTimeSlowMultiplier);
        public float PrepareTimeMultiplier => Get(RuntimeStat.PrepareTimeMultiplier);

        public float RamHealValue => Get(RuntimeStat.RamHealValue);
        public float RamFirerateBoost => Get(RuntimeStat.RamFirerateBoost);
        public float RamBoostDuration => Get(RuntimeStat.RamBoostDuration);

        public float SpreadMultiplier => Get(RuntimeStat.SpreadMultiplier);
        public float ExplosionDamageMultiplier => Get(RuntimeStat.ExplosionDamageMultiplier);
        public float ExplosionAreaMultiplier => Get(RuntimeStat.ExplosionAreaMultiplier);
        public float PlayerShotSpeedMultiplier => Get(RuntimeStat.PlayerShotSpeedMultiplier);
        public float DeviceDamageMultiplier => Get(RuntimeStat.DeviceDamageMultiplier);

        public float EnemyHealthMultiplier => Get(RuntimeStat.EnemyHealthMultiplier);
        public float EnemyBulletSpeedMultiplier => Get(RuntimeStat.EnemyBulletSpeedMultiplier);
        public float EnemyAggresionMultiplier => Get(RuntimeStat.EnemyAggresionMultiplier);
        public float EnemyMobilityMultiplier => Get(RuntimeStat.EnemyMobilityMultiplier);

        public float AsteroidSpawnRateMultiplier => Get(RuntimeStat.AsteroidSpawnRateMultiplier);
        public float AsteroidSpeedMultiplier => Get(RuntimeStat.AsteroidSpeedMultiplier);

        public float TimeBetweenWaves => Get(RuntimeStat.TimeBetweenWaves);
        public float EnemySpawnReloadMultiplier => Get(RuntimeStat.EnemySpawnReloadMultiplier);
    }
}