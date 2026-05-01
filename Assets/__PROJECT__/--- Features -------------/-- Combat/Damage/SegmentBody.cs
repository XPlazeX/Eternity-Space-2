using DamageSystem;
using UnityEngine;

public class SegmentBody : DamageBody
{
    public event deathHandler SegmentZeroHP;

    [Header("Всегда, когда индивидуальное здоровье заканчиваетсяя - вызывается только событие SegmentZeroHP")]
    [SerializeField] private DamageBody _mainDamageBody;
    [SerializeField] private bool _individuallyHP;

    public override bool TakeDamage(DamageBundle damageBundle, out bool killed)
    {
        if (HitPoints <= 0)
        {
            killed = false;
            return false;
        }    

        if (_individuallyHP)
        {
            base.TakeDamage(damageBundle, out killed);
            return true;
        }

        _mainDamageBody.TakeDamage(damageBundle, out killed);
        return true;
    }

    protected override void Death()
    {
        SegmentZeroHP?.Invoke();
    }
}
