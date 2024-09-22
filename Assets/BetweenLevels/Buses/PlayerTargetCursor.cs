using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerTargetCursor : MonoBehaviour
{
    [SerializeField] private float _playerOffset;
    [SerializeField] private bool _toPlayer = false;

    private SpriteRenderer _sr;
    private Transform _targetingTransform;
    private bool _visible = true;

    private void Start() 
    {
        _sr = GetComponent<SpriteRenderer>();
        _targetingTransform = transform.parent;

        transform.SetParent(null);
    }

    private void Update() 
    {
        if (_targetingTransform == null)
        {
            _sr.enabled = false;
            return;
        }

        transform.position = GetTarget().position + (GetPivot().position - GetTarget().position).normalized * _playerOffset;
        transform.up = (GetPivot().position - GetTarget().position);

        if (_visible && (GetTarget().position - GetPivot().position).magnitude < _playerOffset)
        {
            _sr.enabled = false;
            _visible = false;
        } else if (!_visible && (GetTarget().position - GetPivot().position).magnitude > _playerOffset)
        {
            _sr.enabled = true;
            _visible = true;
        }
    }

    private Transform GetPivot() => _toPlayer ? Player.PlayerTransform : _targetingTransform;
    private Transform GetTarget() => _toPlayer ? _targetingTransform : Player.PlayerTransform;
}
