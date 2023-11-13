using UnityEngine;
using DamageSystem;

public class StaticBullet : MonoBehaviour
{
    [SerializeField] private DamageKey _damageKey;
    [SerializeField] private int _damageValue;

    public int ModdedDamage {get; set;} = -1;

    private void OnDisable() {
        if (ModdedDamage != -1)
            ModdedDamage = -1;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        DamageBody damageBody = other.GetComponent<DamageBody>();
        //print(other.gameObject.name);

        //print(ModdedDamage);
        if (damageBody == null)
            return;

        //if (damageBody.KeyDamage != _damageKey)
            //return;

        if (ModdedDamage == -1)
            AttackObject.InflictDamage(damageBody, _damageKey, _damageValue);
            //damageBody.TakeDamage(_damageValue);
        else
            AttackObject.InflictDamage(damageBody, _damageKey, ModdedDamage);

    }
}
