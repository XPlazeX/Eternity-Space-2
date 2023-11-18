using UnityEngine;
using UnityEngine.UI;

public class PlayerUI: MonoBehaviour
{
    public const int lowDamageAnimation = 5; 

    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _armorBar;
    [SerializeField] private GameObject _shieldObject;
    [SerializeField] private Text _healthLabel;
    [SerializeField] private Text _armorLabel;
    [SerializeField] private Text _shieldLabel;
    [Header("Animations")]
    [SerializeField] private Animator[] _damageTakenAnimators; 
    [SerializeField] private Animator _effectAnimator;
    [SerializeField] private Animator _stateAnimator;
    [Space()]
    [SerializeField] private Color _mainColor;
    [SerializeField] private Color[] _effectColors = new Color[6];
    [Header("Sounds")]
    [SerializeField] private SoundObject[] _damageTakens;
    [SerializeField] private SoundObject _manyDamageTaken;

    public enum Effect
    {
        Heal = 0,
        Parry = 1,
        LowDamage = 2,
        PowerUp = 3,
        Shielding = 4,
        Violet = 5
    }

    public int MaxHP {get; set;}

    public void ChangeHP(int nowHP, float partMax)
    {
        _healthBar.fillAmount = partMax;
        _healthLabel.text = nowHP.ToString() + " / " + MaxHP.ToString();
    }

    public void ChangeARM(int nowArm, float partHP)
    {
        _armorBar.fillAmount = partHP;
        _armorLabel.text = nowArm.ToString();
    }

    public void PlayTakingDamage(int value = 0, int flatBlock = 0)
    {
        if (value >= 20)
        {
            _damageTakenAnimators[0].SetTrigger("ManyDamage");
            SoundPlayer.PlayUISound(_manyDamageTaken);
            CameraController.Shake(1f, 2);
            PSEmitter.Emit(20);
            return;
        }
        if (EmissionHandler.EmissionReady)
        {
            _damageTakenAnimators[0].SetTrigger("AdrenalineBlock");
            return;
        }
        if ((value > 0) && (value <= lowDamageAnimation))
        {
            PlayEffect(Effect.LowDamage, 0.15f);
            
            PSEmitter.Emit(3);
            return;
        }
        int idAnim = Random.Range(0, _damageTakenAnimators.Length);
        _damageTakenAnimators[idAnim].SetTrigger("TakeDamage");
        SoundPlayer.PlayUISound(_damageTakens[idAnim]);
        PSEmitter.Emit(10);
        CameraController.Shake(0.5f);
    }

    public void ToggleShield(bool tog) => _shieldObject.SetActive(tog);
    public void SetShieldPoints(int sp) => _shieldLabel.text = sp.ToString();

    public void PlayRecuperation()
    {
        PlayEffect(Effect.Heal, 0.5f);
    }

    public void PlayEffect(Effect effect, float durationMultiplier = 1f)
    {
        _effectAnimator.GetComponent<Image>().color = _effectColors[(int)effect];
        _effectAnimator.speed = 1f / durationMultiplier;
        _effectAnimator.SetTrigger("TriggerEffect");
    }

    public void SetCriticalState(bool tog) => _stateAnimator.SetBool("CriticalState", tog);
}
