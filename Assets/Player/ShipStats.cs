using System.Collections.Generic;
using UnityEngine;

public class ShipStats
{
    public delegate void statOperation(string name, float newValue);
    public static event statOperation StatChanged;

    private Dictionary<string, float> InitGlobalStats = new Dictionary<string, float>()
    {
        // --- Игрок - Инвентарь и снаряжение --- ------------------
        ["ChoiceBoost"] = 0, // округляется до целого вверх
        ["BusChoiceBoost"] = 0,
        ["LuckMultiplier"] = 1f,
        ["PriceMultiplier"] = 1f,
        ["CurrentBusGateChoice"] = 1f,
        ["StrongDebuffs"] = 0,
        // --- Игрок - Здоровье --- --------------------------------
        // основные значения
        ["AdditiveHealth"] = 0, // округляется до целого вверх НННННННННННННННННННННННННН Нужно удалить и сделать для регулировки упрощённый метод в конечном скрипте
        ["AdditiveStartArmor"] = 10, // округляется до целого вверх НННННННННННННННННННННННННН Нужно удалить и сделать для регулировки упрощённый метод в конечном скрипте
        ["AdditiveArmorCap"] = 0, // округляется до целого вверх НННННННННННННННННННННННННН Нужно удалить и сделать для регулировки упрощённый метод в конечном скрипте
        ["CriticalHealthCap"] = 15f, // округляется до целого вверх НННННННННННННННННННННННННН Нужно удалить и сделать для регулировки упрощённый метод в конечном скрипте
        ["BlockArmor"] = 0, // округляется до целого вверх
        ["TimeSlowValue"] = 0.33f,
        ["CriticalTimeSlowMultiplier"] = 0.33f,
        // тараны
        ["RamMoneyValue"] = 3f,// округляется до целого вверх
        //["RamRegenOnCriticalStateMultiplier"] = 3f, // округляется до целого вверх
        ["DecadesBlockForRam"] = 1f, // округляется до целого вверх
        ["RamFirerateBoost"] = 0.5f,
        ["RamBoostDuration"] = 0.66f,
        // восстановление здоровья
        ["LevelEntryHealthRegen"] = 3f, // округляется до целого вверх НННННННННННННННННННННННННН Нужно удалить и сделать для регулировки упрощённый метод в конечном скрипте
        ["PartHealOnBossDefeat"] = 0.75f,
        ["ParryingArmorRegeneration"] = 1f, // округляется до целого вверх
        // --- Игрок - Атака --- ------------------------------------
        // общее
        ["FlatDamageBoost"] = 0, // округляется до целого вверх
        ["PlayerShotSpeedMultiplier"] = 1f,
        ["PlayerShotLifetimeMultiplier"] = 1f,
        // щиты
        ["ShieldDamageMultiplier"] = 1f,
        ["ShieldAreaScaleMultiplier"] = 1f,
        ["BulletsForParry"] = 2f, // округляется до целого вверх
        ["ParryAdditiveFirerateBoost"] = 0.5f,
        // эмиссия
        ["EmissionFillTime"] = 30f,
        ["EmissionFirerateBoost"] = 3f,
        ["EmissionDuration"] = 4f,
        ["EmissionBlock"] = 5f, // округляется до целого вверх
        ["EmissionResetPart"] = 1f,
        // главное оружие
        ["MainWeaponDamageMultiplier"] = 1f,
        ["MainWeaponFirerateMultiplier"] = 1f,
        // устройства
        ["DeviceDamageMultiplier"] = 1f,
        ["DeviceChargeTimeBoostFlat"] = 0,
        ["DeviceChargeTimeBoostMultiplier"] = 1f,
        ["DeviceResetSpeedMultiplier"] = 1f,
        ["DeviceReloadBoostAfterUse"] = 0.15f,
        // ультратехнологии
        ["AdditiveUltratechDamageMultiplier"] = 1f,
        ["UltratechDurationMultiplier"] = 1f,
        ["AdditiveRamsToUltratech"] = 0, // округляется до целого вверх
        ["AdditiveUltratechShield"] = 0, // округляется до целого вверх
        // --- Игрок - дополнительно --- -----------------------------
        ["TimeSlowMultiplier"] = 1f,
        ["CriticalTimeSlowMultiplier"] = 0.33f,
        ["SpreadMultiplier"] = 1f,
        ["SpreadSpeedMultiplier"] = 1f,
        ["FlatHomingBoost"] = 0,
        ["HomingEfficiencyMultiplier"] = 1f,
        ["EnemyPiercesBoost"] = 0, // округляется до целого вверх
        ["PiercedDamageMultiplier"] = 0.66f,
        ["ExplosionDamageMultiplier"] = 1f,
        ["ExplosionAreaMultiplier"] = 1f,
        ["FlatBulletAccelerationBoost"] = 0,
        ["BulletAccelerationMultiplier"] = 1f,
        ["WeaponAccelerationMultiplier"] = 1f,
        ["ShieldCapacityMultiplier"] = 1f,
        ["PickupChanceMultiplier"] = 1f,
        // --- Окружение --- ------------------------------------------
        ["AsteroidSpawnRateMultiplier"] = 1f,
        ["AsteroidSpeedMultiplier"] = 1f,
        ["AsteroidHealthMultiplier"] = 1f,
        ["AsteroidBlockArmorBoost"] = 0, // округляется до целого вверх
        // --- Спаун врагов --- ---------------------------------------
        ["AdditiveWaves"] = 0, // округляется до целого вверх НННННННННННННННННННННННННН Нужно удалить и сделать для регулировки упрощённый метод в конечном скрипте
        ["WaveWeightBoost"] = 0, // округляется до целого вверх
        ["AdditiveWaveBuffer"] = 0, // округляется до целого вверх
        ["TimeBetweenWaves"] = 4f,
        ["EnemySpawnReloadMultiplier"] = 0.45f,
        ["StalkerChance"] = 0.01f,
        ["EliteChance"] = 0.01f,
        // --- Враги - здоровье --- -----------------------------------
        ["EnemyHealthMultiplier"] = 1f,
        ["AdditiveBlockArmor"] = 0, // округляется до целого вверх
        ["EnemyShieldHealthBoost"] = 0, // округляется до целого вверх
        // --- Враги - атака --- --------------------------------------
        ["EnemyAggresionMultiplier"] = 1f,
        ["EnemyMobilityMultiplier"] = 1f,
        ["EnemyProficienceyMultiplier"] = 1f,
        ["TakingDamageMultiplier"] = 1f,
        ["EnemyBulletSpeedMultiplier"] = 1f    
        
    };

    private static Dictionary<string, float> GlobalStats = new Dictionary<string, float>();

    public void Initialize()
    {
        //ShowCapacity();
        ClearSubscribers();
        GlobalStats = new Dictionary<string, float>(InitGlobalStats);
        //ShowCapacity();
    }

    public static void ShowCapacity() => Debug.Log(GlobalStats.Count);

    public static void ClearSubscribers() => StatChanged = null;

    public static int GetIntValue(string statName)
    {
        if (GlobalStats.ContainsKey(statName))
            return Mathf.CeilToInt(GlobalStats[statName]);
        else 
            throw new System.Exception("Несуществующий параметр в ShipStats");
    }

    public static float GetValue(string statName)
    {
        if (GlobalStats.ContainsKey(statName))
            return GlobalStats[statName];
        else 
            throw new System.Exception("Несуществующий параметр в ShipStats");
    }

    public static void ModifyStat(string name, float newValue)
    {
        if (!GlobalStats.ContainsKey(name))
            throw new System.Exception("Несуществующий параметр в ShipStats");
        GlobalStats[name] = newValue;
        StatChanged?.Invoke(name, newValue);
    }

    public static void IncreaseStat(string name, float addValue)
    {
        if (!GlobalStats.ContainsKey(name))
            throw new System.Exception("Несуществующий параметр в ShipStats");
        GlobalStats[name] += addValue;
        StatChanged?.Invoke(name, GlobalStats[name]);
    }

    public static void MultiplyStat(string name, float multiplier)
    {
        if (!GlobalStats.ContainsKey(name))
            throw new System.Exception("Несуществующий параметр в ShipStats");
        GlobalStats[name] *= multiplier;
        StatChanged?.Invoke(name, GlobalStats[name]);
    }
    public static void AddNewStat(string statName, float defaultValue = 1f)
    {
        if (GlobalStats.ContainsKey(statName))
            return;
        
        GlobalStats[statName] = defaultValue;
    }

    // public static float DamageMultiplier{get; set;} = 1f;
    // public static float FirerateMultiplier{get; set;} = 1f;
    // public static float ShotSpeedMultiplier{get; set;} = 1f;
    // public static float TimeSlowingEffeciency{get; set;} = 1f;
    // public static float TakingDamageMultiplier{get; set;} = 1f;
    // public static float SpreadMultiplier{get; set;} = 1f;
    // public static float PowerEffeciency{get; set;} = 1f;

    // public static int DecadesBlocksForRam {get; set;} = 1;
    // public static int RamHealValue {get; set;} = 3;
    // public static float RamFirerateBoost {get; set;} = 0.5f;
    // public static float RamBoostDuration {get; private set;} = 0.75f;

    // public static float ParryingAdditiveFirerateBonus {get; set;} = 0.5f;
    // public static int NeedBulletsForParryingBonus {get; set;} = 2;
}
