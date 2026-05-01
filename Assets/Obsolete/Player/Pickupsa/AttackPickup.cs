using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPickup : Pickup
{
    [SerializeField] private FieldAttackModule.FieldAttackObject _attackObject;

    protected override void Picked()
    {
        _attackObject.Fire();
        base.Picked();
    }
}
