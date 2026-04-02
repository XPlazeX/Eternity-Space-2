using UnityEngine;

namespace DamageSystem
{
    public delegate void healthOperation(int takingValue);
    public delegate void deathHandler();
    public delegate void bodyPositionHandler(Vector3 position);
    public delegate void bodyInflicting(DamageBody damageBody, int resultDamage);
    
    public enum DamageKey
    {
        Unvulnerable = -1,
        Player = 0,
        Enemy = 1,
        Everything = 2,
        ToAsteroids = 3
    }

    public class AttackObject : PullableObject
    {
        public event bodyInflicting DamageBodyInflicted;

        [SerializeField] private DamageBundle damageBundle;

        public DamageKey KeyDamage => damageBundle.damageKey;
        public DamageBundle DMGBundle => damageBundle;

        public int Damage 
        {
            get {return damageBundle.damageValue;}
            set {damageBundle.damageValue = value;}
        }

        protected virtual bool InflictDamage(DamageBody damageBody, float moddedDamage = -1f)
        {
            if (damageBody.KeyDamage == DamageKey.Unvulnerable || !damageBundle.Legitime)
                return false;

            if (moddedDamage > 0)
                damageBundle.damageValue = Mathf.CeilToInt(moddedDamage);

            bool result = damageBody.TakeDamage(damageBundle);

            if (result)
                DamageBodyInflicted?.Invoke(damageBody, damageBundle.damageValue);
            return result;
        }

        public void ChangeDamageKey(DamageKey newKey) => damageBundle.damageKey = newKey;

        public static bool InflictDamage(DamageBody targetBody, DamageBundle damageBundle)
        {
            //print("static inflict");
            if (targetBody.KeyDamage == DamageKey.Unvulnerable || !damageBundle.Legitime)
            {
                return false;
            }

            return targetBody.TakeDamage(damageBundle);
        }
    }

    [System.Serializable]
    public record DamageBundle
    {
        [Header("Main Damage")]
        [Tooltip("Unvulnerable никогда не получают урон, Everything наносит урон всем, ToAsteroids делает проверку is AsteroidBody, Player и Enemy должны совпадать у наносителя и получателя.")]
        public DamageKey damageKey = DamageKey.Everything;
        [Tooltip("Базовый урон с которым всё считается. Если <= 0 - урон считаться НЕ БУДЕТ, даже если эффекты дают прибавку.")]
        public int damageValue = 0;
        [Tooltip("Множитель урона по ShieldPoints. Щит заблокирует удар перед нанесением урона телу.")]
        public float shieldDamageMultiplier = 1f;
        [Tooltip("Множитель урона по AsteroidBody")]
        public float asteroidDamageMultiplier = 1f;
        [Tooltip("Пробитие FlatArmor. Не суммируется с основным уроном, просто вычитает из FlatArmor")]
        public int armorPenetration = 0;
        [Header("Effects")]
        [Tooltip("Количество стана. Базовое количество стана на врагов = 60, восстанавливают 10 в секунду. Стан оглушает противника и делает его доступным для тарана на 2 сек")]
        public float stunAmount = 0f;
        [Tooltip("Структурный урон. Увеличивает получаемый урон на это значение.")]
        public int structuralDamage = 0;
        [Tooltip("Если true - попадание игнорирует отсечку в 10 хп и получение урона может сразу убить врага.")]
        public bool ignoreOneShotProtection = false;
        [Tooltip("Считается после всего урона или если damageValue <= 0. Это восстановление здоровья, не скейлится от баффов урона.")]
        public int regeneratingValue = 0;
        [Tooltip("Сколько раз будет прогонятся данный DamageBundle")]
        public int cycles = 1;

        public bool Legitime => (damageValue > 0 || regeneratingValue > 0 || structuralDamage > 0 || stunAmount > 0f) && cycles > 0;
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
