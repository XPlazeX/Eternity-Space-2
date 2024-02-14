using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerTargetCursor : MonoBehaviour
{
    [SerializeField] private float _playerOffset;

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

        transform.position = Player.PlayerTransform.position + (_targetingTransform.position - Player.PlayerTransform.position).normalized * _playerOffset;
        transform.up = (_targetingTransform.position - Player.PlayerTransform.position);

        if (_visible && (Player.PlayerTransform.position - _targetingTransform.position).magnitude < _playerOffset)
        {
            _sr.enabled = false;
            _visible = false;
        } else if (!_visible && (Player.PlayerTransform.position - _targetingTransform.position).magnitude > _playerOffset)
        {
            _sr.enabled = true;
            _visible = true;
        }
    }
}
