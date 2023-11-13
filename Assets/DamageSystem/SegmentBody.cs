using DamageSystem;
using UnityEngine;

public class SegmentBody : DamageBody
{
    public event deathHandler SegmentZeroHP;

    [Header("Всегда, когда индивидуальное здоровье заканчиваетсяя - вызывается только событие SegmentZeroHP")]
    [SerializeField] private DamageBody _mainDamageBody;
    [SerializeField] private bool _individuallyHP;

    public override void TakeDamage(int damage)
    {
        if (HitPoints <= 0)
            return;

        if (_individuallyHP)
        {
            base.TakeDamage(damage);
            return;
        }

        _mainDamageBody.TakeDamage(damage);
    }

    protected override void Death()
    {
        SegmentZeroHP?.Invoke();
    }
}
