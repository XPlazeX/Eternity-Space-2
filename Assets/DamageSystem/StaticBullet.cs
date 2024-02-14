using UnityEngine;
using DamageSystem;

public class StaticBullet : MonoBehaviour
{
    [SerializeField] private DamageKey _damageKey;
    [SerializeField] private int _damageValue;
    [SerializeField] private bool _hoverable = false;

    public int ModdedDamage {get; set;} = -1;

    private void OnDisable() {
        if (ModdedDamage != -1)
            ModdedDamage = -1;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        DamageBody damageBody = other.GetComponent<DamageBody>();

        if (damageBody == null || (damageBody.KeyDamage == DamageKey.Player && (_hoverable && PlayerShipData.Hover)))
            return;

        if (ModdedDamage == -1)
            AttackObject.InflictDamage(damageBody, _damageKey, _damageValue);
        else
            AttackObject.InflictDamage(damageBody, _damageKey, ModdedDamage);

        //print("inf");
    }
}
