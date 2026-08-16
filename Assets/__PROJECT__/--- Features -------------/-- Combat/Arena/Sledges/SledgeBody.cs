using UnityEngine;
using DamageSystem;

public class SledgeBody : DamageBody
{
    [Header("Skedge Settings")]
    [SerializeField] private int jumperHealthBorder = 300;
    [SerializeField] private int engineHealthBorder = 150;
    [SerializeField] private int regeneratorHealthBorder = 75;
    [SerializeField] private int catcherHealthBorder = 0;
    [SerializeField] private int healthToRepair = 60;

    public bool EngineAvailiable => !IsStunned && CatcherAvailable;
    public bool JumperAvailable => HitPoints > jumperHealthBorder;
    public bool FullEnginePowerAvailiable => HitPoints > engineHealthBorder;
    public bool RegeneratorAvailiable => HitPoints > regeneratorHealthBorder;
    public bool CatcherAvailable => HitPoints > catcherHealthBorder;
    public bool IsAutoDeployState => _autoDeployState;

    protected override bool StackingByStructureDamage { get => false;}
    private bool _autoDeployState = false;

    public override int HitPoints 
    { 
        get => base.HitPoints; 
        protected set
        {
            base.HitPoints = value;
            if (_autoDeployState && base.HitPoints > healthToRepair)
            {
                _autoDeployState = false;
            }
        }
    }

    public override bool TakeDamage(DamageBundle damageBundle, out bool pkilled)
    {
        bool hit = base.TakeDamage(damageBundle, out pkilled);
        return hit;
    }

    public override void GetShield(int shieldPoints)
    {
        // PlayerShipData.GetShield(shieldPoints);
    }

    public override void GetDamageBuffer(int db)
    {
        // PlayerShipData.AddDamageBuffer(db);
    }

    public void RamDamageBody(DamageBody damageBody)
    {
        if ((damageBody == null) || (damageBody.KeyDamage == _damageKey) || (damageBody.KeyDamage == DamageSystem.DamageKey.Unvulnerable))
            return;

        if (damageBody.RamReady && (damageBody.GetType() != typeof(AsteroidBody)))
        {
            PlayerRamsHandler.TryRam();
            damageBody.TakeDamage(new DamageBundle()
            {
                damageKey = DamageKey.Everything,
                damageValue = PlayerRamsHandler.RamDamage,
                ignoreOneShotProtection = true
            }, out bool killed);
            return;
        }
    }

    protected override void Stun()
    {
        base.Stun();
    }

    protected override void Death(DamageBundle sourceBundle, int overdmg)
    {
        _autoDeployState = true;
        // base.Death(sourceBundle, overdmg);
    }

    public void OnTriggerEnter2D(Collider2D other) 
    {
        // if (PlayerShipData.Hover || other.GetComponent<Hover>() != null)
        //     return;

        DamageBody damageBody = other.GetComponent<DamageBody>();

        if ((damageBody == null) || (damageBody.KeyDamage == _damageKey) || (damageBody.KeyDamage == DamageSystem.DamageKey.Unvulnerable))
            return;

        int otherHP = damageBody.HitPoints;

        if (damageBody.RamReady && (damageBody.GetType() != typeof(AsteroidBody)))
        {
            // PlayerRamsHandler.TryRam();
            // damageBody.TakeDamage(new DamageBundle()
            // {
            //     damageKey = DamageKey.Everything,
            //     damageValue = PlayerRamsHandler.RamDamage,
            //     ignoreOneShotProtection = true
            // });
            return;
        }

        if (PlayerShipData.Unvulnerable)
            return;

        damageBody.TakeDamage(new DamageBundle()
        {
            damageKey = DamageKey.Everything,
            damageValue = ShipStats.GetIntValue("MaxDamageTaken"),
            ignoreOneShotProtection = true
        }, out bool killed);
        TakeDamage(new DamageBundle()
        {
            damageKey = DamageKey.Player,
            damageValue = otherHP
        }, out bool meKilled);
    }

    private void Update() {
        if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.D))
        {
            TakeDamage(new DamageBundle()
            {
                damageKey = DamageKey.Player,
                damageValue = 20,
                armorPenetration = 0,
                shieldDamageMultiplier = 1f,
                structuralDamage = 0,
                regeneratingValue = 0,
                stunAmount = 0f,
                cycles = 1
            }, out bool killed);
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(new DamageBundle()
            {
                damageKey = DamageKey.Player,
                damageValue = 0,
                armorPenetration = 0,
                shieldDamageMultiplier = 1f,
                structuralDamage = 0,
                regeneratingValue = 30,
                stunAmount = 0f,
                cycles = 1
            }, out bool killed);
        }
    }
}
