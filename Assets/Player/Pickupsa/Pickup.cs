using UnityEngine;

[RequireComponent(typeof(DeathCaller))]
public class Pickup : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _lifeTime;

    private float _windAngle;
    private float _lifeTimer = 0f;

    private void Start() {
        _windAngle = GameObject.FindWithTag("Level core").GetComponent<EnvironmentSpawner>().WindAngle + Random.Range(150f, 210f);
    }

    private void Update() 
    {
        transform.position += Quaternion.Euler(0, 0, _windAngle) * Vector3.up * _speed * Time.deltaTime;

        _lifeTimer += Time.deltaTime;

        if (_lifeTimer > _lifeTime)
        {
            Destroying();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player"))
            return;

        Picked();
    }

    protected virtual void Picked()
    {
        Destroying();
    }

    protected virtual void Destroying()
    {
        GetComponent<DeathCaller>().DeathExplosion();
        Destroy(gameObject);
    }
}
