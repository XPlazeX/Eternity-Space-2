using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModuleWork;

public class DebuffsHandler : MonoBehaviour
{
    // const int maxUniqueDebuffs = 6;

    // [SerializeField] private PassiveModuleSlot[] _strongDebuffsSlots;
    // [SerializeField] private PassiveModuleSlot[] _debuffsSlots;
    // [Space()]
    // [SerializeField] private int _addDebuffCount = 1;

    // private CharacterModules _characterModules;

    public void Initialize(bool isNewLevel)
    {
        print("Debuffs empty load");
    }

    // private void LoadSlots()
    // {
    //     GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

    //     for (int i = 0; i < save.StrongDebuffsIndexes.Count; i++)
    //     {
    //         _strongDebuffsSlots[i].SetData(new PassiveModuleOperand(ModuleCore.SpawnPassiveModule(PassiveModuleType.StrongDebuff, save.StrongDebuffsIndexes[i]), save.StrongDebuffsIndexes[i]));
    //     }

    //     Dictionary<int, int> keyCodes = new Dictionary<int, int>();
    //     List<int> codes = new List<int>(GameSessionInfoHandler.GetSessionSave().DebuffsIndexes);
    //     List<int> unique = new List<int>();

    //     for (int i = 0; i < codes.Count; i++)
    //     {
    //         if (unique.Contains(codes[i]))
    //             continue;

    //         unique.Add(codes[i]);
    //     }

    //     for (int i = 0; i < save.DebuffsIndexes.Count; i++)
    //     {
    //         if (keyCodes.ContainsKey(save.DebuffsIndexes[i]))
    //             keyCodes[save.DebuffsIndexes[i]] += 1;

    //         else
    //             keyCodes[save.DebuffsIndexes[i]] = 1;
    //     }

    //     for (int i = 0; i < unique.Count; i++)
    //     {
    //         _debuffsSlots[i].SetData(new PassiveModuleOperand(ModuleCore.SpawnPassiveModule(PassiveModuleType.Debuff, unique[i]), unique[i]));
    //         _debuffsSlots[i].SetCount(keyCodes[unique[i]]);

    //         for (int j = 0; j < keyCodes[unique[i]] - 1; j++)
    //         {
    //             ModuleCore.SpawnPassiveModule(PassiveModuleType.Debuff, unique[i]);
    //         }
    //     }
    // }

    // private void DeclareStrongDebuffs()
    // {
    //     int count = ShipStats.GetIntValue("StrongDebuffs");

    //     List<int> codes = new List<int>();

    //     while (codes.Count < count)
    //     {
    //         int val = Random.Range(0, _characterModules.StrongDebuffsCount);
    //         if (codes.Contains(val))
    //             continue;

    //         codes.Add(val);
    //     }

    //     GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
    //     save.StrongDebuffsIndexes = codes;
    //     GameSessionInfoHandler.RewriteSessionSave(save);
    // }

    // private void AddDebuff()
    // {
    //     List<int> codes = new List<int>(GameSessionInfoHandler.GetSessionSave().DebuffsIndexes);

    //     List<int> unique = new List<int>();

    //     for (int i = 0; i < codes.Count; i++)
    //     {
    //         if (unique.Contains(codes[i]))
    //             continue;

    //         unique.Add(codes[i]);
    //     }

    //     if (unique.Count >= maxUniqueDebuffs)
    //         codes.Add(unique[Random.Range(0, unique.Count)]);

    //     else
    //         codes.Add(Random.Range(0, _characterModules.DebuffPerksCount));

    //     GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
    //     save.DebuffsIndexes = codes;
    //     GameSessionInfoHandler.RewriteSessionSave(save);
    // }


}
