using UnityEngine;

public class MMShip : MonoBehaviour
{
    [SerializeField] private ShipSkinPack[] _shipSkinsByID;

    public void RenderShip(int shipID)
    {
        GetComponent<SpriteRenderer>().sprite = _shipSkinsByID[shipID].GetSkin(Skins.SOCurrentSkin());
    }
}
