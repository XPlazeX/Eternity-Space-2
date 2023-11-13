using UnityEngine;

[RequireComponent(typeof(DamageBody))]
public class DamageOverTime : MonoBehaviour
{
    [SerializeField] private float _timeDelay;
    [SerializeField] private int _damageValue;

    private float _timer;

    private void Start() {
        _timer = _timeDelay;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _timer -= Time.deltaTime;

        if (_timer < 0f)
        {
            GetComponent<DamageBody>().TakeDamage(_damageValue);
            _timer = _timeDelay;
        }
    }
}
