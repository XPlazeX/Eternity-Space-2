// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using ModuleWork;

// public class InventoryGearLoader : MonoBehaviour
// {
//     public const int moduleSlotsCap = 23;

//     [SerializeField] private GameObject _inventoryPanel;
//     [SerializeField] private GearSlot[] _gearSlots;
//     [SerializeField] private RarityPreset[] _rarityPresets;
//     [SerializeField] private PassiveModuleSlot _moduleDescriptionSlot;
//     //[SerializeField] private RectTransform _smallSelectorFrame;
//     [SerializeField] private RectTransform _selectorFrame;
//     [Header("Module slots arrays work")]
//     //[SerializeField] private Text[] _collectionCounters;
//     [SerializeField] private ModuleSlotData[] _moduleSlots;
//     //[SerializeField] private ModuleSlotData[] _debuffModuleSlots;
//     //[SerializeField] private Color[] _freshnessChemes;
    
//     private int _nextFreeModuleSlotID = 0;
    
//     //private int _nextFreeDebuffSlotID = 0;

//     public void SetGear(GearOperand gearOperand)
//     {
//         _gearSlots[(int)gearOperand.Type - 1].SetGear(gearOperand);
//         InventoryData.SetGearOperand(gearOperand);
//     }

//     public void Obsolescence()
//     {
//         for (int i = 0; i < _gearSlots.Length; i++)
//         {
//             GearOperand operand = _gearSlots[i].Operand;
//             operand.GearLevel -= 1;
//             if (operand.GearLevel < 0)
//                 operand.GearLevel = 0;

//             _gearSlots[i].SetGear(operand);
//             InventoryData.SetGearOperand(operand);
//         }
//     }

//     public RarityPreset GetRarityPreset(GearRarity rarity)
//     {
//         return _rarityPresets[(int)rarity];
//     }

//     public void SetModuleDescription(PassiveModuleOperand pmOperand, RectTransform sender)
//     {
//         _moduleDescriptionSlot.SetData(pmOperand);

//         _selectorFrame.anchoredPosition = sender.anchoredPosition;
//         _selectorFrame.gameObject.SetActive(true);
//         //_selectorFrame.gameObject.SetActive(false);
        
//     }

//     public void AddModule(PassiveModule module, int id)
//     {
//         if (module.Type == PassiveModuleType.SmallModule)
//             return;

//         _moduleSlots[_nextFreeModuleSlotID].SetData(new PassiveModuleOperand(module, id));
//         _moduleSlots[_nextFreeModuleSlotID].GetComponent<Button>().interactable = true;

//         _nextFreeModuleSlotID ++;
//     }

//     public void ToggleInventory(bool tog)
//     {
//         _inventoryPanel.SetActive(tog);
//         if (!PauseController.HandPause)
//             TimeHandler.Resume();
//     }
    
// }
