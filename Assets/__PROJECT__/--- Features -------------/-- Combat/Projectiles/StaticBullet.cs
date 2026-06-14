using UnityEngine;
using DamageSystem;

public class StaticBullet : MonoBehaviour
{
    [SerializeField] private DamageBundle damageBundle;
    [SerializeField] private bool _hoverable = false;

    public bool IsModdedDamage {get; private set;} = false;
    private DamageBundle _moddedDamageBundle;

    private void OnDisable() {
        if (IsModdedDamage)
        {
            IsModdedDamage = false;
            _moddedDamageBundle = null;
        }
    }

    public void MultiplyDamage(float multiplier)
    {
        IsModdedDamage = true;
        _moddedDamageBundle = new DamageBundle(damageBundle)
        {
            damageValue = Mathf.RoundToInt(damageBundle.damageValue * multiplier)
        };
    }

    public void ModDamageBundle(DamageBundle newBundle)
    {
        IsModdedDamage = true;
        _moddedDamageBundle = new DamageBundle(newBundle);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        DamageBody damageBody = other.GetComponent<DamageBody>();

        if (damageBody == null || (damageBody.KeyDamage == DamageKey.Player && (_hoverable && PlayerShipData.Hover)))
            return;

        if (!IsModdedDamage)
            AttackObject.InflictDamage(damageBody, damageBundle, out bool killed);
        else
        {
            AttackObject.InflictDamage(damageBody, _moddedDamageBundle, out bool killed);
        }
            

        //print("inf");
    }
}
