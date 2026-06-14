using DamageSystem;
using UnityEngine;

public class _ExplosionBullet : MonoBehaviour
{
    [SerializeField] private int _explosionCode = 0;
    [SerializeField] private ExplosionObject _explosionObject;
    [SerializeField] private bool _mute = false;
    [SerializeField] private bool _modParams = false;
    [SerializeField] private Color _color = Color.yellow;
    [SerializeField] private float _scale = 1f;
    [SerializeField] private float _shakePower = 0f;
    [SerializeField] private bool _modDamage = false;
    [SerializeField] private DamageBundle _damageBundle;

    public int ExplosionCode {get {return _explosionCode;} set {_explosionCode = value;}}
    public Color ExplosionColor {get {return _color;} set {_color = value;}}
    public float Scale {get {return _scale;} set {_scale = value;}}
    public DamageBundle DamageBundle {get {return _damageBundle;} set {_damageBundle = value;}}

    // private ExplosionHandler _explosionHandler;

    private void Start() {
        // _explosionHandler = SceneStatics.SceneCore.GetComponent<ExplosionHandler>();
        // _explosionHandler.PreloadExplosion(_explosionCode);

        if (GetComponent<Bullet>())
            GetComponent<Bullet>().Deathed += SpawnExplosion;
    }

    public void SpawnExplosion(Vector3 position)
    {
        // if (_explosionHandler == null )//|| !CameraController.InsideSoundArea(position)
        //     return;
        if (_explosionObject == null)
        {
            Debug.LogError($"Null explosion object on {gameObject.name}");
            return;
        }

        ExplosionObject explosion = Pool.Spawn(_explosionObject, position, Quaternion.identity);//_explosionHandler.InstantiateExplosion(position, _explosionCode);

        if (_mute)
            explosion.Mute();
            //explosion.GetComponent<Explosion>().Mute();

        if (_modDamage)
        {
            explosion.SetDamageBundle(_damageBundle);
        }
        

        if (!_modParams)
            return;

        // if (_damage > 0)
        //     explosion.GetComponent<StaticBullet>().ModdedDamage = Mathf.RoundToInt((float)_damage * ShipStats.GetValue("ExplosionDamageMultiplier"));

        explosion.SetColor(_color);

        // if (explosion.GetComponent<SpriteRenderer>() != null)
        //     explosion.GetComponent<SpriteRenderer>().color = _color;
        explosion.SetScale(_scale);// * (1f / ShipStats.GetValue("Pressure"));

        if (_shakePower > 0)
            CameraController.Shake(_shakePower);
    }

    public void SpawnExplosion()
    {
        SpawnExplosion(transform.position);
    }
}
