using UnityEngine;

[RequireComponent(typeof(StaticBullet))]
public class StaticBulletImpactDealer : ImpactDealer
{
    private StaticBullet _staticBullet;

    void OnEnable()
    {
        if (_staticBullet == null)
            _staticBullet = GetComponent<StaticBullet>();

        _staticBullet.Collided += OnCollided;
    }

    void OnDisable()
    {
        if (_staticBullet != null)
            _staticBullet.Collided -= OnCollided;
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
