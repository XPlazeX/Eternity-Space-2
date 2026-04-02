using UnityEngine;
using DamageSystem;

public class StaticBullet : MonoBehaviour
{
    [SerializeField] private DamageBundle damageBundle;
    [SerializeField] private bool _hoverable = false;

    public int ModdedDamage {get; set;} = -1;

    private void OnDisable() {
        if (ModdedDamage != -1)
            ModdedDamage = -1;
    }

    public void MultiplyDamage(float multiplier)
    {
        ModdedDamage = Mathf.RoundToInt(damageBundle.damageValue * multiplier);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        DamageBody damageBody = other.GetComponent<DamageBody>();

        if (damageBody == null || (damageBody.KeyDamage == DamageKey.Player && (_hoverable && PlayerShipData.Hover)))
            return;

        if (ModdedDamage == -1)
            AttackObject.InflictDamage(damageBody, damageBundle);
        else
            AttackObject.InflictDamage(damageBody, damageBundle);

        //print("inf");
    }
}
