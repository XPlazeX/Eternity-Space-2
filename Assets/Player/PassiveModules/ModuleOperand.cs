using UnityEngine;

[CreateAssetMenu(fileName = "ModuleOperand", menuName = "Teleplane2/ModuleOperand", order = 0)]
public class ModuleOperand : ScriptableObject
{
    [SerializeField] private Module _module;
    [Space()]
    [SerializeField] private Sprite _icon;
    [SerializeField] private int _localizationID;

    public Module HandingModule => _module;
    public int LocalizationID => _localizationID;
    public Sprite Icon => _icon;
}

[System.Serializable]
public struct LevelEvent
{
    public int moduleOperandID;
    public bool isPack;
    public bool stackable;
    public bool destroyOnNextLevel;
    public int auritePrice;

    public UnlockRequire[] requiredUnlocks;
    public bool Unlocked => Unlocks.HasUnlocks(requiredUnlocks);
}
