using DamageSystem;
using UnityEngine;

public class PlayerDamageBody : DamageBody
{
    public override bool TakeDamage(DamageBundle damageBundle)
    {
        PlayerShipData.TakeDamage(damageBundle);
        return true;
    }

    public override void GetShield(int shieldPoints)
    {
        PlayerShipData.GetShield(shieldPoints);
    }

    public override void GetDamageBuffer(int db)
    {
        PlayerShipData.AddDamageBuffer(db);
    }

    public void OnTriggerEnter2D(Collider2D other) 
    {
        if (PlayerShipData.Hover || other.GetComponent<Hover>() != null)
            return;

        DamageBody damageBody = other.GetComponent<DamageBody>();

        if ((damageBody == null) || (damageBody.KeyDamage == _damageKey) || (damageBody.KeyDamage == DamageSystem.DamageKey.Unvulnerable))
            return;

        int otherHP = damageBody.HitPoints;

        if (damageBody.RamReady && (damageBody.GetType() != typeof(AsteroidBody)))
        {
            PlayerRamsHandler.TryRam();
            damageBody.TakeDamage(new DamageBundle()
            {
                damageKey = DamageKey.Everything,
                damageValue = PlayerRamsHandler.RamDamage,
                ignoreOneShotProtection = true
            });
            return;
        }

        if (PlayerShipData.Unvulnerable)
            return;

        damageBody.TakeDamage(new DamageBundle()
        {
            damageKey = DamageKey.Everything,
            damageValue = ShipStats.GetIntValue("MaxDamageTaken"),
            ignoreOneShotProtection = true
        });
        TakeDamage(new DamageBundle()
        {
            damageKey = DamageKey.Player,
            damageValue = otherHP
        });
    }

}
