using UnityEngine;
using UnityEngine.UI;

public class DetonatorArea : MonoBehaviour
{
    [SerializeField] private float _dealy;
    [SerializeField] private Text _countdownLabel;
    [SerializeField] private DeathCaller _deathCaller;
    [SerializeField] private bool _needInside;

    private float _timer;

    private void Start() {
        _timer = _dealy;
    }

    private void Update() 
    {
        _timer -= Time.deltaTime;
        _countdownLabel.text = $"{(int)_timer % 100}.{(int)(_timer * 10) % 10}{(int)(_timer * 100) % 10}";

        if (_timer <= 0f)
        {
            _deathCaller.DeathExplosion();
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (!_needInside)
            return;
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
