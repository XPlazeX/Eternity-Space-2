using UnityEngine;
using GearCompatibility;

[System.Serializable]
[CreateAssetMenu(fileName = "Character", menuName = "Teleplane2/Character", order = 0)]
public class Character : ScriptableObject 
{
    private const int hp_boost_unlock_id = 552;
    private const int armor_unlock_id = 551;
    private const int armor_boost_unlock_id = 553;

    [SerializeField] private GameObject[] _skinnedShips;
    [SerializeField] private ShipClass _class;
    [Space()]
    [SerializeField] private int _hitPoints;
    [SerializeField] private int _boostedHitPoints;
    [Space()]
    [SerializeField] private int _armorPoints;
    [SerializeField] private int _boostedArmorPoints;
    [Space()]
    [SerializeField] private Module[] _handingModules;

    public GameObject GetSkinnedShip(int skinID)
    {
        if (_skinnedShips == null || _skinnedShips.Length == 0)
        {
            return null;
        }

        return _skinnedShips[Mathf.Clamp(skinID, 0, _skinnedShips.Length - 1)];
    }

    public ShipClass Class => _class; 

    public int HP 
    {
        get 
        {
            return Unlocks.HasUnlock(hp_boost_unlock_id) ? _boostedHitPoints : _hitPoints;
        }
    }
    public int ARM
    {
        get
        {
            if (!Unlocks.HasUnlock(armor_unlock_id))
                return 0;

            return Unlocks.HasUnlock(armor_boost_unlock_id) ? _boostedArmorPoints : _armorPoints;
        }
    }
    public Module[] HandingModules => _handingModules;
}
