// using UnityEngine;
// using ModuleWork;
// using System.Collections.Generic;

// public class InventoryData : MonoBehaviour
// {
//     public static GearOperand HandingAttackPattern {get; private set;}
//     public static GearOperand HandingDevice {get; private set;}
//     public static GearOperand HandingUltratech {get; private set;}

//     private void Start() {
//         GameSessionInfoHandler.LevelDischarge += SaveGear;
//         SceneStatics.UICore.GetComponent<FreshnessHandler>().Enforce();
//     }

//     private void OnDisable() {
//         GameSessionInfoHandler.LevelDischarge -= SaveGear;
//     }

//     public static void SaveModule(int id, bool isPerk = false)
//     {
//         GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

//         if (isPerk)
//         {
//             save.PerkIndexes.Add(id);
//         }
//         else
//         {
//             save.SmallPassiveModuleIndexes.Add(id);
//         }
//         GameSessionInfoHandler.RewriteSessionSave(save);
//     }

//     public static void SetGearOperand(GearOperand operand)
//     {
//         switch ((int)operand.Type)
//         {
//             case 1:
//                 HandingAttackPattern = operand;
//                 break;
//             case 2:
//                 HandingDevice = operand;
//                 break;
//             case 3:
//                 HandingUltratech = operand;
//                 break;

//             default:
//                 throw new System.Exception("Неверный тип модуля.");
//         }

//         SceneStatics.UICore.GetComponent<FreshnessHandler>().UpdateFreshnessLabel();
//         // здесь нужно сохранять данные
//     }

//     public static void SaveGear()
//     {
//         GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

//         // save.WeaponModel = new KeyValuePair<int, int>(HandingAttackPattern.GearID, HandingAttackPattern.GearLevel);
//         // save.DeviceModel = new KeyValuePair<int, int>(HandingDevice.GearID, HandingDevice.GearLevel);
//         // save.UltratechModel = new KeyValuePair<int, int>(HandingUltratech.GearID, HandingUltratech.GearLevel);
//     }
// }
