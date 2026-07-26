using UnityEngine;
using ModuleWork;
using System;

public class CharacterModules : MonoBehaviour
{
    [SerializeField] private ModuleOperand[] moduleOperands;
    [SerializeField] private int[] randomizedExclusions;

    public int ModuleCount => moduleOperands.Length;
    public Gear GetGear(GearType type, string name)
    {
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
        return moduleOperands[id];
    }

    public Module GetModule(int id)
    {
        return moduleOperands[id].HandingModule;
    }

    public int GetRandomModuleID()
    {
        while (true)
        {
            int result = UnityEngine.Random.Range(0, moduleOperands.Length);
            if (!Array.Exists(randomizedExclusions, element => element == result))
            {
                return result;
            }
        }
    }
}
