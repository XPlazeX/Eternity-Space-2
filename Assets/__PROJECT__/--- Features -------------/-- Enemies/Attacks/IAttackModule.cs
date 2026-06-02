using UnityEngine;

public abstract class AttackingModule : MonoBehaviour
{
    public abstract void LocalMultiplyAggro(float multiplier);
    public abstract void HandFire(bool volley = false);

    public Vector3 FixedDeltaPosition {get; private set;} = Vector3.zero;
    private Vector3 _previousFixedPosition;

    void FixedUpdate()
    {
        FixedDeltaPosition = transform.position - _previousFixedPosition;

        _previousFixedPosition = transform.position;
    }
}
