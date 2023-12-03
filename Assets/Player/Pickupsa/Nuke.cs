using UnityEngine;

public class Nuke : MonoBehaviour
{
    [SerializeField] private int _damageDeal;
    [SerializeField] private SoundObject _explosionSound;
    private bool _playerIn;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            _playerIn = true;
            print("enter");
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            _playerIn = true;
            print("exit");
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

        if (!_playerIn)
            PlayerShipData.TakeDamage(60);
        else
            ParryingHandler.ConstParry();

        DamageBody[] dbs = GameObject.FindObjectsOfType<DamageBody>();
        for (int i = 0; i < dbs.Length; i++)
        {
            if (dbs[i].GetType() == typeof(PlayerDamageBody))
                return;

            dbs[i].TakeDamage(_damageDeal / 2);
            dbs[i].TakeDamage(_damageDeal / 2);
        }
    }
}
