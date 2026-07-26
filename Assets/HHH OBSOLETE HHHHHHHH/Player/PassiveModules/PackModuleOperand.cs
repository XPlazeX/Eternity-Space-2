using UnityEngine;

[CreateAssetMenu(fileName = "PackModuleOperand", menuName = "Teleplane2/PackModuleOperand", order = 0)]
public class PackModuleOperand : ModuleOperand
{
    [SerializeField] private int _packID;
    public int PackID => _packID;
}
