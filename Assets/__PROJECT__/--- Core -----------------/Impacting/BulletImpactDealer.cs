using UnityEngine;

[RequireComponent(typeof(Bullet))]
public class BulletImpactDealer : ImpactDealer
{
    private Bullet _bullet;

    void OnEnable()
    {
        if (_bullet == null)
            _bullet = GetComponent<Bullet>();

        _bullet.Collided += OnCollided;
    }

    void OnDisable()
    {
        if (_bullet != null)
            _bullet.Collided -= OnCollided;
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
