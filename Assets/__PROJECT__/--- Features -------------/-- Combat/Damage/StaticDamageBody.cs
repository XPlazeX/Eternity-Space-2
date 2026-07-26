using DamageSystem;
using UnityEngine;

public class StaticDamageBody : MonoBehaviour,IDamagable
{
    [SerializeField] protected DamageKey _damageKey;

    public DamageKey KeyDamage => _damageKey;

    public bool TakeDamage(DamageBundle damageBundle, out bool killed)
    {
        killed = false;
        return true;
    }
}
