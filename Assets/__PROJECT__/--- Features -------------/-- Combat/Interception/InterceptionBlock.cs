using DamageSystem;
using UnityEngine;

public class InterceptionBlock : MonoBehaviour
{
    [SerializeField] private Collider2D parryCollider;
    [SerializeField] private Transform parryOrigin;
    // [SerializeField] private DamageKey interceptingDamage;

    [Header("Arc")]
    [SerializeField] private float radius = 2.5f;
    [SerializeField] private float arcHalfAngle = 45f;

    [Header("Turrets")]
    [SerializeField] private Transform[] lookingTransforms;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private AttackingModule[] attackingModules;
    [SerializeField] private float turretReloadTime = 10f;

    [Header("Explosions")]
    [SerializeField] private ExplosionRepeater[] explosionRepeaters;
    [SerializeField] private float explosionsReloadTime = 10f;

    [Header("State")]
    [SerializeField] private bool active;

    private float _turretsTimer = 0f;
    private float _explosionsTimer = 0f;
    private float _minDot;

    private void Awake()
    {
        _minDot = Mathf.Cos(arcHalfAngle * Mathf.Deg2Rad);

        if (parryCollider != null)
            parryCollider.enabled = false;
    }

    private void FixedUpdate()
    {
        if (_turretsTimer > 0f)
            _turretsTimer -= ESTime.worldFixedDeltaTime;

        if (_explosionsTimer > 0f)
            _explosionsTimer -= ESTime.worldFixedDeltaTime;
    }

    public void SetActive(bool value)
    {
        active = value;

        if (parryCollider != null)
            parryCollider.enabled = value;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!active)
            return;

        if (!other.TryGetComponent(out AttackObject attackObject))
            return;

        Vector3 origin = parryOrigin.position;
        Vector3 toBullet = attackObject.transform.position - origin;

        if (toBullet.sqrMagnitude > radius * radius)
            return;

        Vector3 forward = parryOrigin.up;
        float dot = Vector3.Dot(forward, toBullet.normalized);

        if (dot < _minDot)
            return;

        // Пуля должна лететь примерно в сторону блока, а не от него.
        Vector3 bulletVelocity = attackObject.transform.up.normalized;
        Vector3 toOrigin = -toBullet.normalized;

        float incomingDot = Vector3.Dot(bulletVelocity, toOrigin);

        if (incomingDot < 0.25f)
            return;

        HandleInterceptedBullet(attackObject, toBullet);
    }

    private void HandleInterceptedBullet(AttackObject attackObject, Vector3 toBullet)
    {
        RotateTurretsToDirection(toBullet);

        if (_turretsTimer <= 0f)
        {
            FireTurrets();
            _turretsTimer = turretReloadTime;
        }

        if (_explosionsTimer <= 0f)
        {
            ActivateExplosions();
            _explosionsTimer = explosionsReloadTime;
        }
    }

    private void RotateTurretsToDirection(Vector3 direction)
    {
        if (lookingTransforms == null || lookingTransforms.Length == 0)
            return;

        if (direction.sqrMagnitude <= 0.0001f)
            return;

        Vector3 targetDirection = direction.normalized;

        for (int i = 0; i < lookingTransforms.Length; i++)
        {
            Transform lookingTransform = lookingTransforms[i];

            if (lookingTransform == null)
                continue;

            Vector3 newUp = Vector3.RotateTowards(
                lookingTransform.up,
                targetDirection,
                rotationSpeed * Mathf.Deg2Rad * ESTime.worldFixedDeltaTime,
                0f
            );

            lookingTransform.up = newUp;
        }
    }

    private void FireTurrets()
    {
        if (attackingModules == null)
            return;

        for (int i = 0; i < attackingModules.Length; i++)
        {
            AttackingModule attackingModule = attackingModules[i];

            if (attackingModule == null)
                continue;

            attackingModule.HandFire(true);
        }
    }

    private void ActivateExplosions()
    {
        if (explosionRepeaters == null)
            return;

        for (int i = 0; i < explosionRepeaters.Length; i++)
        {
            ExplosionRepeater explosionRepeater = explosionRepeaters[i];

            if (explosionRepeater == null)
                continue;

            explosionRepeater.Activate();
        }
    }
}