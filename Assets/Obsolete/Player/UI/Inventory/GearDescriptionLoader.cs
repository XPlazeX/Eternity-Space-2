// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using ModuleWork;

// public class GearDescriptionLoader : MonoBehaviour
// {
//     public static KeyValuePair<string, string> GetGearTextDescription(GearType gearType, int id)
//     {
//         switch ((int)gearType)
//         {
//             case 1:
//                 return new KeyValuePair<string, string> (SceneLocalizator.GetLocalizedString("Weapons", id, 0), SceneLocalizator.GetLocalizedString("Weapons", id, 1));

//             case 2:
//                 return new KeyValuePair<string, string> (SceneLocalizator.GetLocalizedString("Devices", id, 0), SceneLocalizator.GetLocalizedString("Devices", id, 1));

//             case 3:
//                 return new KeyValuePair<string, string> (SceneLocalizator.GetLocalizedString("Ultrateches", id, 0), SceneLocalizator.GetLocalizedString("Ultrateches", id, 1));
            
//             default:
//                 throw new System.Exception("Не действительный GearType. Проверьте перечисления в ModuleWork в скрипте Module");
//         }
//     }

//     public static KeyValuePair<string, string> GetPassiveModuleTextDescription(PassiveModuleType moduleType, int id)
//     {
//         switch (moduleType)
//         {
//             case PassiveModuleType.Perk:
//                 return new KeyValuePair<string, string> (SceneLocalizator.GetLocalizedString("PassiveModules", id, 0), SceneLocalizator.GetLocalizedString("PassiveModules", id, 1));
//             case PassiveModuleType.Debuff:
//                 return new KeyValuePair<string, string> (SceneLocalizator.GetLocalizedString("Debuffs", id, 0), SceneLocalizator.GetLocalizedString("Debuffs", id, 1));
//             default:
//                 return new KeyValuePair<string, string> (SceneLocalizator.GetLocalizedString("PassiveModules", id, 0), SceneLocalizator.GetLocalizedString("PassiveModules", id, 1));
//         }
//     }

//     // public static string GetGearVisibleStats(GearType gearType, int number)
//     // {
//     //     return SceneLocalizator.GetLocalizedString("GearStats", (int)gearType, number);
//     // }

//     public static string GetFreshnessName(int number)
//     {
//         return SceneLocalizator.GetLocalizedString("Freshness", number, 0);
//     }
// }
