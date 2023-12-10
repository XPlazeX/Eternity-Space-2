using UnityEngine;
using DamageSystem;

public class PlayerShipData : MonoBehaviour
{
    public delegate void playerAction();

    public static event playerAction PlayerDeath;
    public static event healthOperation TakeArmorDamage;
    public static event healthOperation TakeHealthDamage;
    public static event healthOperation ChangeHealth;

    [SerializeField] private bool _testMode = false;
    [SerializeField] private int _loadingHP = 100;
    [SerializeField] private int _loadingArmorCap = 50;

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
    public static int ArmorPoints
    {
        get {return _armorPoints;} 
        
        private set 
        {
            _armorPoints = value;
            CheckState();
            _playerUI.ChangeARM(_armorPoints, (float)_armorPoints / _hpCap);
        }
    }
    public static int ShieldPoints
    {
        get {return _shieldPoints;}
        set {
            if (value == 0 && _shieldPoints > 0)
                BreakShield();

            _shieldPoints = value;
        }
    }
    public static bool Invulnerable {get; private set;} = false;
    public static bool Hover {get; set;} = false;
    public static bool Active {get; private set;} = true;
    public static bool CriticalState {get; private set;} = false;
    public static int MaxHP => _hpCap;
    public static bool OneShotProtection {get; private set;} = true;
    public static float GameTimerBuffer {get; private set;} = 0f;

    private static int _hitPoints;
    private static int _criticalStateBorder = 15;
    private static int _armorPoints;
    private static int _shieldPoints;
    private static int _hpCap;
    private static int _armorCap;
    private static PlayerUI _playerUI;
    private static ShieldComponent _shield;

    public void Initialize(int hp, int arm)
    {
        _playerUI = SceneStatics.UICore.GetComponent<PlayerUI>();
        _playerUI.ToggleShield(false);

        Invulnerable = false;
        Active = true;
        Hover = false;
        GameTimerBuffer = 0f;

        if (_testMode)
        {
            _hpCap = _loadingHP;
            _armorCap = _loadingArmorCap;
        }

        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
        
        _hpCap = hp;
        _armorCap = arm;
        _playerUI.MaxHP = _hpCap;

        if (save.SessionInitialized)
        {
            _hpCap = save.MaxHealth;
            HitPoints = save.HealthPoints;
        } 
        else 
        {
            HitPoints = _hpCap;
            WriteSaveData();
        }

        ArmorPoints = arm;
        VictoryHandler.LevelVictored += WriteSaveData;
    }
    private void OnDisable() {
        VictoryHandler.LevelVictored -= WriteSaveData;
    }

    public static void TakeDamage(int damage)
    {
        if ((damage <= 0) || (Invulnerable) || (SceneStatics.GameTimer - GameTimerBuffer < ShipStats.GetValue("UnvulnerableTimeAfterDamage")))
            return;

        damage = Mathf.CeilToInt(ShipStats.GetValue("TakingDamageMultiplier") * damage);
        if (damage > ShipStats.GetIntValue("MaxDamageTaken"))
        {
            damage = ShipStats.GetIntValue("MaxDamageTaken");
        }

        GameTimerBuffer = SceneStatics.GameTimer;

        int tempDmg = damage - ShipStats.GetIntValue("BlockArmor");

        if (ShieldPoints != 0)
        {
            tempDmg = damage - ShieldPoints;

            if (tempDmg >= 0)
                ShieldPoints = 0;

            else
            {
                ShieldPoints = -tempDmg;
                _shield.UpdateSP(ShieldPoints);

                _playerUI.SetShieldPoints(ShieldPoints);
                return;
            }
        }

        if (tempDmg == 0)
            return;

        _playerUI.PlayTakingDamage(tempDmg, damage - tempDmg);

        int lastARM = ArmorPoints - Mathf.CeilToInt((tempDmg));

        if (lastARM >= 0) // урон по ХП не прошёл
        {
            TakeArmorDamage?.Invoke(ArmorPoints - lastARM);
            SetArmorPoints(lastARM);
        }
        else // урон по ХП прошёл
        {
            if (ArmorPoints > 0)
            {
                TakeArmorDamage?.Invoke(ArmorPoints);
                SetArmorPoints(0);
                // armor break
            }

            TakeHealthDamage?.Invoke(-lastARM);

            SetHitPoints(HitPoints + lastARM);
        }
    }

    private static void SetHitPoints(int newValue)
    {
        if (newValue > _hpCap)
            newValue = _hpCap;

        if (((OneShotProtection) && (HitPoints >= _criticalStateBorder) && (newValue <= 0)) || newValue == 0)
        {
            ParryingHandler.ConstParry();
            HitPoints = 0;
        }
        else
            HitPoints = newValue;

        if (HitPoints < 0)
        {
            HitPoints = -1;
            Death();
        }
    }

    private static void SetArmorPoints(int newValue)
    {
        if (newValue > _armorCap)
            newValue = _armorCap;

        ArmorPoints = newValue;
    }

    public static void UpgradeArmor(int additiveArmorCap, int additiveValue)
    {
        _armorCap += additiveArmorCap;
        ArmorPoints = Mathf.Clamp(ArmorPoints + additiveValue, 0, _armorCap);
    }

    public static void RegenerateArmor(int addingValue)
    {
        ArmorPoints += addingValue;

        if (ArmorPoints >= _armorCap)
            ArmorPoints = _armorCap;
    }

    public static void RegenerateHP(int addingValue)
    {
        _playerUI.PlayRecuperation();

        SetHitPoints(HitPoints + addingValue);
    }

    public static void ConsumeHP(int takingValue)
    {
        _playerUI.PlayTakingDamage(1, 0);

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

    private static void CheckState()
    {
        if ((HitPoints + ArmorPoints <= _criticalStateBorder) && !CriticalState)
        {
            TimeHandler.CriticalState = true;
            CriticalState = true;
            _playerUI.SetCriticalState(true);

            if (!OneShotProtection || !ParryingHandler.Initialized)
                return;

            ParringObject exp = ParryingHandler.GetParringObject(1);
            exp.transform.position = Player.PlayerTransform.position;
        }
        else if ((HitPoints + ArmorPoints >= _criticalStateBorder) && CriticalState)
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

    public static void MultiplyARM(float multiplier)
    {
        print($"multipliying ARM ({multiplier}) from {_armorCap} to {Mathf.CeilToInt(_armorCap * multiplier)}");
        _armorCap = Mathf.CeilToInt(_armorCap * multiplier);
        ArmorPoints = Mathf.CeilToInt(ArmorPoints * multiplier);
    }

    private static void ToggleInvulnerability(bool tog)
    {
        Invulnerable = tog;
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
        DeactivateAllBindedSystems();
        PlayerDeath?.Invoke();
        SceneStatics.UICore.GetComponent<DeathUIHandler>().Death();
    }

    private void WriteSaveData()
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
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            TakeDamage(999);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            RegenerateHP(3);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            TakeDamage(20);
        }
    }
    #endif

}
