using UnityEngine;

public class VectorErrorer : MonoBehaviour
{
    [SerializeField] private float _timeToError;
    [SerializeField] private bool _critical;

    private float _timer;
    private bool _triggered;
    private bool _stay;
    private PlayerUI _playerUI;

    private void Start() {
        _timer = _timeToError;
        _playerUI = SceneStatics.UICore.GetComponent<PlayerUI>();
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

        if (!_critical)
        {
            SceneStatics.UICore.GetComponent<DeathUIHandler>().ErrorVector();
        } else
        {
            
        }

        _triggered = true;
        _playerUI.ToggleIntervention(false);
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (!other.CompareTag("Player"))
            return;

        _timer = _timeToError;
        _stay = false;
        _playerUI.ToggleIntervention(false);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player"))
            return;
            
        if (_triggered)
            return;

        _stay = true;
        _playerUI.ToggleIntervention(true);
    }
}
