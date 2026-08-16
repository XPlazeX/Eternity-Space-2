using UnityEngine;
using DamageSystem;

[RequireComponent(typeof(DeathCaller))]
public class DamageBody : MonoBehaviour, IDamagable
{
    private const int STRUCTURAL_MAX_REGENERATING_VALUE = 5;

    public virtual event healthOperation DamageTaking;
    public virtual event healthOperation Regenerated;
    public virtual event healthOperation HealthChanged;
    public virtual event healthOperation HealthModified;
    public virtual event deathHandler Deathed;
    public virtual event bodyPositionHandler PositionDeathed;
    public System.Action Stunned;
    public System.Action Unstunned;

    [SerializeField] protected DamageKey _damageKey;
    [SerializeField] private int _hitPoints = 1;
    [SerializeField] protected int _startShieldPoints = 0;
    [SerializeField] protected int _startDamageBuffers = 0;
    [SerializeField] protected int _flatArmor = 0;
    [SerializeField] [Range(0, 1f)] protected float damageReduction = 0f;
    [Header("Stuns & Rams")]
    [SerializeField] protected float stunPoints = 60f;
    [SerializeField] protected float stunRecovering = 10f;
    [SerializeField] protected float stunAdaptationStep = 0.15f;
    [SerializeField] protected bool rammable = true;
    [SerializeField] protected float ramFromStunTime = 2f;

    private DeathCaller _deathCaller;
    // private Animator _damageTakingAnimator;
    private int _shieldPoints;
    private int _damageBuffers;
    protected ShieldComponent _shield;
    protected DamageBufferComponent _damageBuffer;
    protected int _maxHP;
    protected int _startFlatArmor;
    private bool _deathed;

    protected float _stunPoints = 0f;
    protected int _stunCount = 0;
    protected float _ramTimer; 
    protected virtual bool StackingByStructureDamage {get;} = true;
    //protected int _decadesBlockForRam;

    public DamageKey KeyDamage {get => _damageKey;}
    public int MaxHP => _maxHP;
    public int StartShield => _startShieldPoints;
    public int StartDamageBuffers => _startDamageBuffers;
    public int CurrentMaxHitPoints => _maxHP - StructuralDamage;
    public virtual int HitPoints 
    {
        get {return _hitPoints;}

        protected set 
        {
            _hitPoints = Mathf.Clamp(value, 0, CurrentMaxHitPoints);
            if (_hitPoints <= PlayerRamsHandler.DecadesBlockForRam * 10)
            {
                RamReady = rammable ? true : false;
            }
        }
    }
    public virtual int ShieldPoints
    {
        get {return _shieldPoints;}
        set {
            if (value == 0 && _shieldPoints > 0)
                BreakShield();
            else
                _shield.UpdateSP(value);

            _shieldPoints = value;
        }
    }
    public virtual int DamageBuffers
    {
        get {return _damageBuffers;}
        set {
            if (value == 0 && _damageBuffers > 0)
                BreakDamageBuffer();
            else
                _damageBuffer.UpdateDB();

            _damageBuffers = value;
        }
    }
    public int FlatArmor {get; private set;}
    public int StructuralDamage {get; private set;}
    public float DamageReduction {get; set;}
    public bool OneShotProtection {get; set;} = true;
    public bool IsStunned {get; private set;} = false;
    public virtual bool RamReady {get; private set;} = false;

    protected virtual void Awake() 
    {
        _maxHP = _hitPoints;
        _startFlatArmor = _flatArmor;// + ShipStats.GetIntValue("EnemyFlatArmor");
    }

    private void OnEnable() 
    {
        HitPoints = _maxHP;
        FlatArmor = _startFlatArmor;
        DamageReduction = damageReduction;
    }

    void Start()
    {
        _deathCaller = GetComponent<DeathCaller>();

        GetShield(_startShieldPoints);
        GetDamageBuffer(_startDamageBuffers);
    }

    public virtual void MultiplyHP(float multiplier)
    {
        _maxHP = Mathf.CeilToInt(_maxHP * multiplier);
        HitPoints = Mathf.CeilToInt(HitPoints * multiplier);
        HealthModified?.Invoke(_maxHP);
        HealthChanged?.Invoke(HitPoints);
    }

    public virtual void AddMaxHP(int hp)
    {
        _maxHP += hp;
        HitPoints += hp;
        HealthModified?.Invoke(_maxHP);
        HealthChanged?.Invoke(HitPoints);
    }

    void Update()
    {
        if (IsStunned)
        {
            _ramTimer -= ESTime.worldDeltaTime;
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
                _stunPoints = Mathf.Max(_stunPoints - ESTime.worldDeltaTime * (1f + (_stunCount * stunAdaptationStep)) * stunRecovering, 0f);
                
            }
            
        }
    }

    public virtual bool TakeDamage(DamageBundle damageBundle, out bool killed)
    {
        killed = false;

        if (KeyDamage == DamageKey.Unvulnerable || !Player.Alive || !damageBundle.Legitime)
            return false;

        bool iAsteroid = this is AsteroidBody;

        if (((damageBundle.damageKey == DamageKey.ToAsteroids && !iAsteroid) || damageBundle.damageKey != KeyDamage) && damageBundle.damageKey != DamageKey.Everything)
            return false;
        
        for (int i = 0; i < damageBundle.cycles; i++)      
        {
            if (damageBundle.damageValue > 0)
            {
                if (DamageBuffers > 0)
                {
                    DamageBuffers --;
                    
                    continue;
                }

                int tempDmg = damageBundle.damageValue;

                if (ShieldPoints > 0)
                {
                    tempDmg = Mathf.CeilToInt(damageBundle.shieldDamageMultiplier * tempDmg) - ShieldPoints;

                    if (tempDmg >= 0)
                    {
                        ShieldPoints = 0;
                        continue;
                    } else
                    {
                        ShieldPoints = -tempDmg;
                        
                        continue;
                    }
                }

                if (iAsteroid)
                    tempDmg = Mathf.CeilToInt(damageBundle.asteroidDamageMultiplier * tempDmg);

                tempDmg = Mathf.CeilToInt(tempDmg - Mathf.Max(0, FlatArmor - damageBundle.armorPenetration) * Mathf.Max(0f, 1f - DamageReduction)) + (StackingByStructureDamage ? StructuralDamage : 0);

                if (tempDmg <= 0)
                {
                    tempDmg = 1;
                }

                if (OneShotProtection && !damageBundle.ignoreOneShotProtection && HitPoints > (PlayerRamsHandler.DecadesBlockForRam * 10) && tempDmg > (HitPoints - (PlayerRamsHandler.DecadesBlockForRam * 10)))
                {
                    HitPoints = PlayerRamsHandler.DecadesBlockForRam * 10;
                }
                else
                {
                    HitPoints -= tempDmg;
                }

                if (HitPoints <= 0)
                {
                    Death(damageBundle, Mathf.Abs(0 - HitPoints));
                    killed = true;
                    return true;
                }

                HealthChanged?.Invoke(HitPoints);
                DamageTaking?.Invoke(tempDmg);
            }

            StructuralDamage += damageBundle.structuralDamage;

            if (damageBundle.stunAmount > 0f)
            {
                TakeStun(damageBundle.stunAmount);
            }

            if (damageBundle.regeneratingValue > 0)
            {
                if (HitPoints >= CurrentMaxHitPoints && StructuralDamage > 0)
                {
                    StructuralDamage -= Mathf.Min(damageBundle.regeneratingValue, STRUCTURAL_MAX_REGENERATING_VALUE);
                } else
                {
                    HitPoints = Mathf.Clamp(HitPoints + damageBundle.regeneratingValue, 0, CurrentMaxHitPoints);
                }

                Regenerated?.Invoke(damageBundle.regeneratingValue);
                HealthChanged?.Invoke(HitPoints);
            }
        }

        FightSoundHelper.PlaySound(0, transform.position);
        return true;
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

    public virtual void GetDamageBuffer(int buffers)
    {
        if (buffers == 0)
            return;
            
        if (_damageBuffer != null)
            BreakDamageBuffer();

        _damageBuffer = ShieldDistributor.SpawnDamageBuffer(transform);
        DamageBuffers += buffers;
    }

    protected virtual void BreakDamageBuffer()
    {
        _damageBuffer.BreakShield();
        _damageBuffer = null;
    }

    protected virtual void Stun()
    {
        _ramTimer = ramFromStunTime;
        RamReady = rammable ? true : false;
        IsStunned = true;
        _stunCount ++;
        Stunned?.Invoke();
    }

    protected virtual void Death(DamageBundle sourceBundle, int overdmg)
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

        // _deathCaller.DeathExplosion();
        _deathCaller.Call(new DeathContext()
        {
            velocity = GetComponent<EnemyAIRoot>() == null ? Vector3.zero : GetComponent<EnemyAIRoot>().Inertion,
            damageTag = sourceBundle.damageTag,
            overdamage = overdmg,
            wasStunned = IsStunned
        });

        DamageTaking = null;
    }
}
