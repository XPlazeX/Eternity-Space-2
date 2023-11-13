using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(_ExplosionBullet))]
public class DestroyOnManyDamage : MonoBehaviour
{
    [SerializeField] private int _damageValue;

    private void OnEnable() {
        PlayerShipData.TakeHealthDamage += CheckDetonate;
    }

    private void OnDisable() {
        PlayerShipData.TakeHealthDamage -= CheckDetonate;
    }

    private void CheckDetonate(int val)
    {
        if (val >= _damageValue)
            Detonate();
    }

    private void Detonate()
    {
        GetComponent<_ExplosionBullet>().SpawnExplosion();
        Destroy(gameObject);
    }
}
