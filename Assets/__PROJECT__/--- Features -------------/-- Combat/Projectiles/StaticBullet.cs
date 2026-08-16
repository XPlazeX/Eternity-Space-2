using UnityEngine;
using DamageSystem;

public class StaticBullet : MonoBehaviour
{
    public event System.Action<GameObject> Collided;

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
        IDamagable idamagable = other.GetComponent<IDamagable>();

        if (idamagable == null || (idamagable.KeyDamage == DamageKey.Player))
            return;

        Collided?.Invoke(other.gameObject);

        if (!IsModdedDamage)
            AttackObject.InflictDamage(idamagable, damageBundle, out bool killed);
        else
        {
            AttackObject.InflictDamage(idamagable, _moddedDamageBundle, out bool killed);
        }
            

        //print("inf");
    }
}
