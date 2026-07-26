using UnityEngine;

[System.Serializable]
public struct ShipSkinPack
{
    [SerializeField] private Sprite[] _skins;

    public Sprite GetSkin(int id) => _skins[id];
}
