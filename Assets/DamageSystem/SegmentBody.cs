using DamageSystem;
using UnityEngine;

public class SegmentBody : DamageBody
{
    public event deathHandler SegmentZeroHP;

    [Header("Всегда, когда индивидуальное здоровье заканчиваетсяя - вызывается только событие SegmentZeroHP")]
    [SerializeField] private DamageBody _mainDamageBody;
    [SerializeField] private bool _individuallyHP;

    public override bool TakeDamage(DamageBundle damageBundle)
    {
        if (HitPoints <= 0)
            return false;

        if (_individuallyHP)
        {
            base.TakeDamage(damageBundle);
            return true;
        }

        _mainDamageBody.TakeDamage(damageBundle);
        return true;
    }

    protected override void Death()
    {
        SegmentZeroHP?.Invoke();
    }
}
