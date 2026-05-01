using UnityEngine;
using DamageSystem;

public class BulletReflector : MonoBehaviour
{
    [SerializeField] private bool _playerReflecting = true;
    [SerializeField] private int _reflectedProjectileID;
    [SerializeField] private DamageKey _catchingKey;
    [SerializeField] private DamageKey _reflectingKey;
    [SerializeField] private int _reflectCount = 1;
    [Space()]
    [SerializeField][Range(0, 180f)] private float _reflectSpread = 5f;
    [SerializeField] private SoundObject _reflectSound;

    private bool _soundPlayed = false;
    //private bool _mirroring = true;
    public static bool PlayerMirroring {get; private set;} = true;
    public static DamageKey PlayerCatchingKey {get; private set;} = DamageKey.Player;
    public static int PlayerAdditiveCount {get; private set;} = 0;

    private void OnEnable() {
        _soundPlayed = false;
    }

    public static void ChangePlayerCathingKey(DamageKey newKey)
    {
        PlayerCatchingKey = newKey;
    }

    public static void TogglePlayerMirroring(bool tog)
    {
        PlayerMirroring = tog;
    }

    public static void AddPlayerReflectsCount(int count)
    {
        PlayerAdditiveCount += count;
    }

    private void OnTriggerEnter2D(Collider2D thing)
    {
        Bullet catchedBullet = thing.GetComponent<Bullet>();

        if (catchedBullet == null || catchedBullet.GetComponent<_Reflected_>() != null)
            return;

        if (catchedBullet.KeyDamage != (_playerReflecting ? PlayerCatchingKey : _catchingKey))
            return;

        float angle = catchedBullet.transform.eulerAngles.z;
        Vector3 position = catchedBullet.transform.position;

        catchedBullet.Parrying();

        for (int i = 0; i < (_playerReflecting ? _reflectCount + PlayerAdditiveCount : _reflectCount); i++)
        {
            AttackObject reflectedProjectile = ReflectorHandler.GetReflectedProjectile(_reflectedProjectileID, _reflectingKey, _playerReflecting);

            reflectedProjectile.transform.position = position;
            reflectedProjectile.transform.rotation = Quaternion.Euler(0, 0, angle - ((_playerReflecting ? PlayerMirroring : true) ? 180f : 0f) + Random.Range(-_reflectSpread, _reflectSpread));   
        }

        if (!_soundPlayed)
        {
            SoundPlayer.PlaySound(_reflectSound, transform.position);
            _soundPlayed = true;
        }
    }
}
