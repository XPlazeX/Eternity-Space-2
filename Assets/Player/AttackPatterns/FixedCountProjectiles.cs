using System.Collections;
using UnityEngine;
using DamageSystem;

public class FixedCountProjectiles : AttackPattern
{
    [Header("AttackObject must have 'BindingObject' component!")]
    [SerializeField] private int _maxProjectileCount;
    [SerializeField] private Vector3 _barrelOffset;

    private BindingObject[] _activeObjects;

    public override void Load()
    {
        base.Load();
        _activeObjects = new BindingObject[_maxProjectileCount];
    }
    
    public override void Fire()
    {
        for (int i = 0; i < _activeObjects.Length; i++)
        {
            if (_activeObjects[i] != null && _activeObjects[i].gameObject.activeSelf == true)
                continue;

            Fired?.Invoke();

            BindingObject bindingObject = SpawnBullet(_barrels[0].position, 0).GetComponent<BindingObject>();

            _activeObjects[i] = bindingObject;
            bindingObject.Bind(Player.PlayerTransform);
        }
    }

    protected AttackObject SpawnBullet(Vector3 position, float startRotation)
    {
        AttackObject bulletSample = CharacterBulletDatabase.GetAttackObject(_bulletIndex);

        bulletSample.transform.rotation = Quaternion.Euler(0, 0, startRotation + Random.Range(-_spread, _spread));
        bulletSample.transform.position = position + _barrelOffset;

        if (_spreadBulletSpeed != 0)
            ((Bullet)bulletSample).MultiplySpeedParams(1f + (Random.Range(-_spreadBulletSpeed, _spreadBulletSpeed)));

        return bulletSample;
    }
}
