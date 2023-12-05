using UnityEngine;

[RequireComponent(typeof(DeathCaller))]
public class AsteroidBody : DamageBody
{
    [Tooltip("Если меньше 0, то не уничтожается через время")][SerializeField] protected float _lifetime = -1f;
    [SerializeField] private Vector2 _minMaxSpeed;
    [SerializeField] private bool _explodeOnTimer;

    private float _lifeTimer;
    private float _selectedSpeed;
    private Vector3 _direction;

    protected override void Awake() {
        base.Awake();
        OneShotProtection = false;
    }

    private void OnEnable() {
        _lifeTimer = _lifetime * ((Mathf.Abs(CameraController.Borders_xXyY.z) + CameraController.Borders_xXyY.w + (CameraController.Size * 2f)) / (CameraController.Size * 2f));
        _selectedSpeed = Random.Range(_minMaxSpeed.x, _minMaxSpeed.y);
        HitPoints = _startHP;
        _direction = transform.up;
        //print($"asteroid HP : {HitPoints}");
    }

    private void FixedUpdate() {
        transform.position += _direction * _selectedSpeed * Time.deltaTime;

        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer > 0)
            return;

        if (_explodeOnTimer)
            TakeDamage(999);
            
        gameObject.SetActive(false);
    }
}
