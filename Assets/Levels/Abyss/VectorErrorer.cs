using UnityEngine;

public class VectorErrorer : MonoBehaviour
{
    [SerializeField] private float _timeToError;

    private float _timer;
    private bool _triggered;
    private bool _stay;

    private void Start() {
        _timer = _timeToError;
    }

    private void Update() {
        if (!_stay)
            return;

        _timer -= Time.deltaTime;
        //print(_timer);

        if (_timer <= 0f)
        {
            Error();
        }
    }

    public void Error()
    {
        SceneStatics.UICore.GetComponent<PlayerUI>().TriggerPreDeath();
        SceneStatics.UICore.GetComponent<DeathUIHandler>().ErrorVector();

        _triggered = true;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (!other.CompareTag("Player"))
            return;

        _timer = _timeToError;
        _stay = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player"))
            return;
            
        if (_triggered)
            return;

        _stay = true;
    }
}
