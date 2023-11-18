using UnityEngine;

public class OrcaShotRangeBoost : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string weaponModel = GameSessionInfoHandler.GetSessionSave().WeaponModel;
        print(weaponModel);

        if (weaponModel == "Spreader")
        {
            ShipStats.MultiplyStat("PlayerShotLifetimeMultiplier", 2f);
        }
    }

}
