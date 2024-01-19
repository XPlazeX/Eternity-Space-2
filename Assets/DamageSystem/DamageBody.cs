using UnityEngine;
using DamageSystem;

[RequireComponent(typeof(DeathCaller))]
public class DamageBody : MonoBehaviour
{
    public event healthOperation DamageTaking;
    public event deathHandler Deathed;
    public event bodyPositionHandler PositionDeathed;

    [SerializeField] protected DamageKey _damageKey;
    [SerializeField] private int _hitPoints = 1;
    [SerializeField] protected int _startShieldPoints = 0;

    private DeathCaller _deathCaller;
    private Animator _damageTakingAnimator;
    private int _shieldPoints;
    protected ShieldComponent _shield;
    protected int _startHP;
    private bool _deathed;
    protected int _decadesBlockForRam;

    public DamageKey KeyDamage => _damageKey;
    public int StartHP => _startHP;
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
    public float DamageReduction {get; set;}
    public bool OneShotProtection {get; set;} = true;

    protected virtual void Awake() 
    {
        _hitPoints = Mathf.CeilToInt(_hitPoints * ShipStats.GetValue("EnemyHealthMultiplier"));
        _startHP = _hitPoints;
        _damageTakingAnimator = GetComponent<Animator>();
    }

    private void OnEnable() 
    {
        HitPoints = _startHP;
    }

    void Start()
    {
        _deathCaller = GetComponent<DeathCaller>();
        _decadesBlockForRam = ShipStats.GetIntValue("DecadesBlockForRam");

        GetShield(_startShieldPoints);
    }

    public virtual void TakeDamage(int damage)
    {
        if (KeyDamage == DamageKey.Unvulnerable)
            return;
        
        int tempDmg = damage;
        
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

        int takingValue = Mathf.CeilToInt(tempDmg * (1f - DamageReduction));

        if (OneShotProtection && HitPoints > (_decadesBlockForRam * 10) && takingValue > (HitPoints - (_decadesBlockForRam * 10)))
        {
            HitPoints = _decadesBlockForRam * 10;
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
            _damageTakingAnimator.SetTrigger("TakeDamage");
        }

        FightSoundHelper.PlaySound(0, transform.position);
        DamageTaking?.Invoke(HitPoints);
    }

    public virtual void GetShield(int shieldPoints)
    {
        if (shieldPoints == 0)
            return;
            
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
