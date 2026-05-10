public enum RuntimeStat
{
    // --- Player / Core ---
    MainWeaponFirerateMultiplier,
    MainWeaponFirerateRandomizing,
    MainWeaponFlatSpread,
    MainWeaponPrepareTimeMultiplier,
    TakingDamageMultiplier,
    TimeSlowMultiplier,
    CriticalTimeSlowMultiplier,
    PrepareTimeMultiplier,

    // --- Ram ---
    RamHealValue,
    RamFirerateBoost,
    RamBoostDuration,

    // --- Weapon / Combat ---
    SpreadMultiplier,
    ExplosionDamageMultiplier,
    ExplosionAreaMultiplier,
    PlayerShotSpeedMultiplier,
    DeviceDamageMultiplier,

    // --- Enemy ---
    EnemyHealthMultiplier,
    EnemyBulletSpeedMultiplier,
    EnemyAggresionMultiplier,
    EnemyMobilityMultiplier,
    EnemyForesightAddition,

    // --- Environment ---
    AsteroidSpawnRateMultiplier,
    AsteroidSpeedMultiplier,

    // --- Waves ---
    TimeBetweenWaves,
    EnemySpawnReloadMultiplier,
}

public enum RuntimeStatGroup
{
    Player,
    Ram,
    Weapon,
    Enemy,
    Environment,
    Waves,
    All
}