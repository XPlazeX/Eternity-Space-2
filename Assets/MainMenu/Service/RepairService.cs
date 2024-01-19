using UnityEngine;
using UnityEngine.UI;

public class RepairService : MonoBehaviour
{
    [SerializeField] private Button _btn;
    [SerializeField] private int _startPrice;
    [SerializeField] private int _priceStep;
    [SerializeField] private BuyButton _buyBtn;
    [SerializeField] private Text _repairPercentageLabel;

    public float RepairPart {get; private set;} = 0f;
    public float RepairBoostPerRepair {get; private set;} = 0.03f;

    private void Start() {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        CheckLevel();
    }

    public void Repair()
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
        save.HealthPoints += Mathf.CeilToInt((float)save.MaxHealth * RepairPart);
        if (save.HealthPoints > save.MaxHealth)
            save.HealthPoints = save.MaxHealth;
        save.HealsCount ++;
        GameSessionInfoHandler.RewriteSessionSave(save);
        GameSessionInfoHandler.SaveAll();

        CheckLevel();
    }

    public void CheckLevel()
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
        int level = save.HealsCount;

        _buyBtn.ScalePrice(_startPrice + _priceStep * level);

        if (save.HealthPoints >= save.MaxHealth || ModulasSaveHandler.GetSave().BlockHeal)
            _btn.interactable = false;

        RepairPart = GlobalSaveHandler.GetSave().RepairPart + GameSessionInfoHandler.GetSessionSave().RepairAdditivePart + (RepairBoostPerRepair * level);
        RepairPart = Mathf.Clamp01(RepairPart);
        
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
    }
}
