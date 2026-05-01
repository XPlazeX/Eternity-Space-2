// using UnityEngine;
// using UnityEngine.UI;

// public class FreshnessHandler : MonoBehaviour
// {
//     const int maxFreshness = 6;

//     [SerializeField] private Image _freshnessBar;
//     [SerializeField] private Module[] _freshnessBoosters;
//     [SerializeField] private Module _freshnessOverstatBooster;

//     private int _totalLevel = 0;

//     public void Enforce()
//     {
//         UpdateFreshnessLabel();

//         for (int i = 0; i < _totalLevel; i++)
//         {
//             if (i > _freshnessBoosters.Length - 1)
//             {
//                 ModuleCore.SpawnModule(_freshnessOverstatBooster);
//             }
//             else
//             {
//                 ModuleCore.SpawnModule(_freshnessBoosters[i]);
//             }
//         }
//         print($"load freshness : {_totalLevel}");
//     }

//     public void UpdateFreshnessLabel()
//     {
//         int totalLevel = 0;

//         if (InventoryData.HandingAttackPattern != null)
//             totalLevel += InventoryData.HandingAttackPattern.GearLevel;
//         if (InventoryData.HandingDevice != null)
//             totalLevel += InventoryData.HandingDevice.GearLevel;
//         if (InventoryData.HandingUltratech != null)
//             totalLevel += InventoryData.HandingUltratech.GearLevel;

//         _totalLevel = totalLevel;
        
//         print($"update Freshness Label : {Mathf.Clamp01((float)_totalLevel / maxFreshness)}");
//         _freshnessBar.fillAmount = Mathf.Clamp01((float)_totalLevel / maxFreshness);
//     }
// }
