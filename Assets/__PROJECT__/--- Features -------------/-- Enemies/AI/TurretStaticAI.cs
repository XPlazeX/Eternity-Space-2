using UnityEngine;

public class TurretStaticAI : MonoBehaviour
{
    [SerializeField] protected float _rotationSpeed;
    [SerializeField] protected float _foresight;

    protected Transform _player;
    protected float _mobility = 1f;
    protected bool _useForesight = true;

    private void Update() {
        if (_player != null)
        {
            transform.up = SceneStatics.FlatVector(Vector3.RotateTowards(transform.up, ((_useForesight ? Player.GetPlayerPosition(_foresight) : _player.position) - transform.position), _rotationSpeed * ESTime.worldDeltaTime * _mobility, 0f));
            transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z);
        }

        else
            FindPlayer();
    }

    private void OnEnable() 
    {
        Player.PlayerChanged += FindPlayer;
        FindPlayer();

        ShipStats.StatChanged += ObserveStat;
        _mobility = ShipStats.GetValue("EnemyMobilityMultiplier");
    }

    private void OnDisable() {
        ShipStats.StatChanged -= ObserveStat;
    }

    private void ObserveStat(string name, float val)
    {
        if (name == "EnemyMobilityMultiplier")
            _mobility = ShipStats.GetValue("EnemyMobilityMultiplier");
    }

    public virtual void FindPlayer() => _player = Player.PlayerTransform;
}
