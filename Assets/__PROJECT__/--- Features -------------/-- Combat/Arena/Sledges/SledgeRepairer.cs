using UnityEngine;

public class SledgeRepairer : MonoBehaviour
{
    // [SerializeField] private SledgeCore sledgeCore;
    [SerializeField] private Transform pivot;
    [SerializeField] private float connectionRadius = 2f;
    [SerializeField] private float connectionTime = 3f;
    [SerializeField] private float angleThreshold = 1f;

    [Header("Rule Behaviour")]
    [SerializeField] private GameObject servicingIndicator;
    [SerializeField] private GameObject unactiveIndicator;

    [Header("Gun Behaviour")]
    [SerializeField] private Transform tower;
    [SerializeField] private float towerRotationSpeed = 2f;
    [SerializeField] private Transform barrel;
    [SerializeField] private float cooldown = 0.4f;

    [Header("Heal Attack")]
    [SerializeField] protected DamageSystem.AttackObject _bulletSample;
    [SerializeField] protected AudioClip _soundWork;
    [SerializeField][Range(0, 1f)] private float _volume = 1f;
    [SerializeField][Range(0, 2f)] private float _startPitch = 1f;
    [SerializeField][Range(0, 3f)] private float _pitchSpread = 0f;
    [SerializeField] private _ExplosionBullet muzzleExplosion;

    [Header("Visual")]
    [SerializeField] private CircleLineRenderer radiusRenderer;
    [SerializeField] private Transform leftArm;
    [SerializeField] private Transform rightArm;
    [SerializeField] private float topArmAngle = 45f;
    [SerializeField] private LineRenderer leftLR;
    [SerializeField] private LineRenderer rightLR;
    [SerializeField] private Transform left2Arm;
    [SerializeField] private Transform right2Arm;
    [SerializeField] private float top2ArmAngle = 45f;
    [SerializeField] private LineRenderer left2LR;
    [SerializeField] private LineRenderer right2LR;
    [SerializeField] private Gradient armConnectionGradient;
    [SerializeField] private Gradient circleConnectionGradient;
    [SerializeField] private Color resetArmColor;
    [SerializeField] private Color resetCircleConnectionColor;
    [Header("Bow")]
    [SerializeField] private Transform leftBower;
    [SerializeField] private Transform rightBower;
    [SerializeField] private float topBowersAngle = 60f;
    [SerializeField] private Transform[] bowerConnectors;
    [SerializeField] private AnimationCurve bowersYProgression;
    [SerializeField] private Transform boltConnector;
    [SerializeField] private AnimationCurve boltYProgression;
    [SerializeField] private float connectorsResetSpeedMultiplier = 2f;
    [SerializeField] private Transform leftRotor;
    [SerializeField] private Transform rightRotor;
    [SerializeField] private float rotorsTopAngle = 360f;
    [SerializeField] private SpriteRenderer boltSpriteRenderer;
    [SerializeField] private Gradient boltCooldownGradient;
    [SerializeField] private float bowResetSpeed = 3f;

    private bool _active = true;
    private float _connectionProgress = 0f;
    private float _bowProgress = 0f;
    private bool _connected = false;
    private float _cooldownTimer = 0f;
    private bool _servicingPlayer;

    public float ConnectionProgress => _connectionProgress;

    private void OnEnable() 
    {
        PlayerShipData.HealthChanged += OnPlayerHealthChanged;
        OnPlayerHealthChanged(0);
    }

    void OnDisable()
    {
        PlayerShipData.HealthChanged -= OnPlayerHealthChanged;
    }

    private void Start() 
    {
        boltSpriteRenderer.color = boltCooldownGradient.Evaluate(1f);
        ProgressBow(0f);
        ResetRenderers();
    }

    private void FixedUpdate()
    {
        if (!_active)
        {
            if (_connected)
            {
                LoseConnection();
            }
            return;
        }

        Transform player = Player.PlayerTransform;

        if (player == null) return;
        bool angleCondition = Vector3.Angle(tower.up, player.position - tower.position) <= angleThreshold;

        if (_servicingPlayer && !_connected && (player.position - pivot.position).magnitude <= connectionRadius && angleCondition)
        {
            StartConnection();
        }   
        else if (_connected && ((player.position - pivot.position).magnitude > connectionRadius || !angleCondition || !_servicingPlayer))
        {
            LoseConnection();
        }

        if (_servicingPlayer)
        {
            tower.up = SceneStatics.FlatVector(Vector3.RotateTowards(tower.up, player.position - tower.position, towerRotationSpeed * ESTime.worldFixedDeltaTime, 100f));
        } else
        {
            tower.up = SceneStatics.FlatVector(Vector3.RotateTowards(tower.up, pivot.up, towerRotationSpeed * ESTime.worldFixedDeltaTime, 100f));
        }
    }

    private void Update() 
    {
        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= ESTime.worldDeltaTime;

            float cooldownProgress = Mathf.Clamp01(1f - (_cooldownTimer / cooldown));
            boltSpriteRenderer.color = boltCooldownGradient.Evaluate(cooldownProgress);

            _bowProgress = 1f - cooldownProgress * connectorsResetSpeedMultiplier;
            ProgressBow(_bowProgress);

            return;    
        }
        
        if (!_active) return;

        if (_connected)
        {
            ProgressConnection(ESTime.worldDeltaTime);
        } else
        {
            _bowProgress -= ESTime.worldDeltaTime * bowResetSpeed;
            ProgressBow(_bowProgress);
        }
    }

    private void ProgressConnection(float dt)
    {
        _connectionProgress += dt;

        if (_connectionProgress >= connectionTime)
        {
            ReleaseConnection();
            return;
        }

        ProgressRenderers(_connectionProgress / connectionTime);

        _bowProgress = _connectionProgress / connectionTime;
        ProgressBow(_bowProgress);
    }

    private void StartConnection()
    {
        _connected = true;
    }

    private void LoseConnection()
    {
        _connectionProgress = 0f;
        _connected = false;

        ResetRenderers();
    }

    private void ReleaseConnection()
    {
        Fire();
        ResetRenderers();
        _connectionProgress = 0f;
        _cooldownTimer = cooldown;
    }

    public void Fire()
    {
        DamageSystem.AttackObject bulletSample = Pool.Spawn(_bulletSample);//CharacterBulletDatabase.GetAttackObject(_bulletIndex);

        bulletSample.transform.rotation = barrel.rotation;
        bulletSample.transform.position = barrel.position;

        muzzleExplosion.SpawnExplosion(barrel.position);
        SoundPlayer.PlaySound(_soundWork, _volume, Random.Range(_startPitch - _pitchSpread, _startPitch + _pitchSpread));
    }

    private void OnPlayerHealthChanged(int hp)
    {
        _servicingPlayer = !PlayerShipData.IsFullHP;

        if (_active)
        {
            servicingIndicator.SetActive(_servicingPlayer);
        }

        Debug.Log($"Servicing player: {_servicingPlayer}");
    }

    private void ProgressRenderers(float t)
    {
        t = Mathf.Clamp01(t);

        leftLR.startColor = armConnectionGradient.Evaluate(t);
        rightLR.startColor = armConnectionGradient.Evaluate(t);
        left2LR.startColor = armConnectionGradient.Evaluate(t);
        right2LR.startColor = armConnectionGradient.Evaluate(t);
        radiusRenderer.SetColor(circleConnectionGradient.Evaluate(t));

        leftArm.eulerAngles = new Vector3(0f, 0f, tower.eulerAngles.z + Mathf.Lerp(0f, topArmAngle, 1f - t));
        rightArm.eulerAngles = new Vector3(0f, 0f, tower.eulerAngles.z - Mathf.Lerp(0f, topArmAngle, 1f - t));
        left2Arm.eulerAngles = new Vector3(0f, 0f, tower.eulerAngles.z + Mathf.Lerp(0f, top2ArmAngle, 1f - t));
        right2Arm.eulerAngles = new Vector3(0f, 0f, tower.eulerAngles.z - Mathf.Lerp(0f, top2ArmAngle, 1f - t));
    }

    private void ResetRenderers()
    {
        leftLR.startColor = resetArmColor;
        rightLR.startColor = resetArmColor;
        left2LR.startColor = resetArmColor;
        right2LR.startColor = resetArmColor;
        radiusRenderer.SetColor(resetCircleConnectionColor);

        leftArm.eulerAngles = new Vector3(0f, 0f, topArmAngle);
        rightArm.eulerAngles = new Vector3(0f, 0f, -topArmAngle);
        left2Arm.eulerAngles = new Vector3(0f, 0f, top2ArmAngle);
        right2Arm.eulerAngles = new Vector3(0f, 0f, -top2ArmAngle);

        radiusRenderer.SetRadius(connectionRadius);
    }

    private void ProgressBow(float p)
    {
        leftBower.localRotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Lerp(0f, topBowersAngle, p)));
        rightBower.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -Mathf.Lerp(0f, topBowersAngle, p)));
        for (int i = 0; i < bowerConnectors.Length; i++)
        {
            bowerConnectors[i].localPosition = new Vector3(
                bowerConnectors[i].localPosition.x,
                bowersYProgression.Evaluate(p),
                0f
            );
        }

        boltConnector.localPosition = new Vector3(
            boltConnector.localPosition.x,
            boltYProgression.Evaluate(p),
            0f
        );
        leftRotor.localRotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Lerp(0f, rotorsTopAngle, p)));
        rightRotor.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -Mathf.Lerp(0f, rotorsTopAngle, p)));
    }
}
