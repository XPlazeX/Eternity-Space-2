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
            hp += Mathf.FloorToInt(save.MaxHealth * _repairPart);
            hp += _repairFlat;

            save.HealthPoints = hp;
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
