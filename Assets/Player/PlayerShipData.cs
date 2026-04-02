using UnityEngine;
using DamageSystem;

public class PlayerShipData : MonoBehaviour
{
    public delegate void playerAction();

    public static event playerAction PlayerDeath;
    public static event playerAction ShieldBreaked;
    public static event healthOperation TakeShieldDamage;
    public static event healthOperation TakeHealthDamage;
    public static event healthOperation LoseDamageBuffer;
    public static event healthOperation Regenerated;

    public static event healthOperation ChangeHealth;
    public static event healthOperation ChangeArmor;

    public static int HitPoints 
    {
        get {return _hitPoints;} 
        
        private set 
        {
            _hitPoints = value;
            CheckState();
            _playerUI.ChangeHP(_hitPoints, (float)_hitPoints / _hpCap);
            ChangeHealth?.Invoke(value);
        }
    }
    public static int StructuralDamage 
    {
        get {return _structuralDamage;} 
        
        private set 
        {
            _structuralDamage = value;
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
    public static bool Hover {get; set;} = false;
    public static bool Active {get; private set;} = false;
    public static bool CriticalState {get; private set;} = false;
    public static int CriticalStateBorder => _criticalStateBorder;
    public static int MaxHP => _hpCap;
    public static bool OneShotProtection {get; private set;} = true;
    public static float GameTimerBuffer {get; private set;} = 0f;

    private static int _hitPoints;
    private static int _criticalStateBorder = 15;
    private static int _armorPoints;
    private static int _shieldPoints;
    private static int _structuralDamage;
    private static int _damageBuffers;
    private static int _hpCap;
    // private static int _armorCap;
    private static int _flatArmor;
    private static float _damageReduction;
    private static PlayerUI _playerUI;
    private static ShieldComponent _shield;
    private static DamageBufferComponent _damageBuffer;

    public void Initialize(int hp, int arm, int shipID)
    {
        _playerUI = SceneStatics.UICore.GetComponent<PlayerUI>();
        _playerUI.ToggleShield(false);

        Unvulnerable = false;
        Active = true;
        Hover = false;
        _criticalStateBorder = 15;
        OneShotProtection = true;
        GameTimerBuffer = 0f;

        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
        
        _hpCap = 100;
        // _armorCap = arm;

        // CheckOtherShip(hp, arm, shipID);
        SetHitPoints(100);

        _playerUI.MaxHP = _hpCap;

        VictoryHandler.LevelVictored += WriteSaveData;
    }
    private void OnDisable() {
        VictoryHandler.LevelVictored -= WriteSaveData;
    }

    private void CheckOtherShip(int startHP, int startArm, int shipID)
    {
        // GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        // if (save.SessionInitialized && save.ShipModel != shipID)
        // {
        //     _hpCap = startHP;
        //     HitPoints = _hpCap;
        //     WriteSaveData();
        //     return;
        // }

        // if (save.SessionInitialized)
        // {
        //     _hpCap = save.MaxHealth;
        //     HitPoints = save.HealthPoints;
        // } 
        // else 
        // {
        //     HitPoints = _hpCap;
        //     WriteSaveData();
        // }
    }

    public static void LoadHealth()
    {
        // GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
        // HitPoints = save.HealthPoints;
    }

    public static bool TakeDamage(DamageBundle damageBundle)
    {
        if (Unvulnerable || !Player.Alive || !damageBundle.Legitime || (damageBundle.damageKey != DamageKey.Player && damageBundle.damageKey != DamageKey.Everything))
            return false;

        // (SceneStatics.GameTimer - GameTimerBuffer < ShipStats.GetValue("UnvulnerableTimeAfterDamage"))

        for (int i = 0; i < damageBundle.cycles; i++)      
        {
            if (!Active) break;

            if (damageBundle.damageValue > 0)
            {
                if (DamageBuffers > 0)
                {
                    DamageBuffers --;
                    LoseDamageBuffer?.Invoke(1);
                    continue;
                }

                int tempDmg = damageBundle.damageValue;

                if (ShieldPoints > 0)
                {
                    tempDmg = Mathf.CeilToInt(damageBundle.shieldDamageMultiplier * tempDmg) - ShieldPoints;

                    if (tempDmg >= 0)
                    {
                        ShieldPoints = 0;
                        ShieldBreaked?.Invoke();
                        continue;
                    } else
                    {
                        ShieldPoints = -tempDmg;
                        TakeShieldDamage?.Invoke(tempDmg);
                        continue;
                    }
                }

                tempDmg = Mathf.CeilToInt(tempDmg - Mathf.Max(0, FlatArmor - damageBundle.armorPenetration) * Mathf.Max(0f, 1f - DamageReduction)) + StructuralDamage;

                if (tempDmg <= 0)
                {
                    tempDmg = 1;
                }

                _playerUI.PlayTakingDamage(tempDmg, FlatArmor);

                if (OneShotProtection && !damageBundle.ignoreOneShotProtection && HitPoints > CriticalStateBorder && tempDmg > (HitPoints - CriticalStateBorder))
                {
                    ParryingHandler.ConstParry();
                    SetHitPoints(CriticalStateBorder);
                }
                else
                {
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

        // FightSoundHelper.PlaySound(0, transform.position);
        CheckState();
        return true;

        // damage = Mathf.CeilToInt(ShipStats.GetValue("TakingDamageMultiplier") * damage);
        // if (damage > ShipStats.GetIntValue("MaxDamageTaken"))
        // {
        //     damage = ShipStats.GetIntValue("MaxDamageTaken");
        // }

        // GameTimerBuffer = SceneStatics.GameTimer;

        // int tempDmg = damage - ShipStats.GetIntValue("BlockArmor");

        // if (tempDmg < 1)
        //     tempDmg = 1;
        // else if (tempDmg >= 100)
        // {
        //     Unlocks.NewUnlock(923);
        // }

        // TakeAnyDamage?.Invoke(tempDmg);

        // if (ShieldPoints != 0)
        // {
        //     tempDmg = damage - ShieldPoints;

        //     if (tempDmg >= 0)
        //         ShieldPoints = 0;

        //     else
        //     {
        //         ShieldPoints = -tempDmg;
                

        //         _playerUI.SetShieldPoints(ShieldPoints);
        //         return;
        //     }
        // }

        // if (tempDmg == 0)
        //     return;

        // _playerUI.PlayTakingDamage(tempDmg, damage - tempDmg);

        // int lastARM = ArmorPoints - Mathf.CeilToInt((tempDmg));

        // if (lastARM >= 0) // урон по ХП не прошёл
        // {
        //     TakeArmorDamage?.Invoke(ArmorPoints - lastARM);
        //     SetArmorPoints(lastARM);
        // }
        // else // урон по ХП прошёл
        // {
        //     if (ArmorPoints > 0)
        //     {
        //         TakeArmorDamage?.Invoke(ArmorPoints);
        //         SetArmorPoints(0);
        //         // armor break
        //     }

        //     TakeHealthDamage?.Invoke(-lastARM);

        //     SetHitPoints(HitPoints + lastARM);
        // }
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
        // if ((OneShotProtection && (HitPoints >= _criticalStateBorder) && (newValue <= 0)) || newValue == 0)
        // {
        //     ParryingHandler.ConstParry();
        //     HitPoints = 0;
        // }
        // else
        HitPoints = newValue;

        if (HitPoints <= 0)
        {
            // HitPoints = -1;
            Death();
        }
    }

    public static void RegenerateHP(int addingValue)
    {
        if (_playerUI != null)
            _playerUI.PlayRecuperation();

        SetHitPoints(HitPoints + addingValue);
        Regenerated?.Invoke(addingValue);

        // if (VictoryHandler.LevelVictoried)
        // {
        //     WriteSaveData();
        // }
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
        //_playerUI.PlayTakingDamage(1, 0);

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
            //print("noneShield");
            return;
        }
        
        _shield = ShieldDistributor.SpawnShield(Player.PlayerTransform, shieldPoints);
        ShieldPoints = shieldPoints;

        _playerUI.ToggleShield(true);
        _playerUI.SetShieldPoints(ShieldPoints);
    }

    private static void BreakShield()
    {
        if (_shield != null)
            _shield.BreakShield();
        _shield = null;
        _playerUI.ToggleShield(false);
    }

    public static void AddDamageBuffer(int db)
    {
        if (!Active)
            return;

        if (db == 0)
        {
            //print("noneShield");
            return;
        }
        
        if (_damageBuffer == null)
        {
            _damageBuffer = ShieldDistributor.SpawnDamageBuffer(Player.PlayerTransform);
        }
        
        DamageBuffers += db;
    }

    private static void BreakDamageBuffer()
    {
        if (_damageBuffer != null)
            _damageBuffer.BreakShield();
        
        _damageBuffer = null;
        // _playerUI.ToggleShield(false);
    }

    private static void CheckState()
    {
        if ((HitPoints - StructuralDamage <= _criticalStateBorder) && !CriticalState)
        {
            TimeHandler.CriticalState = true;
            CriticalState = true;
            _playerUI.SetCriticalState(true);

            if (!OneShotProtection || !ParryingHandler.Initialized)
                return;

            ParringObject exp = ParryingHandler.GetParringObject(1);
            exp.transform.position = Player.PlayerTransform.position;
        }
        else if ((HitPoints - StructuralDamage >= _criticalStateBorder) && CriticalState)
        {
            TimeHandler.CriticalState = false;
            CriticalState = false;
            _playerUI.SetCriticalState(false);
        }
    }

    public static void MultiplyHP(float multiplier)
    {
        print($"multipliying HP ({multiplier}) from {_hpCap} to {Mathf.CeilToInt(_hpCap * multiplier)}");
        _hpCap = Mathf.CeilToInt(_hpCap * multiplier);
        HitPoints = Mathf.CeilToInt(HitPoints * multiplier);

        _playerUI.MaxHP = _hpCap;
    }

    private static void ToggleInvulnerability(bool tog)
    {
        Unvulnerable = tog;
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

        //PlayerDeath?.Invoke();
    }

    private static void Death()
    {
        ReviveManager.TryRevive();

        DeactivateAllBindedSystems();
        PlayerDeath?.Invoke();
        SceneStatics.UICore.GetComponent<DeathUIHandler>().Death();
    }

    private static void WriteSaveData()
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
        save.HealthPoints = HitPoints;
        save.MaxHealth = _hpCap;
        //save.ArmorPart = (float)ArmorPoints / _armorCap;
        print($"---save hp : {HitPoints}");

        GameSessionInfoHandler.RewriteSessionSave(save);
    }

    #if UNITY_EDITOR
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
        if (Input.GetKeyDown(KeyCode.A))
        {
            // RegenerateArmor(10);
        }
    }
    #endif

}
