using UnityEngine;

public class _ExplosionBullet : MonoBehaviour
{
    [SerializeField] private int _explosionCode = 0;
    [SerializeField] private bool _mute = false;
    [SerializeField] private bool _modParams = false;
    [SerializeField] private Color _color = Color.yellow;
    [SerializeField] private float _scale = 1f;
    [SerializeField] private float _shakePower = 0f;
    [Header("Если урон = 0, то урон взрыва по умолчанию не будет изменён")]
    [SerializeField] private int _damage;

    private ExplosionHandler _explosionHandler;

    private void Start() {
        _explosionHandler = SceneStatics.SceneCore.GetComponent<ExplosionHandler>();

        if (GetComponent<Bullet>())
            GetComponent<Bullet>().Deathed += SpawnExplosion;
    }

    public void SpawnExplosion()
    {
        if (_explosionHandler == null || !CameraController.InsideSoundArea(transform.position))
            return;

        GameObject explosion = _explosionHandler.InstantiateExplosion(transform.position, _explosionCode);

        if (_mute)
            explosion.GetComponent<Explosion>().Mute();

        if (!_modParams)
            return;

        if (_damage > 0)
            explosion.GetComponent<StaticBullet>().ModdedDamage = _damage;

        explosion.GetComponent<SpriteRenderer>().color = _color;
        explosion.transform.localScale = Vector3.one * _scale;

        if (_shakePower > 0)
            CameraController.Shake(_shakePower);
    }


}
