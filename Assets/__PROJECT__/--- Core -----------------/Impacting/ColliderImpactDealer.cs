using UnityEngine;

public class ColliderImpactDealer : ImpactDealer
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        IImpactReceiver impactReceiver = other.GetComponentInParent<IImpactReceiver>();

        if (impactReceiver == null)
            return;

        DealImpact(impactReceiver, new ImpactContext
        {
            Source = gameObject,
            Instigator = gameObject,
            Point = other.ClosestPoint(transform.position),
            Direction = (other.transform.position - transform.position).normalized,
            Impacts = base.Impacts
        });
    }
}
