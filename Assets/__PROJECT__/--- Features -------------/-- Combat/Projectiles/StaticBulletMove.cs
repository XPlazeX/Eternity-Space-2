using UnityEngine;

public class StaticBulletMove : MonoBehaviour
{
    [SerializeField] protected float _speed;
    [SerializeField] protected float _acceleration;
    [SerializeField] private bool _accelerateToZero = false;
    [SerializeField] private bool _explodeOnTimer = false;
    [SerializeField] private _ExplosionBullet _explosionBullet;
    [SerializeField] private float _lifetime;

    private float _lifeTimer;
    private float _startSpeed;
    private float _startAcceleration;

    private void OnEnable() 
    {
        _startSpeed = _speed;
        _startAcceleration = _acceleration;

        _speed = _startSpeed;
        _lifeTimer = _lifetime;
    }

    private void Update() 
    {      
        if (!(_accelerateToZero && (Mathf.Abs(_speed) < 0.1f)))
            _speed += _acceleration * ESTime.worldDeltaTime;

        transform.position += ((transform.up * _speed) + (PlayerController.DefaultForce)) * ESTime.worldDeltaTime;

        if (!_explodeOnTimer)
            return;

        _lifeTimer -= ESTime.worldDeltaTime;
        if (_lifeTimer <= 0)
            Death();
    }

    protected virtual void Death()
    {
        if (_explodeOnTimer)
            _explosionBullet.SpawnExplosion();

        Destroy(gameObject);
    }
}
