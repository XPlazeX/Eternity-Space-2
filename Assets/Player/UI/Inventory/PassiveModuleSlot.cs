// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using ModuleWork;

// public class PassiveModuleSlot : MonoBehaviour
// {
//     [SerializeField] private Image _icon;
//     [SerializeField] private Text _name;
//     [SerializeField] private Text _description;
//     [SerializeField] private Image _affectedByColorImage;

//     public void SetData(PassiveModuleOperand pmOperand)
//     {
//         if (_icon != null)
//         {
//             _icon.sprite = pmOperand.HandingModule.Icon;
//             _icon.gameObject.SetActive(true);
//         }
//         Color typeColor = SceneStatics.UICore.GetComponent<ModuleTypesOrganizer>().GetColor(pmOperand.Type);

//         _name.color = new Color(typeColor.r, typeColor.g, typeColor.b, 1f);
//         _description.color = new Color(typeColor.r, typeColor.g, typeColor.b, _description.color.a);
        
//         if (_affectedByColorImage != null)
//             _affectedByColorImage.color = new Color(typeColor.r, typeColor.g, typeColor.b, _affectedByColorImage.color.a);

//         KeyValuePair<string, string> dataset = GearDescriptionLoader.GetPassiveModuleTextDescription(pmOperand.HandingModule.Type, pmOperand.ModuleID);

//         _name.text = dataset.Key;
//         _description.text = dataset.Value;

//         // if (_affectedByColorImage != null)
//         //     _affectedByColorImage.color = new Color(rarityColor.r, rarityColor.g, rarityColor.b, _affectedByColorImage.color.a);
//     }

//     public void SetCount(int count)
//     {
//         if (count == 1)
//             return;

//         _name.text = _name.text + " x" + count.ToString();
//     }
// }
