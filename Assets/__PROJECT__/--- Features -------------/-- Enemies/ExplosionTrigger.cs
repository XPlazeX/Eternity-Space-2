using UnityEngine;
using DamageSystem;

public class ExplosionTrigger : MonoBehaviour
{
    [SerializeField] private string _targetTag;
    [SerializeField] private bool _bulletTriggered;
    [SerializeField] private DamageKey _bulletKey;
    [SerializeField] private DeathCaller _explosionCaller;
    [SerializeField] private float _triggerReload;

    private float _timer;

    private void FixedUpdate() {
        _timer -= Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other) {

        if (!_bulletTriggered && other.CompareTag(_targetTag))
        {
            if (_timer < 0f)
            {
                // _explosionCaller.DeathExplosion();
                _timer = _triggerReload;
            }
        }

        if (_bulletTriggered && other.GetComponent<Bullet>() != null && other.GetComponent<Bullet>().KeyDamage == _bulletKey)
        {
            if (_timer < 0f)
            {
                // _explosionCaller.DeathExplosion();
                _timer = _triggerReload;
            }
        }
    }
}
