using UnityEngine;

public class AsqRepairing : Module
{
    public override void Asquiring()
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
        save.HealthPoints = save.MaxHealth;

        save.HealsCount = 0;

        GameSessionInfoHandler.RewriteSessionSave(save);
        GameSessionInfoHandler.SaveAll();

        RepairService rs = GameObject.FindObjectOfType<RepairService>();
        if (rs != null)
            rs.CheckLevel();
    }
}
