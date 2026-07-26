using DamageSystem;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class CountermeasuresAIBlock : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyAIRoot enemyAI;
    [SerializeField] private Transform reactionCenter;

    [Header("Projectile Reaction")]
    [SerializeField] private bool reactToProjectiles = true;
    [SerializeField] private float projectileCooldown = 1.2f;
    [SerializeField] private float projectileImpulseForce = 24f;
    [SerializeField] private float projectileImpulseDuration = 0.08f;
    [Tooltip("Минимальная дистанция по X от линии стрельбы игрока. Если враг почти на линии, будет fallback.")]
    [SerializeField] private float projectileLineDeadZone = 0.15f;

    [Tooltip("Пуля должна лететь примерно в сторону врага. Чем выше значение, тем строже проверка.")]
    [Range(-1f, 1f)]
    [SerializeField] private float projectileIncomingDot = 0.35f;

    [Tooltip("Добавка по направлению движения снаряда. Даёт уклонение не строго вбок, а слегка по диагонали.")]
    [Range(-1f, 1f)]
    [SerializeField] private float projectileForwardBias = 0.25f;

    [Tooltip("Случайный разброс направления уклонения.")]
    [Range(0f, 1f)]
    [SerializeField] private float projectileDodgeRandomness = 0.25f;

    [Header("Player Retreat")]
    [SerializeField] private bool retreatFromPlayer = true;
    [SerializeField] private float playerCheckInterval = 0.1f;
    [SerializeField] private float playerRetreatDistance = 5f;
    [SerializeField] private float playerRetreatCooldown = 2.5f;
    [SerializeField] private float playerRetreatForce = 36f;
    [SerializeField] private float playerRetreatDuration = 0.16f;

    [Tooltip("Разброс направления отступления от игрока.")]
    [Range(0f, 1f)]
    [SerializeField] private float playerRetreatRandomness = 0.15f;

    [Header("Debug")]
    [SerializeField] private bool drawDebug;

    private float _projectileTimer;
    private float _playerRetreatTimer;
    private float _playerCheckTimer;

    private Transform _player;

    private void Reset()
    {
        enemyAI = GetComponentInParent<EnemyAIRoot>();
        reactionCenter = transform;
    }

    private void Awake()
    {
        if (enemyAI == null)
            enemyAI = GetComponentInParent<EnemyAIRoot>();

        if (reactionCenter == null)
            reactionCenter = transform;
    }

    private void OnEnable()
    {
        Player.PlayerChanged += FindPlayer;
        FindPlayer();
    }

    private void OnDisable()
    {
        Player.PlayerChanged -= FindPlayer;
    }

    private void FixedUpdate()
    {
        float dt = ESTime.worldFixedDeltaTime;

        if (_projectileTimer > 0f)
            _projectileTimer -= dt;

        if (_playerRetreatTimer > 0f)
            _playerRetreatTimer -= dt;

        if (_playerCheckTimer > 0f)
            _playerCheckTimer -= dt;

        if (_playerCheckTimer <= 0f)
        {
            _playerCheckTimer = playerCheckInterval;
            TryRetreatFromPlayer();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!reactToProjectiles)
            return;

        if (_projectileTimer > 0f)
            return;

        if (enemyAI == null)
            return;

        if (!other.TryGetComponent(out AttackObject attackObject))
            return;

        TryDodgeProjectile(attackObject);
    }

    private void TryDodgeProjectile(AttackObject attackObject)
    {
        Vector2 center = GetCenterPosition();
        Vector2 projectilePosition = attackObject.transform.position;

        Vector2 toEnemy = center - projectilePosition;

        if (toEnemy.sqrMagnitude <= 0.0001f)
            return;

        Vector2 projectileForward = attackObject.transform.up.normalized;

        float incomingDot = Vector2.Dot(projectileForward, toEnemy.normalized);

        if (incomingDot < projectileIncomingDot)
            return;

        Vector2 dodgeDirection = BuildProjectileDodgeDirection(projectileForward, projectilePosition);

        bool accepted = enemyAI.Accelerate(
            dodgeDirection,
            projectileImpulseForce,
            projectileImpulseDuration,
            resetCurrentAcceleration: true
        );

        if (accepted)
            _projectileTimer = projectileCooldown;
    }

    private Vector2 BuildProjectileDodgeDirection(Vector2 projectileForward, Vector2 projectilePosition)
    {
        Vector2 center = GetCenterPosition();

        Vector2 sideDirection = BuildSideAwayFromPlayerFireLine(center, projectilePosition);

        Vector2 random = Random.insideUnitCircle * projectileDodgeRandomness;

        Vector2 direction =
            sideDirection +
            projectileForward.normalized * projectileForwardBias +
            random;

        if (direction.sqrMagnitude <= 0.0001f)
            direction = sideDirection;

        return direction.normalized;
    }

    private Vector2 BuildSideAwayFromPlayerFireLine(Vector2 enemyCenter, Vector2 projectilePosition)
    {
        if (_player == null)
        {
            // fallback: от линии самого снаряда
            float projectileSide = enemyCenter.x - projectilePosition.x;

            if (Mathf.Abs(projectileSide) > projectileLineDeadZone)
                return projectileSide >= 0f ? Vector2.right : Vector2.left;

            return Random.value < 0.5f ? Vector2.left : Vector2.right;
        }

        float side = enemyCenter.x - _player.position.x;

        if (Mathf.Abs(side) > projectileLineDeadZone)
            return side >= 0f ? Vector2.right : Vector2.left;

        // Если враг почти ровно над игроком, отходим от текущего X снаряда.
        float projectileSideFromEnemy = enemyCenter.x - projectilePosition.x;

        if (Mathf.Abs(projectileSideFromEnemy) > projectileLineDeadZone)
            return projectileSideFromEnemy >= 0f ? Vector2.right : Vector2.left;

        // Совсем редкий случай: всё на одной линии.
        return Random.value < 0.5f ? Vector2.left : Vector2.right;
    }

    private void TryRetreatFromPlayer()
    {
        if (!retreatFromPlayer)
            return;

        if (_playerRetreatTimer > 0f)
            return;

        if (enemyAI == null || _player == null)
            return;

        Vector2 center = GetCenterPosition();
        Vector2 toEnemyFromPlayer = center - (Vector2)_player.position;

        float sqrDistance = toEnemyFromPlayer.sqrMagnitude;
        float sqrRetreatDistance = playerRetreatDistance * playerRetreatDistance;

        if (sqrDistance > sqrRetreatDistance)
            return;

        if (toEnemyFromPlayer.sqrMagnitude <= 0.0001f)
            toEnemyFromPlayer = Random.insideUnitCircle.normalized;

        Vector2 retreatDirection =
            toEnemyFromPlayer.normalized +
            Random.insideUnitCircle * playerRetreatRandomness;

        if (retreatDirection.sqrMagnitude <= 0.0001f)
            retreatDirection = toEnemyFromPlayer.normalized;

        bool accepted = enemyAI.Accelerate(
            retreatDirection.normalized,
            playerRetreatForce,
            playerRetreatDuration,
            resetCurrentAcceleration: true
        );

        if (accepted)
            _playerRetreatTimer = playerRetreatCooldown;
    }

    private Vector2 GetCenterPosition()
    {
        if (reactionCenter != null)
            return reactionCenter.position;

        if (enemyAI != null)
            return enemyAI.AiPosition;

        return transform.position;
    }

    private void FindPlayer()
    {
        _player = Player.PlayerTransform;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!drawDebug)
            return;

        Vector3 center = reactionCenter != null ? reactionCenter.position : transform.position;

        Gizmos.DrawWireSphere(center, playerRetreatDistance);
    }
#endif
}