using UnityEngine;

public class EnemyAIRoot : MonoBehaviour
{
    public const float level_borders_moving_offset = 3f;

    protected enum LookingOrientation 
    {
        Fixed,
        RotateToPlayer,
        RotateToTarget
    }

    [SerializeField] protected bool _autoStart = true;
    [SerializeField] private float _speed;
    [SerializeField] protected LookingOrientation _orientation;
    [SerializeField] protected float _rotationSpeed;
    [SerializeField] protected AnimationCurve _movingProgression;

    private float _startSpeed;
    private float _mobility = 1f;
    private float _localMobility = 1f;
    protected Vector3 _targetPosition;
    protected Transform _player;

    public float Speed => _speed;
    public float Mobility => _mobility * _localMobility;
    public bool Active {get; private set;} = false;

    private void OnEnable() 
    {
        _startSpeed = _speed;
        _speed *= Random.Range(0.95f, 1.05f);

        Player.PlayerChanged += FindPlayer;
        FindPlayer();

        ShipStats.StatChanged += ObserveStat;
        _mobility = ShipStats.GetValue("EnemyMobilityMultiplier");
    }

    public void LocalMultiplyMobility(float multiplier)
    {
        _localMobility *= multiplier;
    }

    public void Reload()
    {
        OnDisable();
        OnEnable();
    }

    private void ObserveStat(string name, float val)
    {
        if (name == "EnemyMobilityMultiplier")
            _mobility = ShipStats.GetValue("EnemyMobilityMultiplier");
    }

    protected virtual void Start() {
        if (_autoStart)
            StartMoving(); 
    }

    protected virtual void FixedUpdate()
    {
        if (!Active)
            return;

        if (_orientation == LookingOrientation.RotateToPlayer)
            RotateToPlayer();
        else if (_orientation == LookingOrientation.RotateToTarget)
            RotateToTarget();

        DoMove();
    }

    protected virtual void DoMove() {}

    public virtual void StartMoving() => Active = true;

    public virtual void StopMoving() => Active = false;

    private void OnDisable() 
    {
        Player.PlayerChanged -= FindPlayer;
        ShipStats.StatChanged -= ObserveStat;
    }

    protected void RotateToTarget()
    {
        transform.up = SceneStatics.FlatVector(Vector3.RotateTowards(transform.up, (_targetPosition - transform.position), _rotationSpeed * Time.deltaTime * (Speed / _startSpeed) * Mobility, 0f));

        CorrectRotation();
    }

    protected void RotateToPlayer()
    {
        if (_player != null)
                transform.up = SceneStatics.FlatVector(Vector3.RotateTowards(transform.up, (_player.position - transform.position), _rotationSpeed * Time.deltaTime * (Speed / _startSpeed) * Mobility, 0f));

        CorrectRotation();
    }

    protected virtual void CorrectRotation()
    {
        if (transform.rotation.eulerAngles.y != 180 && transform.rotation .eulerAngles.y != -180)
            return;

        transform.rotation = Quaternion.Euler(0, 0, 180);
    }

    public virtual void FindPlayer() => _player = Player.PlayerTransform;
}
