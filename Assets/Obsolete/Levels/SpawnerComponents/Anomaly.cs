using UnityEngine;

public class Anomaly : MonoBehaviour
{
    [SerializeField] private float _stepTime;
    [SerializeField] private Vector2 _minMaxWindRotation = new Vector2(0, 360f);

    private EnvironmentSpawner _environmentSpawner;
    private float _timer;

    private void Start() {
        _environmentSpawner = GameObject.FindWithTag("Level core").GetComponent<EnvironmentSpawner>();
    }

    private void Update() {
        _timer -= Time.deltaTime;

        if (_timer < 0f)
        {
            _environmentSpawner.WindAngle = Random.Range(_minMaxWindRotation.x, _minMaxWindRotation.y);
            _timer = _stepTime;
        }
    }
}
