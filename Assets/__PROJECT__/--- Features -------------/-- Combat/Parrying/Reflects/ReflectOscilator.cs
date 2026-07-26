using UnityEngine;

public class ReflectOscilator : MonoBehaviour
{
    [SerializeField] private PullableObject _triggeringObject;
    [SerializeField] private Transform _spawnPivot;
    [SerializeField] private float _reloadTime;

    private PullForObjects ObjectPool;
    private bool _workable;
    private float _timer;

    private void Start() {
        ObjectPool = new PullForObjects(_triggeringObject);
        _timer = _reloadTime;
    }

    private void Update() {
        if (_timer <= 0f)
        {
            SpawnProjectile();
            _timer = _reloadTime;
        }
        _timer -= ESTime.worldDeltaTime;
    }

    public void ChangeTriggeringObject(PullableObject newObject)
    {
        ObjectPool.SampleChanged();
        ObjectPool = new PullForObjects(newObject);
    }

    public void SpawnProjectile()
    {
        GameObject projectile = ObjectPool.GetGameObject();

        projectile.transform.position = _spawnPivot.position;
    }
}
