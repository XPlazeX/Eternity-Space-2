using UnityEngine;
using DamageSystem;

[RequireComponent(typeof(DeathCaller))]
public class DamageBody : MonoBehaviour
{
    public virtual event healthOperation DamageTaking;
    public virtual event healthOperation Regenerated;
    public virtual event healthOperation HealthModified;
    public virtual event deathHandler Deathed;
    public virtual event bodyPositionHandler PositionDeathed;
    public System.Action Stunned;
    public System.Action Unstunned;

    [SerializeField] protected DamageKey _damageKey;
    [SerializeField] private int _hitPoints = 1;
    [SerializeField] protected int _startShieldPoints = 0;
    [SerializeField] protected int _flatArmor = 0;
    [SerializeField] [Range(0, 1f)] protected float damageReduction = 0f;
    [Header("Stuns & Rams")]
    [SerializeField] protected float stunPoints = 60f;
    [SerializeField] protected float stunRecovering = 10f;
    [SerializeField] protected float stunAdaptationStep = 0.15f;
    [SerializeField] protected float ramFromStunTime = 2f;

    private DeathCaller _deathCaller;
    // private Animator _damageTakingAnimator;
    private int _shieldPoints;
    protected ShieldComponent _shield;
    protected int _startHP;
    protected int _startFlatArmor;
    private bool _deathed;

    protected float _stunPoints = 0f;
    protected int _stunCount = 0;
    protected float _ramTimer; 
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
    public bool IsStunned {get; private set;} = false;
    public bool RamReady {get; private set;} = false;

    protected virtual void Awake() 
    {
        _hitPoints = Mathf.CeilToInt(_hitPoints * ShipStats.GetValue("EnemyHealthMultiplier") * GameSessionInfoHandler.HardnessMultiplier);
        _startHP = _hitPoints;
        _startFlatArmor = _flatArmor + ShipStats.GetIntValue("EnemyFlatArmor");
        // _damageTakingAnimator = GetComponent<Animator>();
    }

    private void OnEnable() 
    {
        HitPoints = _startHP;
        FlatArmor = _startFlatArmor;
        DamageReduction = damageReduction;
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

    void Update()
    {
        if (IsStunned)
        {
            _ramTimer -= Time.deltaTime;
            if (_ramTimer <= 0f)
            {
                IsStunned = false;
                if (HitPoints > PlayerRamsHandler.BaseHPtoRam)
                {
                    RamReady = false;
                }
                _stunPoints = 0f;
                Unstunned?.Invoke();
            }
        }
        else
        {
            if (_stunPoints > 0f)
            {
                _stunPoints = Mathf.Max(_stunPoints - Time.deltaTime * (1f + (_stunCount * stunAdaptationStep)) * stunRecovering, 0f);
                
            }
            
        }

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

        if (HitPoints <= PlayerRamsHandler.DecadesBlockForRam * 10)
        {
            RamReady = true;
        }
        
        if (HitPoints <= 0)
        {
            Death();
            return;
        }

        // if (_damageTakingAnimator)
        // {
        //     if (FlatArmor > 0)
        //     {
        //         _damageTakingAnimator.SetTrigger("ArmoredTakeDamage");
        //     }
        //     else{
        //         _damageTakingAnimator.SetTrigger("TakeDamage");
        //     }
        // }

        FightSoundHelper.PlaySound(0, transform.position);
        DamageTaking?.Invoke(HitPoints);
    }

    public virtual void TakeStun(float amount)
    {
        if (IsStunned || ShieldPoints > 0) return;
        _stunPoints += amount;
        if (_stunPoints > stunPoints * (1f + (_stunCount * stunAdaptationStep)))
        {
            Stun();
        }
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

    protected virtual void Stun()
    {
        _ramTimer = ramFromStunTime;
        RamReady = true;
        IsStunned = true;
        _stunCount ++;
        Stunned?.Invoke();
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
