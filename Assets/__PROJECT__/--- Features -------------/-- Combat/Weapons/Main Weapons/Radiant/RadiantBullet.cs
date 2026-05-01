using DamageSystem;
using UnityEngine;

[RequireComponent(typeof(Bullet))]
public class RadiantBullet : MonoBehaviour
{
    private Bullet _cachedBullet;

    private void OnEnable() {
        if (_cachedBullet == null)
        {
            _cachedBullet = GetComponent<Bullet>();
        }
        _cachedBullet.Killed += OnKill;
    }

    void OnDisable()
    {
        if (_cachedBullet == null)
        {
            _cachedBullet = GetComponent<Bullet>();
        }
        _cachedBullet.Killed -= OnKill;
    }

    private void OnKill(IDamagable damagable)
    {
        RadiantChargeBank.RadiantBulletKill();
    }
}
