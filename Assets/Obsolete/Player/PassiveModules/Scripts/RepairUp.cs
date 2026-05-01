using UnityEngine;

public class RepairUp : Module
{
    [SerializeField] private float _repairPartBoost;

    public override void Asquiring()
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        save.RepairAdditivePart += _repairPartBoost;

        GameSessionInfoHandler.RewriteSessionSave(save);
        GameSessionInfoHandler.SaveAll();

        RepairService rs = GameObject.FindObjectOfType<RepairService>();
        if (rs != null)
            rs.CheckLevel();
    }
}
