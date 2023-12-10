using UnityEngine;
using DamageSystem;

public class MainWeaponBulletModifier : Module
{
    [SerializeField] private Sprite _newSprite;
    [SerializeField] private float _damageMult = 1f;
    [Space()]
    [SerializeField] private bool _addHoming;
    [SerializeField] private string _homingTag;
    [SerializeField] private float _homingPower;

    public override void Load()
    {
        AttackPattern[] attackPatterns = GameObject.FindObjectsOfType<AttackPattern>();

        for (int i = 0; i < attackPatterns.Length; i++)
        {
            GameObject attackObj = CharacterBulletDatabase.GetForChangeAttackObject(attackPatterns[i].CharacterBulletIndex).gameObject;  

            if (attackObj.GetComponent<SpriteRenderer>() != null)
                attackObj.GetComponent<SpriteRenderer>().sprite = _newSprite;

            attackObj.GetComponent<AttackObject>().Damage = Mathf.CeilToInt(attackObj.GetComponent<AttackObject>().Damage * _damageMult);

            if (_addHoming && attackObj.GetComponent<Bullet>() != null)
            {
                _Homing_ home = attackObj.AddComponent<_Homing_>();
                home.ModParams(_homingTag, _homingPower);
            }
        }
    }
}
