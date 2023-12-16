using UnityEngine;
using ModuleWork;

[CreateAssetMenu(fileName = "ModuleOperand", menuName = "Teleplane2/ModuleOperand", order = 0)]
public class ModuleOperand : ScriptableObject
{
    [SerializeField] private Module _module;
    [Space()]
    [SerializeField] private Sprite _icon;
    [SerializeField] private int _localizationID;
    [Space()]
    [SerializeField] private ModuleStackType _stackType = ModuleStackType.NoneStacks;
    [SerializeField] private bool _oneLeveled = false;

    public Module HandingModule => _module;
    public int LocalizationID => _localizationID;
    public Sprite Icon => _icon;
    public bool OneLeveled => _oneLeveled;
    public ModuleStackType StackType => _stackType;
}

[System.Serializable]
public struct LevelEvent
{
    public int moduleOperandID;
    public int auritePrice;
    public UnlockRequire[] requiredUnlocks;

    [HideInInspector] public Module handingModule => SceneStatics.CharacterCore.GetComponent<CharacterModules>().GetModule(moduleOperandID);
    [HideInInspector] public ModuleOperand handingModuleOperand => SceneStatics.CharacterCore.GetComponent<CharacterModules>().GetModuleOperand(moduleOperandID);
    [HideInInspector] public bool unlocked => Unlocks.HasUnlocks(requiredUnlocks);
    [HideInInspector] public bool oneLeveled => handingModuleOperand.OneLeveled;
    [HideInInspector] public ModuleStackType stackType => handingModuleOperand.StackType;
}
