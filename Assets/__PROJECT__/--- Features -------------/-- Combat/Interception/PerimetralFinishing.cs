using DamageSystem;
using UnityEngine;

public class PerimetralFinishing : MonoBehaviour
{
    [SerializeField] private Collider2D triggerCollider;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private DamageKey targetDamageKey;
    [SerializeField] private DamageBundle targetDamageBundle;
    [SerializeField] private ExplosionRepeater visualExplosion;
    [SerializeField] private float reloadTime = 0.5f;
    [SerializeField] private TriggerCondition triggerCondition;
    [SerializeField] private int hpThreshold = 10;
    [SerializeField] private DamageBundle othersDamageBundle;

    private readonly Collider2D[] _overlapResults = new Collider2D[128];

    private ContactFilter2D _contactFilter;
    private float _reloadTimer;

    private void Awake()
    {
        _contactFilter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = targetMask,
            useTriggers = true
        };
    }

    void FixedUpdate()
    {
        _reloadTimer -= ESTime.worldFixedDeltaTime;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_reloadTimer > 0f) return;

        if (!PassesCondition(other, out DamageBody damageBody))
            return;

        _reloadTimer = reloadTime;
        if (visualExplosion != null)
        {
            visualExplosion.Activate();
        }

        if (damageBody != null)
        {
            AttackObject.InflictDamage(damageBody, targetDamageBundle, out bool killed);
        }

        int count = triggerCollider.Overlap(_contactFilter, _overlapResults);

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = _overlapResults[i];

            if (hit == null)
                continue;

            HandleColliderInside(hit);
        }
    }

    private bool PassesCondition(Collider2D other, out DamageBody damageBody)
    {
        damageBody = other.GetComponent<DamageBody>();

        if (damageBody == null) return false;
        else if (damageBody.KeyDamage != targetDamageKey) return false;

        switch (triggerCondition)
        {
            case TriggerCondition.UseAlways: return true;
            case TriggerCondition.UseHPThreshold: return damageBody.HitPoints <= hpThreshold;
            case TriggerCondition.UseRamReady: return damageBody.RamReady;
            default:
                return false;
        }
    }

    private void HandleColliderInside(Collider2D other)
    {
        DamageBody damageBody = other.GetComponent<DamageBody>();

        if (damageBody == null) return;
        else if (damageBody.KeyDamage != targetDamageKey) return;

        AttackObject.InflictDamage(damageBody, othersDamageBundle, out bool killed);
    }

    private enum TriggerCondition
    {
        UseRamReady,
        UseHPThreshold,
        UseAlways
    }
}
