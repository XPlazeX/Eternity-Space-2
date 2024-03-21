using UnityEngine;
using DamageSystem;

public class BulletReflector : MonoBehaviour
{
    [SerializeField] private bool _playerReflecting = true;
    [SerializeField] private int _reflectedProjectileID;
    [SerializeField] private DamageKey _catchingKey;
    [SerializeField] private DamageKey _reflectingKey;
    [Space()]
    [SerializeField][Range(0, 180f)] private float _reflectSpread = 5f;
    [SerializeField] private SoundObject _reflectSound;

    private bool _soundPlayed = false;

    private void OnEnable() {
        _soundPlayed = false;
    }

    private void OnTriggerEnter2D(Collider2D thing)
    {
        Bullet catchedBullet = thing.GetComponent<Bullet>();

        if (catchedBullet == null)
            return;

        if (catchedBullet.KeyDamage != _catchingKey)
            return;

        float angle = catchedBullet.transform.eulerAngles.z;
        Vector3 position = catchedBullet.transform.position;

        catchedBullet.Parrying();

        AttackObject reflectedProjectile = ReflectorHandler.GetReflectedProjectile(_reflectedProjectileID, _reflectingKey, _playerReflecting);

        reflectedProjectile.transform.position = position;
        reflectedProjectile.transform.rotation = Quaternion.Euler(0, 0, angle - 180f + Random.Range(-_reflectSpread, _reflectSpread));

        if (!_soundPlayed)
        {
            SoundPlayer.PlaySound(_reflectSound, transform.position);
            _soundPlayed = true;
        }
    }
}
