using UnityEngine;

public class BulletExplosionModifier : Module
{
    [SerializeField] private bool _boostExplosion;
    [SerializeField] private int _explosionBoost;
    [SerializeField] private bool _setColor;
    [SerializeField] private Color _newColor;
    [SerializeField] private bool _boostScale;
    [SerializeField] private float _scaleMultiplier;
    [SerializeField] private bool _boostDamage;
    [SerializeField] private float _damageMultiplier;

    public override void Load()
    {
        AttackPattern[] attackPatterns = GameObject.FindObjectsOfType<AttackPattern>();

        for (int i = 0; i < attackPatterns.Length; i++)
        {
            GameObject attackObj = CharacterBulletDatabase.GetForChangeAttackObject(attackPatterns[i].CharacterBulletIndex).gameObject;

            _ExplosionBullet expb = attackObj.GetComponent<_ExplosionBullet>();

            if (expb == null)
                continue;

            if (_boostExplosion)
                expb.ExplosionCode = expb.ExplosionCode + _explosionBoost;

            if (_setColor)
                expb.ExplosionColor = _newColor;

            if (_boostScale)
                expb.Scale = expb.Scale * _scaleMultiplier;

            if (_boostDamage)
                expb.Damage = Mathf.RoundToInt((float)expb.Damage * _damageMultiplier);
        }
    }
}
