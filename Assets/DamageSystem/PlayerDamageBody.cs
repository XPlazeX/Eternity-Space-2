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
        if (PlayerShipData.Hover || other.GetComponent<Hover>() != null)
            return;

        DamageBody damageBody = other.GetComponent<DamageBody>();

        if ((damageBody == null) || (damageBody.KeyDamage == _damageKey) || (damageBody.KeyDamage == DamageSystem.DamageKey.Unvulnerable))
            return;

        int otherHP = damageBody.HitPoints;

        if ((otherHP <= PlayerRamsHandler.DecadesBlockForRam * 10) && (damageBody.GetType() != typeof(AsteroidBody)))
        {
            PlayerRamsHandler.TryRam();
            damageBody.TakeDamage((PlayerRamsHandler.DecadesBlockForRam * 10));
            return;
        }

        if (PlayerShipData.Invulnerable)
            return;

        damageBody.TakeDamage(ShipStats.GetIntValue("MaxDamageTaken"));
        TakeDamage(otherHP);
    }

}
