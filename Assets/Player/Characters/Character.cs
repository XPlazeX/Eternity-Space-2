using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Character", menuName = "Teleplane2/Character", order = 0)]
public class Character : ScriptableObject 
{
    [SerializeField] private GameObject _playerShip;
    [SerializeField] private int _hitPoints;
    [SerializeField] private int _armorPoints;
    [SerializeField] private int _armorCap;
    [SerializeField] private Module[] _handingModules;

    public GameObject Ship => _playerShip;
    public int HP => _hitPoints;
    public int ARM => _armorPoints;
    public Module[] HandingModules => _handingModules;
}
