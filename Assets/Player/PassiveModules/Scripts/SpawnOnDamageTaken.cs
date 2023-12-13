using UnityEngine;

public class SpawnOnDamageTaken : Module
{
    [SerializeField] private int _explosionCode;
    [SerializeField] private bool _modParams = true;
    [SerializeField] private Color _color = Color.yellow;
    [SerializeField] private float _scale = 1f;
    [SerializeField] private int _damage;
    [SerializeField] private SpawnCondition _condition;

    private ExplosionHandler _explosionHandler;

    [System.Flags]
    private enum SpawnCondition
    {
        None = 0,
        DamageTaken = 1,
        Ram = 1 << 1
    }

    private void Start() {
        _explosionHandler = SceneStatics.SceneCore.GetComponent<ExplosionHandler>();
    }

    public override void Load()
    {
        if ((_condition & SpawnCondition.DamageTaken) == SpawnCondition.DamageTaken)
            PlayerShipData.TakeHealthDamage += Spawn;

        if ((_condition & SpawnCondition.Ram) == SpawnCondition.Ram)
            PlayerRamsHandler.RamSuccess += Spawn;
    }

    private void OnDisable() {
        if ((_condition & SpawnCondition.DamageTaken) == SpawnCondition.DamageTaken)
            PlayerShipData.TakeHealthDamage -= Spawn;

        if ((_condition & SpawnCondition.Ram) == SpawnCondition.Ram)
            PlayerRamsHandler.RamSuccess -= Spawn;
    }

    public void Spawn()
    {
        Spawn(1);
    }

    public void Spawn(int val = 0)
    {
        GameObject explosion = _explosionHandler.InstantiateExplosion(Player.PlayerTransform.position, _explosionCode);

        if (!_modParams)
            return;

        if (_damage > 0)
            explosion.GetComponent<StaticBullet>().ModdedDamage = _damage;

        explosion.GetComponent<SpriteRenderer>().color = _color;
        explosion.transform.localScale = Vector3.one * _scale;
    }
}
