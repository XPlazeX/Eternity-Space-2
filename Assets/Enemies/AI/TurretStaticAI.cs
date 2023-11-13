using UnityEngine;

public class TurretStaticAI : MonoBehaviour
{
    [SerializeField] protected float _rotationSpeed;

    protected Transform _player;
    protected float _mobility = 1f;

    private void Update() {
        if (_player != null)
            transform.up = SceneStatics.FlatVector(Vector3.RotateTowards(transform.up, (_player.position - transform.position), _rotationSpeed * Time.deltaTime * _mobility, 0f));

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
