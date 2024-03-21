using UnityEngine;

namespace DamageSystem
{
    public delegate void healthOperation(int takingValue);
    public delegate void deathHandler();
    public delegate void bodyPositionHandler(Vector3 position);
    
    public enum DamageKey
    {
        Unvulnerable = -1,
        Player = 0,
        Enemy = 1,
        Everything = 2
    }

    public class AttackObject : PullableObject
    {
        public const float defaultDamageToAsteroid = 1f;

        [SerializeField] private DamageKey _damageKey;
        [SerializeField] private int _damageValue;
        [SerializeField] private float _shieldDamageMultiplier = 1f;
        [SerializeField] private float _asteroidDamageMultiplier = 1f;

        public DamageKey KeyDamage => _damageKey;

        public int Damage 
        {
            get {return _damageValue;}
            set {_damageValue = value;}
        }

        protected virtual bool InflictDamage(DamageBody damageBody, float moddedDamage = -1f)
        {
            if (damageBody.KeyDamage == DamageKey.Unvulnerable || ((_damageKey != damageBody.KeyDamage) && _damageKey != DamageKey.Everything))
                return false;

            float dmg = _damageValue;

            if (moddedDamage > 0)
                dmg = moddedDamage;

            if (_shieldDamageMultiplier != 1f && damageBody.ShieldPoints > 0)
                dmg *= _shieldDamageMultiplier;

            if (_asteroidDamageMultiplier != 1f && damageBody is AsteroidBody)
                dmg *= _asteroidDamageMultiplier;

            damageBody.TakeDamage(Mathf.CeilToInt(dmg));
            return true;
        }

        public void ChangeDamageKey(DamageKey newKey) => _damageKey = newKey;

        public static bool InflictDamage(DamageBody targetBody, DamageKey key, int damageValue)
        {
            //print("static inflict");
            if (targetBody.KeyDamage != DamageKey.Unvulnerable && ((key == targetBody.KeyDamage) || key == DamageKey.Everything))
            {
                targetBody.TakeDamage(damageValue);
                return true;
            }
            else
                return false;

        }
    }
}

namespace EffectsDesign
{
    public enum ShleifMode
    {
        None = 0,
        OnMove = 1,
        Always = 2
    }
}
