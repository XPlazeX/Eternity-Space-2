using UnityEngine;

public class NukeOnArmorBrake : Module
{
    [SerializeField] private int _damageDeal;
    [SerializeField] private SoundObject _explosionSound;

    private bool _active = false;
    private bool _loaded = false;
    private bool _triggered = false;

    public override void Load()
    {
        PlayerShipData.ChangeArmor += OnArmorChanged;
        _loaded = true;

        // OnArmorChanged(PlayerShipData.ArmorPoints);
    }

    private void OnDisable() {
        if (_loaded)
         PlayerShipData.ChangeArmor -= OnArmorChanged;
    }

    public void OnArmorChanged(int arm)
    {
        if (arm <= 0 && _active && !_triggered)
        {
           Explode();
           _triggered = true;
        }

        _active = arm > 0;
    }

    public void Explode()
    {
        SceneStatics.UICore.GetComponent<PlayerUI>().Flash();
        SoundPlayer.PlayUISound(_explosionSound);

        ParryingHandler.ConstParry();

        DamageBody[] dbs = GameObject.FindObjectsOfType<DamageBody>();
        for (int i = 0; i < dbs.Length; i++)
        {
            if (dbs[i].GetType() == typeof(PlayerDamageBody))
                return;

            dbs[i].TakeDamage(new DamageSystem.DamageBundle()
            {
                damageKey = DamageSystem.DamageKey.Everything,
                damageValue = _damageDeal,
                ignoreOneShotProtection = true
            }, out bool killed);
        }

        Nuke.TriggerExplodeEvent();
    }
}
