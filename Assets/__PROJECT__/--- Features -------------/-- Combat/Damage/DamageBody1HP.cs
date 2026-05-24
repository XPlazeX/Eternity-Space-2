using UnityEngine;
using DamageSystem;

public class DamageBody1HP : DamageBody
{
    public override event deathHandler Deathed;
    public override event bodyPositionHandler PositionDeathed;

    [SerializeField] private MonoBehaviour[] _disablingBehavioursOnDeath;
    [SerializeField] private MonoBehaviour[] _destroyingBehavioursOnDeath;
    [SerializeField] private GameObject[] _togglingObjectssOnDeath;

    protected override void Death(DamageBundle sourceBundle, int overdmg)
    {
        _damageKey = DamageSystem.DamageKey.Unvulnerable;
        HitPoints = 1;

        //base.Death();

        Deathed?.Invoke();
        PositionDeathed?.Invoke(transform.position);

        // if (GetComponent<PullableObject>())
        //     gameObject.SetActive(false);
        // else 
        // {
        //     Destroy(gameObject);
        //     //_deathed = true;
        // }

        for (int i = 0; i < _disablingBehavioursOnDeath.Length; i++)
        {
            _disablingBehavioursOnDeath[i].enabled = false;
        }

        for (int i = 0; i < _destroyingBehavioursOnDeath.Length; i++)
        {
            Destroy(_destroyingBehavioursOnDeath[i]);
        }

        for (int i = 0; i < _togglingObjectssOnDeath.Length; i++)
        {
            _togglingObjectssOnDeath[i].SetActive(!_togglingObjectssOnDeath[i].activeInHierarchy);
        }

        // _deathCaller.DeathExplosion();
    }
}
