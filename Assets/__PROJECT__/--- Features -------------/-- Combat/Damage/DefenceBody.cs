using UnityEngine;
using DamageSystem;

public class DefenceBody : DamageBody
{

    protected override void Death()
    {

    }

    public override bool TakeDamage(DamageBundle damageBundle, out bool killed)
    {
        killed = false;
        return false;
    }


}
