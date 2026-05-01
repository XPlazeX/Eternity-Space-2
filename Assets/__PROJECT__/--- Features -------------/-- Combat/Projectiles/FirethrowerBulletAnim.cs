using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Bullet))]
public class FirethrowerBulletAnim : MonoBehaviour
{
    [SerializeField] private float _startDamage;
    [SerializeField] private AnimationCurve _damageOverLifetime;
    [SerializeField] private AnimationCurve _scaleOverLifetime;
    [SerializeField] private SpriteRenderer[] _fadingSRs;

    private Bullet _bulletComponent;

    private void Awake() {
        _bulletComponent = GetComponent<Bullet>();
    }

    private void FixedUpdate() 
    {
        for (int i = 0; i < _fadingSRs.Length; i++)
        {
            Color color = _fadingSRs[i].color;
            _fadingSRs[i].color = new Color(color.r, color.g, color.b, _bulletComponent.Lifetimer / _bulletComponent.Lifetime);
        }

        _bulletComponent.Damage = Mathf.RoundToInt(_startDamage * _damageOverLifetime.Evaluate(1f - _bulletComponent.Lifetimer / _bulletComponent.Lifetime));
        transform.localScale = Vector3.one * _scaleOverLifetime.Evaluate(1f - _bulletComponent.Lifetimer / _bulletComponent.Lifetime);
    }


}
