using UnityEngine;

public class MMShip : MonoBehaviour
{
    private const int ruStore_skin_id = 1;

    [SerializeField] private ShipSkinPack[] _shipSkinsByID;

    private void Start()
    {
        int skinID = Dev.RuStoreVersionSprites ? ruStore_skin_id : 0; 
        GetComponent<SpriteRenderer>().sprite = _shipSkinsByID[GameSessionInfoHandler.GetSessionSave().ShipModel].GetSkin(skinID);
    }
}
