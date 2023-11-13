using UnityEngine;
using BankSystem;

public class Bank : MonoBehaviour
{
    public delegate void valueChanged();
    public static event valueChanged BankUpdated;

    public static bool EnoughtCash(Currency currency, int value)
    {
        switch (currency)
        {
            case Currency.Cosmilite:
                return GlobalSaveHandler.GetSave().Cosmilite >= value;
            case Currency.Positronium:
                return GlobalSaveHandler.GetSave().Positronium >= value;
            case Currency.Aurite:
                return GameSessionInfoHandler.GetSessionSave().Money >= value;
            default:
                return false;
        }
    }

    public static void PutCash(Currency currency, int value)
    {
        if (value < 0)
            throw new System.Exception("Для снятия денег используйте метод Consume.");

        GlobalSave globalSave = GlobalSaveHandler.GetSave();

        switch (currency)
        {
            case Currency.Cosmilite:
                globalSave.Cosmilite += value;
                GlobalSaveHandler.RewriteSave(globalSave);
                BankUpdated?.Invoke();
                break;
            case Currency.Positronium:
                globalSave.Positronium += value;
                GlobalSaveHandler.RewriteSave(globalSave);
                BankUpdated?.Invoke();
                break;
            case Currency.Aurite:
                GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
                save.Money += value;
                GameSessionInfoHandler.RewriteSessionSave(save);
                GameSessionInfoHandler.SaveAll();
                break;
            default:
                break;
        }
    }

    public static void ConsumeCash(Currency currency, int value)
    {
        if (value < 0)
            throw new System.Exception("Для пополнения денег используйте метод Put.");

        GlobalSave globalSave = GlobalSaveHandler.GetSave();

        switch (currency)
        {
            case Currency.Cosmilite:
                globalSave.Cosmilite -= value;
                GlobalSaveHandler.RewriteSave(globalSave);
                BankUpdated?.Invoke();
                break;
            case Currency.Positronium:
                globalSave.Positronium -= value;
                GlobalSaveHandler.RewriteSave(globalSave);
                BankUpdated?.Invoke();
                break;
            case Currency.Aurite:
                GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
                save.Money -= value;
                GameSessionInfoHandler.RewriteSessionSave(save);
                GameSessionInfoHandler.SaveAll();
                break;
            default:
                break;
        }
    }

    public void AddAurite(int val)
    {
        PutCash(Currency.Aurite, val);
    }

    public void ConsumeAurite(int val)
    {
        if (!EnoughtCash(Currency.Aurite, val))
            return;
            
        ConsumeCash(Currency.Aurite, val);
    }
}
