using UnityEngine;
using DamageSystem;

public class BulletSlowField : MonoBehaviour
{
    [SerializeField] private float _slowMultiplier;
    [SerializeField] private DamageKey _keyDamage;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        Bullet bullet = other.GetComponent<Bullet>();

        if (bullet == null || bullet.KeyDamage != _keyDamage)
            return;

        bullet.MultiplySpeedParams(_slowMultiplier);
        
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        Bullet bullet = other.GetComponent<Bullet>();

        if (bullet == null || bullet.KeyDamage != _keyDamage)
            return;

        bullet.MultiplySpeedParams(1f / _slowMultiplier);    
    }
}
