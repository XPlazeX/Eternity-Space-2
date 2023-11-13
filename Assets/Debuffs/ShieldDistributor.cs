using UnityEngine;

public class ShieldDistributor : MonoBehaviour
{
    [SerializeField] private ShieldComponent _shieldObject;

    private static PullForObjects _shieldPool;

    public bool Initialize()
    {
        _shieldPool = new PullForObjects(_shieldObject);

        return true;
    }

    public static ShieldComponent SpawnShield(Transform carrier, int maxHP)
    {
        ShieldComponent shield = _shieldPool.GetGameObject().GetComponent<ShieldComponent>();
        shield.InitializeCarrier(carrier, maxHP);

        return shield;
    }
}
