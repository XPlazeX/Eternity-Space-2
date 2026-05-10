using UnityEngine;

public abstract class AttackingModule : MonoBehaviour
{
    public abstract void LocalMultiplyAggro(float multiplier);
    public abstract void HandFire(bool volley = false);
}
