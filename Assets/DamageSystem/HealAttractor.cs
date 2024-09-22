using UnityEngine;
using DamageSystem;

public class HealAttractor : MonoBehaviour
{
    [SerializeField] private AttackObject _damageDealer;
    [SerializeField] private int _healPerAttack;
    [SerializeField] private int _minHealLimit = 10;
    [SerializeField] private int _healLimit = 999;
    [SerializeField] private bool _multiUse = false;
    [SerializeField] private int _orbID;
    [Space()]
    [SerializeField] private bool _spriteChanging;
    [SerializeField] private Sprite _readySprite;
    [SerializeField] private Sprite _fulledSprite;
    [Space()]
    [SerializeField] private bool _abilityUnlocker;

    private int _healTank = 0;
    private int _state = 0;

    private void OnEnable() 
    {
        _damageDealer.Initialize();
        _damageDealer.DamageBodyInflicted += OnDamageInflicted;
    }

    private void OnDisable() {
        _damageDealer.DamageBodyInflicted -= OnDamageInflicted;
    }

    private void OnDamageInflicted(DamageBody targetBody, int damage)
    {
        _healTank += _healPerAttack;

        if (_spriteChanging && _healTank >= _minHealLimit && _state == 0)
        {
            GetComponent<SpriteRenderer>().sprite = _readySprite;
            _state = 1;
        }

        if (_healTank > _healLimit)
        {
            if (_spriteChanging && _fulledSprite != null && _state == 1)
            {
                GetComponent<SpriteRenderer>().sprite = _fulledSprite;
                _state = 2;
            }

            _healTank = _healLimit;

            if (_abilityUnlocker && targetBody.GetComponent<_AbbyssCreature_>() != null && !Unlocks.HasUnlock(904))
            {
                Unlocks.NewUnlock(904);
            }
        }

        else if (_orbID != -1)
            OrbHandler.SpawnOrb(_orbID, targetBody.transform.position, transform);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        PlayerDamageBody pdb = other.GetComponent<PlayerDamageBody>();

        if (pdb == null)
            return;

        if (Mathf.Abs(_healTank) < _minHealLimit)
            return;

        if (_healTank > 0)
        {
            PlayerShipData.RegenerateHP(_healTank);
        } else if (_healTank < 0)
        {
            PlayerShipData.TakeDamage(-_healTank);
        }

        _healTank = 0;

        if (!_multiUse)
            Destroy(gameObject);
    }
}
