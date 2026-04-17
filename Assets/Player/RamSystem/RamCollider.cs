using UnityEngine;

public class RamCollider : MonoBehaviour
{
    private PlayerDamageBody _playerDamageBody;

    void Start()
    {
        _playerDamageBody = GetComponentInParent<PlayerDamageBody>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (_playerDamageBody == null) return;

        DamageBody damageBody = other.GetComponent<DamageBody>();

        if (damageBody == null)
            return;

        if (damageBody.RamReady && (damageBody.GetType() != typeof(AsteroidBody)))
        {
            _playerDamageBody.RamDamageBody(damageBody);
        }
    }
}
