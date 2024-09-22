using UnityEngine;
using StatsManipulating;

public class Augmentation : Module
{
    [SerializeField] private string _collectionName;
    [SerializeField] private bool _recycleModulas;
    [SerializeField] private bool _recycleWeaponlevel;
    [SerializeField] private StatOperator[] _operatorsPerRecycling;
    [SerializeField] private int _armorUpgradePerRecycling;

    public override void Asquiring()
    {
        int recycles = 0;

        if (_recycleModulas)
        {
            recycles += ModulasSaveHandler.GetSave().GetAllLevelEvents().Count;
            ModulasSaveHandler.ClearPassiveModules(false);
        }
        if (_recycleWeaponlevel)
        {
            GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
            recycles += save.WeaponLevel;
            save.WeaponLevel = 0;
            GameSessionInfoHandler.RewriteSessionSave(save);
        }

        Debug.Log($"Recycles: {recycles}");
        if (GameSessionInfoHandler.ExistDataCollection(_collectionName))
        {
            GameSessionInfoHandler.ReplaceValueInCollection(_collectionName, 0, GameSessionInfoHandler.GetDataCollection(_collectionName)[0] + recycles);
        } else
        {
            GameSessionInfoHandler.AddDataCollection(_collectionName, new System.Collections.Generic.List<int>());
            GameSessionInfoHandler.AddValueToCollection(_collectionName, recycles);
        }

        GameObject.FindObjectOfType<WeaponService>().CheckLevel();
    }

    public override void Load()
    {
        int recycles = GameSessionInfoHandler.GetDataCollection(_collectionName)[0];
        //Debug.Log(recycles);

        for (int i = 0; i < recycles; i++)
        {
            for (int j = 0; j < _operatorsPerRecycling.Length; j++)
            {
                _operatorsPerRecycling[j].Enforce();
            }
        }

        if (_armorUpgradePerRecycling != 0)
            PlayerShipData.UpgradeArmor(_armorUpgradePerRecycling * recycles, _armorUpgradePerRecycling * recycles);
    }
}
