using UnityEngine;
using DamageSystem;

public class DefenceBody : DamageBody
{

    protected override void Death()
    {

    }

    public override bool TakeDamage(DamageBundle damageBundle)
    {
        //base.TakeDamage(damage);
        return false;
    }


}
