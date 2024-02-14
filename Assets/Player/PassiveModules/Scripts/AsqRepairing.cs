using UnityEngine;

public class AsqRepairing : Module
{
    [SerializeField] private bool _fullRepair = true;
    [SerializeField][Range(0, 1f)] private float _repairPart = 0f;
    [SerializeField] private int _repairFlat = 0;
    [Space()]
    [SerializeField] private bool _resetRepairs = true;

    public override void Asquiring()
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        if (_fullRepair)
            save.HealthPoints = save.MaxHealth;
        else
        {
            int hp = save.HealthPoints;
            print($"hp from save: {hp}");
            hp += Mathf.FloorToInt((float)save.MaxHealth * _repairPart);
            print($"repair percentaged: {Mathf.FloorToInt((float)save.MaxHealth * _repairPart)}");
            hp += _repairFlat;

            save.HealthPoints = Mathf.Clamp(hp, 0, save.MaxHealth);
        }

        if (_resetRepairs)
            save.HealsCount = 0;

        GameSessionInfoHandler.RewriteSessionSave(save);
        GameSessionInfoHandler.SaveAll();

        if (Player.Alive)
        {
            PlayerShipData.LoadHealth();
        }

        RepairService rs = GameObject.FindObjectOfType<RepairService>();
        if (rs != null)
            rs.CheckLevel();
    }
}
