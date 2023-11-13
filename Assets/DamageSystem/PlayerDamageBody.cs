using UnityEngine;

public class PlayerDamageBody : DamageBody
{
    //private int _hpCap = 0;

    public override void TakeDamage(int damage)
    {
        PlayerShipData.TakeDamage(damage);

        //print("taked damage : " + damage.ToString());
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
            //print("tryRam");
            PlayerRamsHandler.TryRam();
            damageBody.TakeDamage((_decadesBlockForRam * 10));
            return;
        }

        damageBody.TakeDamage(PlayerShipData.HitPoints);
        TakeDamage(otherHP);
    }

}
