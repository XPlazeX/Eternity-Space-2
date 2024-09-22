using UnityEngine;

public class DisableHeal : Module
{
    public override void MissionMenuLoad()
    {
        Debug.Log("Disable heal");
        ModulasSave msave = ModulasSaveHandler.GetSave();
        msave.BlockHeal = true;
        ModulasSaveHandler.RewriteSave(msave);

        RepairService rs = GameObject.FindObjectOfType<RepairService>();
        if (rs != null)
            rs.BlockHeal();
    }
}
