using UnityEngine;
using DamageSystem;

public class ParringObject : PullableObject
{
    [SerializeField] private DamageKey _damageKey;
    [SerializeField] private bool _countParry = false;
    [SerializeField] private float _lifetime = 1f;
    [SerializeField] private SoundObject _sound;

    public DamageKey KeyDamage => _damageKey;
    private int _parriedObjects = 0;
    private bool _sendedMsg;
    private bool _sounded = false;
    private float _lifeTimer = 0f;

    protected override void SetDefaultStats()
    {
        _parriedObjects = 0;
        _sendedMsg = false;
        _lifeTimer = _lifetime;
        _sounded = false;
    }

    public virtual void OnTriggerEnter2D(Collider2D thing)
    {
        Bullet cathedBullet = thing.GetComponent<Bullet>();

        if (cathedBullet == null)
            return;

        if (cathedBullet.KeyDamage != KeyDamage && KeyDamage != DamageKey.Everything)
            return;


        cathedBullet.Parrying();

        if (!_countParry)
            return;

        ParryingHandler.Parry();
        _parriedObjects ++;

        if (!_sendedMsg && (_parriedObjects >= ShipStats.GetIntValue("BulletsForParry")))
        {
            ParryingHandler.BuffedParry();
            _sendedMsg = true;
        }
        //canHeal = false;
    }

    private void FixedUpdate() 
    {
        if (!_sounded)
        {
            SoundPlayer.PlaySound(_sound, transform.position);
            _sounded = true;
        }

        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer < 0)
        {
            Death();
        }
    }

    public virtual void Death()
    {
        gameObject.SetActive(false);
    }
}
