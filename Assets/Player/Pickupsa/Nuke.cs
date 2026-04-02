using UnityEngine;

public class Nuke : MonoBehaviour
{
    public delegate void nukeAction();

    public static event nukeAction NukeExploded;

    [SerializeField] private int _damageDeal;
    [SerializeField] private SoundObject _explosionSound;
    [SerializeField] private bool _trackPlayer = true;
    private bool _playerIn;

    private void OnTriggerStay2D(Collider2D other) {
        if (!_trackPlayer)
            return;
        if (other.CompareTag("Player"))
        {
            _playerIn = true;
            //print("stay");
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (!_trackPlayer)
            return;
        if (other.CompareTag("Player"))
        {
            _playerIn = false;
            //print("exit");
        }
    }

    public void Blink()
    {
        SceneStatics.UICore.GetComponent<PlayerUI>().TriggerPreDeath();
    }

    public void Explode()
    {
        SceneStatics.UICore.GetComponent<PlayerUI>().Flash();
        SoundPlayer.PlayUISound(_explosionSound);

        if (!_playerIn && _trackPlayer)
            PlayerShipData.TakeDamage(new DamageSystem.DamageBundle()
            {
                damageKey = DamageSystem.DamageKey.Everything,
                damageValue = 80
            });
        else
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
            });
        }

        TriggerExplodeEvent();
    }

    public static void TriggerExplodeEvent()
    {
        NukeExploded?.Invoke();
    }
}
