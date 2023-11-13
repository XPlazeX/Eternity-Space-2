using UnityEngine;

public class SpawnOnDamageTaken : Module
{
    [SerializeField] private int _explosionCode;
    [SerializeField] private Color _color = Color.yellow;
    [SerializeField] private float _scale = 1f;
    [SerializeField] private int _damage;
    [SerializeField] private bool _onRam;

    private ExplosionHandler _explosionHandler;

    private void Start() {
        _explosionHandler = SceneStatics.SceneCore.GetComponent<ExplosionHandler>();
    }

    public override void Load()
    {
        PlayerShipData.TakeHealthDamage += Spawn;

        if (_onRam)
            PlayerRamsHandler.RamSuccess += Spawn;
    }

    private void OnDisable() {
        PlayerShipData.TakeHealthDamage -= Spawn;

        if (_onRam)
            PlayerRamsHandler.RamSuccess -= Spawn;
    }

    public void Spawn()
    {
        Spawn(1);
    }

    public void Spawn(int val = 0)
    {
        GameObject explosion = _explosionHandler.InstantiateExplosion(Player.PlayerTransform.position, _explosionCode);

        if (_damage > 0)
            explosion.GetComponent<StaticBullet>().ModdedDamage = _damage;

        explosion.GetComponent<SpriteRenderer>().color = _color;
        explosion.transform.localScale = Vector3.one * _scale;
    }
}
