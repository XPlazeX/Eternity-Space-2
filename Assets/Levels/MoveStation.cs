using UnityEngine;

public class MoveStation : MonoBehaviour
{
    [SerializeField] private float _defaultSpeed;
    [SerializeField] private bool _unscaledTime;
    //[SerializeField] private float _defaultStep;
    //private float _speed;

    // private float _stepSpawn = 15f;

    // private void Start() {
    //     if (_defaultSpeed != 0)
    //     {
    //         Initialize(_defaultSpeed, _defaultStep);
    //     }
    // }

    // public void Initialize(float speed, float step = 15f, bool secondaryLoad = false)
    // {
    //     _speed = speed;
    //     _stepSpawn = step;

    //     if (secondaryLoad)
    //         return;
        
    //     for (int i = 0; i < 2; i++)
    //     {
    //         MoveStation secStation = Instantiate(this.gameObject, new Vector3(Random.Range(-1.25f, 1.25f), _stepSpawn * (i + 1), 0), Quaternion.Euler(0, 0, Random.Range(-50f, 50f))).GetComponent<MoveStation>();
    //         secStation.Initialize(_speed, _stepSpawn, true);
    //     }
    // }

    private void FixedUpdate() {
        if (_unscaledTime)
        {
            transform.position += Vector3.down * _defaultSpeed * Time.unscaledDeltaTime;
        }
        else
        {
            transform.position += Vector3.down * _defaultSpeed * Time.deltaTime;
        }
        // if (transform.position.y < -_stepSpawn)
        //     transform.position = new Vector3(Random.Range(-2f, 2f), _stepSpawn * 2f, 0);
    }
}
