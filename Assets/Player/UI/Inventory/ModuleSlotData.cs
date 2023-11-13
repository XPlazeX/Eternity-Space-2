// using UnityEngine;
// using UnityEngine.UI;
// using ModuleWork;

// public class ModuleSlotData : MonoBehaviour
// {
//     [SerializeField] private Image _icon;

//     private PassiveModuleOperand _pmOperand;

//     public void SetData(PassiveModuleOperand pmOperand)
//     {
//         _pmOperand = pmOperand;

//         SetNormalIcon();
//         _icon.gameObject.SetActive(true);
//     }

//     public void SetNormalIcon()
//     {
//         _icon.sprite = _pmOperand.HandingModule.Icon;
//     }

//     public void SetTypeIcon()
//     {
//         _icon.sprite = SceneStatics.UICore.GetComponent<ModuleTypesOrganizer>().GetSprite(_pmOperand.Type);
//     }

//     public void SelectModuleSlot()
//     {
//         if (_pmOperand != null)
//         {
//             InventoryGearLoader inventory = SceneStatics.UICore.GetComponent<InventoryGearLoader>();
//             inventory.SetModuleDescription(_pmOperand, GetComponent<RectTransform>());
//         }
//     }

//     public PassiveModule GetBindedModule() => _pmOperand.HandingModule;
//     public int GetBindedModuleID() => _pmOperand.ModuleID;
// }
