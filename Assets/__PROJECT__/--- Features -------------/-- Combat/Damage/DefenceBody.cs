using UnityEngine;
using DamageSystem;

public class DefenceBody : DamageBody
{

    protected override void Death(DamageBundle damageBundle, int overdmg)
    {

    }

    public override bool TakeDamage(DamageBundle damageBundle, out bool killed)
    {
        killed = false;
        return false;
    }


}
