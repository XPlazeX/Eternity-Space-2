using UnityEngine;
using ModuleWork;

public class CharacterModules : MonoBehaviour
{
    [SerializeField] private ModuleOperand[] moduleOperands;

    public int ModuleCount => moduleOperands.Length;
    public Gear GetGear(GearType type, string name)
    {
        print($"{name}");
        switch (type)
        {       
            case GearType.Weapon:
                return Resources.Load<Gear>($"Gear/Weapons/{name}");

            case GearType.Device:
                return Resources.Load<Gear>($"Gear/Devices/{name}");
            
            default:
                throw new System.Exception("Неверный тип снаряжения.");
        }
    }

    public ModuleOperand GetModuleOperand(int id)
    {
        print($"return module op: {id}");
        return moduleOperands[id];
    }

    public Module GetModule(int id)
    {
        return moduleOperands[id].HandingModule;
    }
}
