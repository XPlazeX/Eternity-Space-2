using UnityEngine;

public class PlayerTargetMovement : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private float _lerpSpeed;
    [SerializeField] private bool _fearing;
    [SerializeField] private int _fearingDamage = 40;
    [SerializeField] private Vector2 _backDistanceMinMax;
    [SerializeField] private float _backXoffset;
    [SerializeField] private float _fearTime;

    private float _fearTimer;
    private Vector3 _fearPosition;

    private void Start() {
        PlayerController.ReplacePlayer(_targetTransform);
        _targetTransform.SetParent(null);
    }

    private void OnEnable() {
        if (_fearing)
            PlayerShipData.TakeHealthDamage += OnHealthDamageTaken;
    }

    private void OnDisable() {
        if (_fearing)
            PlayerShipData.TakeHealthDamage -= OnHealthDamageTaken;
    }

    private void Update() {
        if (!PlayerController.CanControl)
            return;

        if (_fearTimer <= 0)
        {
            transform.position = Vector3.Lerp(transform.position, _targetTransform.position, _lerpSpeed * Time.deltaTime);
        } 
        else
        {
            _fearTimer -= Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, _fearPosition, _lerpSpeed * Time.deltaTime * 1.5f);
        }
    }

    private void OnHealthDamageTaken(int value)
    {
        if (value >= _fearingDamage)
            Fear();
    }

    private void Fear()
    {
        _fearTimer = _fearTime;
        _fearPosition = transform.position + new Vector3(Random.Range(-_backXoffset, _backXoffset), -Random.Range(_backDistanceMinMax.x, _backDistanceMinMax.y), 0);
    }
}
