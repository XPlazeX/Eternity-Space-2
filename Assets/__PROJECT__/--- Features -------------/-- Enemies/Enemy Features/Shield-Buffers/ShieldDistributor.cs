using UnityEngine;

public class ShieldDistributor : MonoBehaviour
{
    [SerializeField] private ShieldComponent _shieldObject;
    [SerializeField] private DamageBufferComponent damageBufferComponent;

    private static PullForObjects _shieldPool;
    private static PullForObjects _damageBufferPool;

    public bool Initialize()
    {
        _shieldPool = new PullForObjects(_shieldObject);
        _damageBufferPool = new PullForObjects(damageBufferComponent);

        return true;
    }

    public static ShieldComponent SpawnShield(Transform carrier, int maxHP)
    {
        ShieldComponent shield = _shieldPool.GetGameObject().GetComponent<ShieldComponent>();
        shield.InitializeCarrier(carrier, maxHP);

        return shield;
    }

    public static DamageBufferComponent SpawnDamageBuffer(Transform carrier)
    {
        DamageBufferComponent db = _damageBufferPool.GetGameObject().GetComponent<DamageBufferComponent>();
        db.InitializeCarrier(carrier);

        return db;
    }
}
