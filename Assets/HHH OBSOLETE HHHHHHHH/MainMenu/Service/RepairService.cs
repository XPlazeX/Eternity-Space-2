using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RepairService : MonoBehaviour
{
    [SerializeField] private Button _btn;
    [SerializeField] private int _startPrice;
    [SerializeField] private int _priceStep;
    [SerializeField] private BuyButton _buyBtn;
    [SerializeField] private Text _repairPercentageLabel;
    [Space()]
    [SerializeField] private NanomachinesLabel _nmLabel;
    [SerializeField] private float _waitRefillTime;
    [SerializeField] private float _animationTime;
    [SerializeField] private SoundObject _healingSound;
    [SerializeField] private Button _weaponButton;

    public float RepairPart {get; private set;} = 0f;
    public float RepairBoostPerRepair {get; private set;} = 0.03f;

    private bool _blockHeal;

    private void Start() {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        if (ModulasSaveHandler.GetSave().BlockHeal)
            _blockHeal = true;

        CheckLevel();
    }

    public void Repair()
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        int oldHP = save.HealthPoints;

        save.HealthPoints += Mathf.CeilToInt((float)save.MaxHealth * RepairPart);
        if (save.HealthPoints > save.MaxHealth)
            save.HealthPoints = save.MaxHealth;
        save.HealsCount ++;
        GameSessionInfoHandler.RewriteSessionSave(save);
        GameSessionInfoHandler.SaveAll();

        CheckLevel();

        StartCoroutine(HealingAnimation(oldHP, save.HealthPoints, save.MaxHealth));
    }

    public void BlockHeal()
    {
        _blockHeal = true;
        _btn.interactable = false;
    }

    public void CheckLevel()
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
        int level = save.HealsCount;

        _buyBtn.ScalePrice(_startPrice + _priceStep * level);

        _btn.interactable = (save.HealthPoints < save.MaxHealth && !_blockHeal);

        RepairPart = GlobalSaveHandler.GetSave().RepairPart + GameSessionInfoHandler.GetSessionSave().RepairAdditivePart + (RepairBoostPerRepair * level);
        RepairPart = Mathf.Clamp01(RepairPart);

        if (RepairPart < 0.05f)
            RepairPart = 0.05f;
        
        _repairPercentageLabel.text = $"{RepairPart * 100f}%";
    }

    public void TakeDamage(int val)
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        save.HealthPoints -= val;

        if (save.HealthPoints < 0)
            save.HealthPoints = 0;

        GameSessionInfoHandler.RewriteSessionSave(save);
        GameSessionInfoHandler.SaveAll();

        _btn.interactable = (save.HealthPoints < save.MaxHealth && !_blockHeal);
    }

    #if UNITY_EDITOR
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            TakeDamage(11);
        }
    }
    #endif

    private IEnumerator HealingAnimation(int fromHP, int toHP, int maxHP)
    {
        float timer = _animationTime;
        float startHP = (float)fromHP;
        
        _nmLabel.SetSafetyValue(Mathf.RoundToInt(startHP), maxHP);

        _btn.interactable = false;

        SoundPlayer.PlayUISound(_healingSound);
        
        MenuNavigation menu = GameObject.FindWithTag("MenuNavigation").GetComponent<MenuNavigation>();
        menu.ToggleNavigation(false);
        _weaponButton.interactable = false;

        yield return new WaitForSecondsRealtime(_waitRefillTime);

        while (timer > 0)
        {
            _nmLabel.SetSafetyValue(Mathf.RoundToInt(startHP), maxHP);

            timer -= ESTime.unscaledDeltaTime;
            startHP = Mathf.Lerp((float)fromHP, (float)toHP, 1f - (timer / _animationTime));

            yield return null;
        }

        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
        _btn.interactable = (save.HealthPoints < save.MaxHealth && !_blockHeal);

        _nmLabel.CheckValue();
        menu.ToggleNavigation(true);
        _weaponButton.interactable = true;
    }
}
