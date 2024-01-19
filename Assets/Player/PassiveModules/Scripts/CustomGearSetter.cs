using UnityEngine;

public class CustomGearSetter : Module
{
    [SerializeField] private string _customWeaponModel;
    [SerializeField] private int _customShipID = -1;
    [SerializeField] private bool _maxWeaponUpgrade;

    public override void Asquiring()
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        if (!string.IsNullOrEmpty(_customWeaponModel))
        {
            save.WeaponModel = _customWeaponModel;
        }    

        if (_customShipID != -1)
        {
            save.ShipModel = _customShipID;
        }

        GameSessionInfoHandler.RewriteSessionSave(save);

        if (_maxWeaponUpgrade)
        {
            WeaponService ws = GameObject.FindObjectOfType<WeaponService>();

            for (int i = 0; i < 5; i++)
            {
                ws.Upgrade();
            }
        }
    }
}
