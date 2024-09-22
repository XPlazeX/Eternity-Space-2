using UnityEngine;
using DamageSystem;

[RequireComponent(typeof(DeathCaller))]
public class DamageBody : MonoBehaviour
{
    public virtual event healthOperation DamageTaking;
    public virtual event healthOperation HealthModified;
    public virtual event deathHandler Deathed;
    public virtual event bodyPositionHandler PositionDeathed;

    [SerializeField] protected DamageKey _damageKey;
    [SerializeField] private int _hitPoints = 1;
    [SerializeField] protected int _startShieldPoints = 0;
    [SerializeField] protected int _flatArmor = 0;

    private DeathCaller _deathCaller;
    private Animator _damageTakingAnimator;
    private int _shieldPoints;
    protected ShieldComponent _shield;
    protected int _startHP;
    protected int _startFlatArmor;
    private bool _deathed;
    //protected int _decadesBlockForRam;

    public DamageKey KeyDamage => _damageKey;
    public int StartHP => _startHP;
    public int StartShield => _startShieldPoints;
    public virtual int HitPoints 
    {
        get {return _hitPoints;}

        protected set {
            _hitPoints = value;
            if (_hitPoints <= 0)
            {
                _hitPoints = 0;
            }
        }
    }
    public virtual int ShieldPoints
    {
        get {return _shieldPoints;}
        set {
            if (value == 0 && _shieldPoints > 0)
                BreakShield();

            _shieldPoints = value;
        }
    }
    public int FlatArmor {get; private set;}
    public float DamageReduction {get; set;}
    public bool OneShotProtection {get; set;} = true;

    protected virtual void Awake() 
    {
        _hitPoints = Mathf.CeilToInt(_hitPoints * ShipStats.GetValue("EnemyHealthMultiplier") * GameSessionInfoHandler.HardnessMultiplier);
        _startHP = _hitPoints;
        _startFlatArmor = _flatArmor;
        _damageTakingAnimator = GetComponent<Animator>();
    }

    private void OnEnable() 
    {
        HitPoints = _startHP;
        FlatArmor = _startFlatArmor;
    }

    void Start()
    {
        _deathCaller = GetComponent<DeathCaller>();
        //_decadesBlockForRam = ShipStats.GetIntValue("DecadesBlockForRam");

        GetShield(_startShieldPoints);
    }

    public virtual void MultiplyHP(float multiplier)
    {
        _startHP = Mathf.CeilToInt(_startHP * multiplier);
        HitPoints = Mathf.CeilToInt(HitPoints * multiplier);
        HealthModified?.Invoke(_startHP);
        DamageTaking?.Invoke(HitPoints);
    }

    public virtual void AddMaxHP(int hp)
    {
        _startHP += hp;
        HitPoints += hp;
        HealthModified?.Invoke(_startHP);
        DamageTaking?.Invoke(HitPoints);
    }

    public virtual void TakeDamage(int damage)
    {
        if (KeyDamage == DamageKey.Unvulnerable || !Player.Alive || damage == 0)
            return;
        
        int tempDmg = Mathf.CeilToInt(ShipStats.GetValue("InflictingDamageMultiplier") * damage);
        
        if (ShieldPoints != 0)
        {
            tempDmg = damage - ShieldPoints;

            if (tempDmg >= 0)
                ShieldPoints = 0;

            else
            {
                ShieldPoints = -tempDmg;
                _shield.UpdateSP(ShieldPoints);
                return;
            }
        }

        int takingValue = Mathf.CeilToInt((tempDmg - FlatArmor) * (1f - DamageReduction));

        if (takingValue <= 0)
            takingValue = 1;

        if (OneShotProtection && HitPoints > (PlayerRamsHandler.DecadesBlockForRam * 10) && takingValue > (HitPoints - (PlayerRamsHandler.DecadesBlockForRam * 10)))
        {
            HitPoints = PlayerRamsHandler.DecadesBlockForRam * 10;
        }
        else
            HitPoints -= takingValue;
        
        if (HitPoints <= 0)
        {
            Death();
            return;
        }

        if (_damageTakingAnimator)
        {
            if (FlatArmor > 0)
            {
                _damageTakingAnimator.SetTrigger("ArmoredTakeDamage");
            }
            else{
                _damageTakingAnimator.SetTrigger("TakeDamage");
            }
        }

        FightSoundHelper.PlaySound(0, transform.position);
        DamageTaking?.Invoke(HitPoints);
    }

    public void AddFlatArmor(int value) => FlatArmor += value;

    public virtual void GetShield(int shieldPoints)
    {
        if (shieldPoints == 0)
            return;

        shieldPoints = Mathf.FloorToInt(GameSessionInfoHandler.HardnessMultiplier * shieldPoints);
            
        if (_shield != null)
            BreakShield();
        _shield = ShieldDistributor.SpawnShield(transform, shieldPoints);
        ShieldPoints = shieldPoints;
    }

    protected virtual void BreakShield()
    {
        _shield.BreakShield();
        _shield = null;
    }

    protected virtual void Death()
    {
        if (_deathed)
            return;

        Deathed?.Invoke();
        PositionDeathed?.Invoke(transform.position);

        if (GetComponent<PullableObject>())
            gameObject.SetActive(false);
        else 
        {
            Destroy(gameObject);
            _deathed = true;
        }

        _deathCaller.DeathExplosion();

        DamageTaking = null;
    }
}
