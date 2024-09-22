using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    public delegate void energyAction(float mw);

    public static event energyAction Overcharged;
    public static event energyAction EnergyChanged;

    public static float Megawatts {get; private set;}
    public static float MaxMegawatts {get; private set;} // если их бустить, то через Asquire

    private static float _addedMw;

    public static void LoadMegawatts()
    {
        Megawatts = GameSessionInfoHandler.GetSessionSave().Megawatts;
        MaxMegawatts = GameSessionInfoHandler.GetSessionSave().MaxMegawatts;
    }

    public static void SaveMegawatts()
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
        save.Megawatts = Megawatts;
        GameSessionInfoHandler.RewriteSessionSave(save);

        Unlocks.ProgressUnlock(931, Mathf.RoundToInt(_addedMw * 1000)); // нужно 1 000 000
    }

    public static bool EnoughtEnergy(float mw) => Megawatts >= mw;

    public static void NullifyEnergy()
    {
        Megawatts = 0f;
        EnergyChanged?.Invoke(Megawatts);
    }

    public static void ConsumeEnergy(float mw)
    {
        if (!EnoughtEnergy(mw))
        {
            throw new System.Exception("Недостаточно энергии! проверяйте её наличие.");
        }

        Megawatts -= mw;
        EnergyChanged?.Invoke(Megawatts);
    }

    public static void AddEnergy(float mw)
    {
        Megawatts += mw;

        _addedMw += mw;

        if (Megawatts > MaxMegawatts)
        {
            Overcharged?.Invoke(Megawatts - MaxMegawatts);
            Megawatts = MaxMegawatts;
        }

        EnergyChanged?.Invoke(Megawatts);
    }
}
