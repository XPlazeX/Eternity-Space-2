using UnityEngine;
using DamageSystem;

public class PlayerShipData : MonoBehaviour
{
    public delegate void playerAction();

    public static event playerAction PlayerDeathed;

    public static event playerAction ShieldCreated;
    public static event playerAction ShieldBreaked;
    public static event playerAction DamageBufferGetted;
    public static event playerAction DamageBufferLosed;

    public static event playerAction AnyHurtTaked;
    public static event healthOperation StructuralDamageChanged;
    public static event healthOperation ShieldDamageTaked;
    public static event healthOperation HealthDamageTaked;
    public static event healthOperation Regenerated;

    public static event healthOperation HealthChanged;
    public static event System.Action<bool> UnvulnerabilityToggled;

    public static event playerAction SystemsExposed; // 200
    public static event playerAction CoreExposed; // 100
    public static event playerAction LastHPExposed; // 1
    public static event playerAction Stunned;

    public static event playerAction CoreSealed; // 100
    public static event playerAction SystemsSealed; // 200
    public static event playerAction ShellSealed; // 300

    public const int MAX_HITPOINTS = 300;
    public const int SHELL_BORDER = 200;
    public const int CORE_BORDER = 100;
    public const float STUN_AMOUNT = 100f;
    public const float UNVULNERABLE_TIME_BETWEEN_LAYERS = 0.5f;
    public const int STRUCTURE_REGENERATION_AMOINT = 5;

    private static int _hitPoints;
    private static int _shieldPoints;
    private static int _maxGettedShieldPoints;
    private static int _damageBuffers;
    private static int _structuralDamage;
    
    private static int _flatArmor;
    private static float _damageReduction;
    private static ShieldComponent _shield;
    private static DamageBufferComponent _damageBuffer;

    private static float _lastHurtTimeStamp;
    private static int _unvulnerabilityRequests = 0;
    private static bool _isLastChanceReady = false;
    private static bool _isCoreSealed = false;
    private static bool _isSystemsSealed = false;
    private static bool _isShellSealed = false;

    public static int HitPoints 
    {
        get {return _hitPoints;} 
        
        private set 
        {
            _hitPoints = Mathf.Clamp(value, 0, MAX_HITPOINTS);
            HealthChanged?.Invoke(value);
        }
    }
    public static int StructuralDamage 
    {
        get {return _structuralDamage;} 
        
        private set 
        {
            _structuralDamage = Mathf.Clamp(value, 0, MaxStructuralDamage);
            StructuralDamageChanged?.Invoke(value);
        }
    }
    public static int FlatArmor 
    {
        get {return _flatArmor;} 
        
        private set 
        {
            _flatArmor = value;
        }
    }
    public static float DamageReduction 
    {
        get {return _damageReduction;} 
        
        private set 
        {
            _damageReduction = value;
        }
    }
    public static int ShieldPoints
    {
        get {return _shieldPoints;}
        set {
            if (value == 0 && _shieldPoints > 0)
            {
                BreakShield();
                _shieldPoints = 0;
                return;
            }

            _shieldPoints = Mathf.Clamp(value, 0, _maxGettedShieldPoints);
            _shield?.UpdateSP(ShieldPoints);
        }
    }
    public static int DamageBuffers
    {
        get {return _damageBuffers;}
        set {
            if (value == 0 && _damageBuffers > 0)
                BreakDamageBuffer();
            else
                _damageBuffer.UpdateDB();

            _damageBuffers = Mathf.Clamp(value, 0, MaxDamageBuffers);
        }
    }

    public static bool Unvulnerable => ESTime.worldTime - _lastHurtTimeStamp < UNVULNERABLE_TIME_BETWEEN_LAYERS * RR.Get(RuntimeStat.UnvulnerableTimeAfterDamageMultiplier) || _unvulnerabilityRequests > 0;
    public static bool Alive {get; private set;} = false;
    public static int MaxStructuralDamage => MAX_HITPOINTS - SHELL_BORDER;
    public static int CurrentMaxHitPoints => MAX_HITPOINTS - StructuralDamage;
    public static bool HasShield => _shield != null && ShieldPoints > 0;
    public static int CurrentMaxShieldPoints => _shield != null ? _maxGettedShieldPoints : 0;
    public static int MaxDamageBuffers => 1;
    public static bool IsShellSealed => _isShellSealed;
    public static bool IsSystemsSealed => _isSystemsSealed;
    public static bool IsCoreSealed => _isCoreSealed;
    public static bool IsFullHP => HitPoints >= MAX_HITPOINTS;

    public void Initialize(int hp, int arm, int shipID)
    {
        _flatArmor = 0;
        _damageReduction = 0f;
        _shieldPoints = 0;
        _damageBuffers = 0;
        _structuralDamage = 0;

        _isCoreSealed = true;
        _isSystemsSealed = true;
        _isShellSealed = true;
        _isLastChanceReady = true;

        Alive = true;

        SetHitPoints(MAX_HITPOINTS);
    }

    public static bool TakeDamage(DamageBundle damageBundle, out bool killed)
    {
        killed = false;

        if (Unvulnerable || !Alive || !damageBundle.Legitime || (damageBundle.damageKey != DamageKey.Player && damageBundle.damageKey != DamageKey.Everything))
            return false;

        for (int i = 0; i < damageBundle.cycles; i++)      
        {
            AnyHurtTaked?.Invoke();

            if (damageBundle.damageValue > 0)
            {
                if (DamageBuffers > 0)
                {
                    DamageBuffers --;
                    DamageBufferLosed?.Invoke();
                    continue;
                }

                int tempDmg = damageBundle.damageValue;

                if (ShieldPoints > 0)
                {
                    tempDmg = Mathf.CeilToInt(damageBundle.shieldDamageMultiplier * tempDmg) - ShieldPoints;

                    if (tempDmg >= 0)
                    {
                        ShieldPoints = 0;
                        ShieldDamageTaked?.Invoke(tempDmg);
                        continue;
                    } else
                    {
                        ShieldPoints = -tempDmg;
                        ShieldDamageTaked?.Invoke(tempDmg);
                        continue;
                    }
                }

                tempDmg = Mathf.CeilToInt((tempDmg - Mathf.Max(0, FlatArmor - damageBundle.armorPenetration)) * Mathf.Max(0f, 1f - DamageReduction));

                if (tempDmg <= 0)
                {
                    tempDmg = 1;
                }

                int targetHP = HitPoints - tempDmg;

                if (_isShellSealed && targetHP <= MAX_HITPOINTS)
                {
                    _isShellSealed = false;
                }

                if (_isSystemsSealed && targetHP <= SHELL_BORDER) // 
                {
                    _isSystemsSealed = false;
                    if (HitPoints > SHELL_BORDER)
                        targetHP = SHELL_BORDER;

                    _lastHurtTimeStamp = ESTime.worldTime;

                    SystemsExposed?.Invoke();
                }
                else if (_isCoreSealed && targetHP <= CORE_BORDER) // 
                {
                    _isCoreSealed = false;
                    if (HitPoints > CORE_BORDER)
                        targetHP = CORE_BORDER;

                    _lastHurtTimeStamp = ESTime.worldTime;

                    CoreExposed?.Invoke();
                }
                else if (_isLastChanceReady && targetHP <= 1) // 
                {
                    _isLastChanceReady = false;
                    targetHP = 1;

                    _lastHurtTimeStamp = ESTime.worldTime;

                    LastHPExposed?.Invoke();
                } else if (!_isLastChanceReady && targetHP <= 0)
                {
                    killed = true;
                }
                
                SetHitPoints(targetHP);
                HealthDamageTaked?.Invoke(tempDmg);
                
            }

            StructuralDamage += damageBundle.structuralDamage;

            if (damageBundle.stunAmount > STUN_AMOUNT)
            {
                Stunned?.Invoke();
            }

            if (damageBundle.regeneratingValue > 0)
            {
                RegenerateHP(damageBundle.regeneratingValue);
            }
        }

        // CheckState();
        return true;
    }

    private static void SetHitPoints(int newValue)
    {
        if (!Alive)
            return;

        HitPoints = Mathf.Clamp(newValue, 0, MAX_HITPOINTS);

        if (HitPoints <= 0)
        {
            Death();
        }
    }

    public static void RegenerateHP(int addingValue)
    {
        if (addingValue <= 0 || !Alive)
            return;

        int targetHP = HitPoints + addingValue;

        if (!_isLastChanceReady)
        {
            _isLastChanceReady = true;
        }
        if (!_isCoreSealed && targetHP >= CORE_BORDER)
        {
            _isCoreSealed = true;
            targetHP = CORE_BORDER;
            CoreSealed?.Invoke();
        }
        else if (!_isSystemsSealed && targetHP >= SHELL_BORDER)
        {
            _isSystemsSealed = true;
            targetHP = SHELL_BORDER;
            SystemsSealed?.Invoke();
        }
        else if (!_isShellSealed && targetHP >= CurrentMaxHitPoints)
        {
            targetHP = CurrentMaxHitPoints;

            if (targetHP >= MAX_HITPOINTS)
            {
                _isShellSealed = true;
                ShellSealed?.Invoke();
            } else
            {
                StructuralDamage -= STRUCTURE_REGENERATION_AMOINT;
            }
        }

        SetHitPoints(targetHP);
        Regenerated?.Invoke(addingValue);
    }

    public static void GetShield(int shieldPoints)
    {
        if (!Alive)
            return;

        if (_shield != null)
            BreakShield();

        if (shieldPoints == 0)
        {
            return;
        }
        
        _shield = ShieldDistributor.SpawnShield(Player.PlayerTransform, shieldPoints);
        _maxGettedShieldPoints = shieldPoints;
        ShieldPoints = shieldPoints;
        ShieldCreated?.Invoke();
    }

    private static void BreakShield()
    {
        if (_shield != null)
            _shield.BreakShield();

        _shield = null;
        _maxGettedShieldPoints = 0;
        ShieldBreaked?.Invoke();
    }

    public static void AddDamageBuffer(int db)
    {
        if (!Alive)
            return;

        if (db == 0)
        {
            return;
        }
        
        if (_damageBuffer == null)
        {
            _damageBuffer = ShieldDistributor.SpawnDamageBuffer(Player.PlayerTransform);
        }
        
        DamageBuffers += db;
        DamageBufferGetted?.Invoke();
    }

    private static void BreakDamageBuffer()
    {
        if (_damageBuffer != null)
            _damageBuffer.BreakShield();
        
        _damageBuffer = null;
    }

    public static void RequestUnvulnerability()
    {
        _unvulnerabilityRequests++;
        UnvulnerabilityToggled?.Invoke(true);
    }

    public static void ReleaseUnvulnerability()
    {
        if (_unvulnerabilityRequests <= 0)
            return;

        _unvulnerabilityRequests--;
        if (_unvulnerabilityRequests < 0)
        {
            UnvulnerabilityToggled?.Invoke(false);
        }
    }

    private static void Death()
    {
        ReviveManager.TryRevive();

        // DeactivateAllBindedSystems();
        PlayerDeathed?.Invoke();
        SceneStatics.UICore.GetComponent<DeathUIHandler>().Death();
    }

    // #if UNITY_EDITOR
    private void Update() {
        // if (Input.GetKeyDown(KeyCode.Alpha9))
        // {
        //     TakeDamage(999);
        // }
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(new DamageBundle()
            {
                damageKey = DamageKey.Player,
                damageValue = 0,
                armorPenetration = 0,
                shieldDamageMultiplier = 1f,
                structuralDamage = 0,
                regeneratingValue = 20,
                stunAmount = 0f,
                cycles = 1
            }, out bool killed);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            TakeDamage(new DamageBundle()
            {
                damageKey = DamageKey.Player,
                damageValue = 30,
                armorPenetration = 0,
                shieldDamageMultiplier = 1f,
                structuralDamage = 0,
                regeneratingValue = 0,
                stunAmount = 0f,
                cycles = 1
            }, out bool killed);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            TakeDamage(new DamageBundle()
            {
                damageKey = DamageKey.Player,
                damageValue = 13,
                armorPenetration = 0,
                shieldDamageMultiplier = 1f,
                structuralDamage = 13,
                regeneratingValue = 0,
                stunAmount = 0f,
                cycles = 1
            }, out bool killed);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            GetShield(40);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            AddDamageBuffer(1);
        }
        // if (Input.GetKeyDown(KeyCode.M))
        // {
        //     TakeDamage(20);
        // }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetHitPoints(0);
        }
    }
    // #endif

}
