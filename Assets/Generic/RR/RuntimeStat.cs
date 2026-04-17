public enum RuntimeStat
{
    // --- Player / Core ---
    MainWeaponFirerateMultiplier,
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