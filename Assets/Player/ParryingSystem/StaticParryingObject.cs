using UnityEngine;
using DamageSystem;

public class StaticParryingObject : MonoBehaviour
{
    [SerializeField] private DamageKey _damageKey;
    [SerializeField] private bool _countParry = false;

    public DamageKey KeyDamage => _damageKey;
    private int _parriedObjects = 0;
    private bool _sendedMsg;

    protected void OnEnable()
    {
        _parriedObjects = 0;
        _sendedMsg = false;
    }

    public virtual void OnTriggerEnter2D(Collider2D thing)
    {
        Bullet cathedBullet = thing.GetComponent<Bullet>();

        if (cathedBullet == null)
            return;

        if (cathedBullet.KeyDamage != KeyDamage)
            return;


        cathedBullet.Parrying();

        if (!_countParry)
            return;

        ParryingHandler.Parry();
        _parriedObjects ++;

        if (!_sendedMsg && (_parriedObjects >= ShipStats.GetIntValue("BulletsForParry")))
        {
            ParryingHandler.BuffedParry();
            _sendedMsg = true;
        }
        //canHeal = false;
    }

    // protected override void SetDefaultStats()
    // {
    //     canHeal = true;
    // }
}
