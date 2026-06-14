using UnityEngine;
using DamageSystem;

public class PlayerShipData : MonoBehaviour
{
    public delegate void playerAction();

    public static event playerAction PlayerDeathed;
    public static event playerAction ShieldCreated;
    public static event playerAction DamageBufferGetted;
    public static event playerAction ShieldBreaked;
    public static event System.Action AnyDamageTaked;
    public static event System.Action<bool> CriticalStateChanged;
    public static event healthOperation StructuralDamageChanged;
    public static event healthOperation ShieldDamageTaked;
    public static event healthOperation HealthDamageTaked;
    public static event playerAction DamageBuffer1Losed;
    public static event healthOperation Regenerated;

    public static event healthOperation HealthChanged;
    // public static event healthOperation ArmorChanged;
    /// <summary>
    /// Не затрагивает неуязвимость после получения урона.
    /// </summary>
    public static event System.Action<bool> UnvulnerabilityToggled;

    public static int HitPoints 
    {
        get {return _hitPoints;} 
        
        private set 
        {
            _hitPoints = value;
            CheckState();
            // _playerUI.ChangeHP(_hitPoints, (float)_hitPoints / _hpCap);
            HealthChanged?.Invoke(value);
        }
    }
    public static int StructuralDamage 
    {
        get {return _structuralDamage;} 
        
        private set 
        {
            _structuralDamage = value;
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
                BreakShield();
            else
                _shield.UpdateSP(ShieldPoints);

            _shieldPoints = value;
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

            _damageBuffers = value;
        }
    }

    public static bool Unvulnerable {get; private set;} = false;
    public static bool Hover {get; set;} = false; // невосприимчивость к контактному урону и невозможность таранов
    public static bool Active {get; private set;} = false;
    public static bool CriticalState {get; private set;} = false;
    public static int CriticalStateBorder => _criticalStateBorder;
    public static int MaxHP => _hpCap;
    public static bool OneShotProtection {get; private set;} = true;
    public static float GameTimerBuffer {get; private set;} = 0f;

    private static int _hitPoints;
    private static int _criticalStateBorder = 15;
    private static int _shieldPoints;
    private static int _structuralDamage;
    private static int _damageBuffers;
    private static int _hpCap;
    private static float _unvulnerableTimeAfterDamage = 0.1f;
    private static int _flatArmor;
    private static float _damageReduction;
    // private static PlayerUI _playerUI;
    private static ShieldComponent _shield;
    private static DamageBufferComponent _damageBuffer;

    public void Initialize(int hp, int arm, int shipID)
    {
        // _playerUI = SceneStatics.UICore.GetComponent<PlayerUI>();
        // _playerUI.ToggleShield(false);

        Unvulnerable = false;
        Active = true;
        Hover = false;
        _criticalStateBorder = 15;
        OneShotProtection = true;
        GameTimerBuffer = 0f;

        _hpCap = Mathf.Max(1, hp);
        _flatArmor = Mathf.Max(0, arm);
        _damageReduction = 0f;
        _shieldPoints = 0;
        _damageBuffers = 0;
        _structuralDamage = 0;

        SetHitPoints(_hpCap);

        // _playerUI.MaxHP = _hpCap;

        // Legacy save writing is disabled for the PC rebuild.
        // VictoryHandler.LevelVictored += WriteSaveData;
    }
    private void OnDisable() {
        // VictoryHandler.LevelVictored -= WriteSaveData;
    }

    // private void CheckOtherShip(int startHP, int startArm, int shipID)
    // {
    //     // GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

    //     // if (save.SessionInitialized && save.ShipModel != shipID)
    //     // {
    //     //     _hpCap = startHP;
    //     //     HitPoints = _hpCap;
    //     //     WriteSaveData();
    //     //     return;
    //     // }

    //     // if (save.SessionInitialized)
    //     // {
    //     //     _hpCap = save.MaxHealth;
    //     //     HitPoints = save.HealthPoints;
    //     // } 
    //     // else 
    //     // {
    //     //     HitPoints = _hpCap;
    //     //     WriteSaveData();
    //     // }
    // }

    // public static void LoadHealth()
    // {
    //     // GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
    //     // HitPoints = save.HealthPoints;
    // }

    public static bool TakeDamage(DamageBundle damageBundle, out bool killed)
    {
        killed = false;

        if (Unvulnerable || !Player.Alive || !damageBundle.Legitime || (damageBundle.damageKey != DamageKey.Player && damageBundle.damageKey != DamageKey.Everything))
            return false;

        if (SceneStatics.GameTimer - GameTimerBuffer < _unvulnerableTimeAfterDamage * RR.Get(RuntimeStat.UnvulnerableTimeAfterDamageMultiplier))
        {
            return false;
        }
        GameTimerBuffer = SceneStatics.GameTimer;

        for (int i = 0; i < damageBundle.cycles; i++)      
        {
            if (!Active) break;

            AnyDamageTaked?.Invoke();

            if (damageBundle.damageValue > 0)
            {
                if (DamageBuffers > 0)
                {
                    DamageBuffers --;
                    DamageBuffer1Losed?.Invoke();
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
                        ShieldDamageTaked?.Invoke(tempDmg);
                        continue;
                    }
                }

                tempDmg = Mathf.CeilToInt(tempDmg - Mathf.Max(0, FlatArmor - damageBundle.armorPenetration) * Mathf.Max(0f, 1f - DamageReduction)) + StructuralDamage;

                if (tempDmg <= 0)
                {
                    tempDmg = 1;
                }

                // _playerUI.PlayTakingDamage(tempDmg, FlatArmor);

                // ONE SHOT PROTECTION
                if (OneShotProtection && !damageBundle.ignoreOneShotProtection && HitPoints > CriticalStateBorder && tempDmg > (HitPoints - CriticalStateBorder))
                {
                    ParryingHandler.ConstParry();
                    HealthDamageTaked?.Invoke(HitPoints - CriticalStateBorder);
                    SetHitPoints(CriticalStateBorder);
                }
                // -------------------
                else
                {
                    HealthDamageTaked?.Invoke(tempDmg);
                    SetHitPoints(HitPoints - tempDmg);
                }
            }

            StructuralDamage += damageBundle.structuralDamage;

            // if (damageBundle.stunAmount > 0f)
            // {
            //     TakeStun(damageBundle.stunAmount);
            // }

            if (damageBundle.regeneratingValue > 0)
            {
                RegenerateHP(damageBundle.regeneratingValue);
            }
        }

        CheckState();
        return true;
    }

    public static void SetCriticalBorder(int newValue)
    {
        _criticalStateBorder = newValue;
        SetHitPoints(HitPoints);
    }

    public static void ToggleOneShotProtection(bool tog)
    {
        OneShotProtection = tog;
    }

    private static void SetHitPoints(int newValue)
    {
        if (!Active)
            return;
            
        if (newValue > _hpCap)
            newValue = _hpCap;

        else if (newValue < 0)
            newValue = 0;

        HitPoints = newValue;

        if (HitPoints <= 0)
        {
            Death();
        }
    }

    public static void RegenerateHP(int addingValue)
    {
        // if (_playerUI != null)
        //     _playerUI.PlayRecuperation();

        SetHitPoints(HitPoints + addingValue);
        Regenerated?.Invoke(addingValue);
    }

    public static void RepairStructureDamage(int amount)
    {
        if (amount <= 0)
            return;
        StructuralDamage = Mathf.Max(StructuralDamage - amount, 0);
        CheckState();
    }

    public static void ConsumeHP(int takingValue)
    {
        SetHitPoints(HitPoints - takingValue);
    }

    public static void GetShield(int shieldPoints)
    {
        if (!Active)
            return;

        if (_shield != null)
            BreakShield();

        if (shieldPoints == 0)
        {
            return;
        }
        
        _shield = ShieldDistributor.SpawnShield(Player.PlayerTransform, shieldPoints);
        ShieldPoints = shieldPoints;
        ShieldCreated?.Invoke();

        // _playerUI.ToggleShield(true);
        // _playerUI.SetShieldPoints(ShieldPoints);
    }

    private static void BreakShield()
    {
        if (_shield != null)
            _shield.BreakShield();
        _shield = null;
        ShieldBreaked?.Invoke();
        // _playerUI.ToggleShield(false);
    }

    public static void AddDamageBuffer(int db)
    {
        if (!Active)
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

    private static void CheckState()
    {
        if ((HitPoints - StructuralDamage <= _criticalStateBorder) && !CriticalState)
        {
            TimeHandler.CriticalState = true;
            CriticalState = true;
            CriticalStateChanged?.Invoke(true);
            // _playerUI.SetCriticalState(true);

            if (!OneShotProtection || !ParryingHandler.Initialized)
                return;

            ParringObject exp = ParryingHandler.GetParringObject(1);
            exp.transform.position = Player.PlayerTransform.position;
        }
        else if ((HitPoints - StructuralDamage >= _criticalStateBorder) && CriticalState)
        {
            TimeHandler.CriticalState = false;
            CriticalState = false;
            CriticalStateChanged?.Invoke(false);
            // _playerUI.SetCriticalState(false);
        }
    }

    public static void MultiplyHP(float multiplier)
    {
        print($"multipliying HP ({multiplier}) from {_hpCap} to {Mathf.CeilToInt(_hpCap * multiplier)}");
        _hpCap = Mathf.CeilToInt(_hpCap * multiplier);
        HitPoints = Mathf.CeilToInt(HitPoints * multiplier);

        // _playerUI.MaxHP = _hpCap;
    }

    private static void ToggleInvulnerability(bool tog)
    {
        Unvulnerable = tog;
        UnvulnerabilityToggled?.Invoke(tog);
    }

    public static void TryToggleInvulnerability(bool tog)
    {
        ToggleInvulnerability(tog);
    }

    public static void DeactivateAllBindedSystems()
    {
        print("---Deactivate all binded systems to PlayerShip data");
        Active = false;
        PlayerController.CanControl = false;
        ToggleInvulnerability(true);

        //PlayerDeathed?.Invoke();
    }

    private static void Death()
    {
        ReviveManager.TryRevive();

        DeactivateAllBindedSystems();
        PlayerDeathed?.Invoke();
        SceneStatics.UICore.GetComponent<DeathUIHandler>().Death();
    }

    private static void WriteSaveData()
    {
        // Legacy save writing is intentionally disabled.
        // GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
        // save.HealthPoints = HitPoints;
        // save.MaxHealth = _hpCap;
        // print($"---save hp : {HitPoints}");
        // GameSessionInfoHandler.RewriteSessionSave(save);
    }

    // #if UNITY_EDITOR
    private void Update() {
        // if (Input.GetKeyDown(KeyCode.Alpha9))
        // {
        //     TakeDamage(999);
        // }
        // if (Input.GetKeyDown(KeyCode.H))
        // {
        //     RegenerateHP(3);
        // }
        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     TakeDamage(10);
        // }
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
