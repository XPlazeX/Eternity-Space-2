using UnityEngine;

[RequireComponent(typeof(LaserObject))]
public class LaserImpactDealer : ImpactDealer
{
    private LaserObject _laser;

    void OnEnable()
    {
        if (_laser == null)
            _laser = GetComponent<LaserObject>();

        _laser.Collided += OnCollided;
    }

    void OnDisable()
    {
        if (_laser != null)
            _laser.Collided -= OnCollided;
    }

    private void OnCollided(GameObject other)
    {
        IImpactReceiver impactReceiver = other.GetComponentInParent<IImpactReceiver>();

        if (impactReceiver == null)
            return;

        DealImpact(impactReceiver, new ImpactContext
        {
            Source = gameObject,
            Instigator = gameObject,
            Point = other.transform.position,
            Direction = (other.transform.position - transform.position).normalized,
            Impacts = base.Impacts
        });
    }
}
