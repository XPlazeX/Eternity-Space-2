using UnityEngine;

public class PlayerDamageBody : DamageBody
{
    public override void TakeDamage(int damage)
    {
        PlayerShipData.TakeDamage(damage);
    }

    public override void GetShield(int shieldPoints)
    {
        PlayerShipData.GetShield(shieldPoints);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (PlayerShipData.Invulnerable || PlayerShipData.Hover)
            return;

        DamageBody damageBody = other.GetComponent<DamageBody>();

        if ((damageBody == null) || (damageBody.KeyDamage == _damageKey))
            return;

        int otherHP = damageBody.HitPoints;

        if ((otherHP <= _decadesBlockForRam * 10) && (damageBody.GetType() != typeof(AsteroidBody)))
        {
            PlayerRamsHandler.TryRam();
            damageBody.TakeDamage((_decadesBlockForRam * 10));
            return;
        }

        damageBody.TakeDamage(ShipStats.GetIntValue("MaxDamageTaken"));
        TakeDamage(otherHP);
    }

}
