// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using ModuleWork;

// public class GearSlot : MonoBehaviour
// {
//     [SerializeField] private Image[] _affectedByColorImages;
//     [SerializeField] private Text[] _affectedByColorTexts;
//     [SerializeField] private Text _gearRarityLabel;

//     [SerializeField] private Image _gearBlueprint;
//     [SerializeField] private Image _gearBlueprintGradient;
//     [SerializeField] private Text _gearName;
//     [SerializeField] private Text _gearDescription;
//     [SerializeField] private Text _gearFreshnessLabel;
//     //[SerializeField] private Text _damageBoostLabel;
//     // [Space()]
//     // [SerializeField] private Text _firstParameterLabel;
//     // [SerializeField] private Text _secondParameterLabel;

//     public GearOperand Operand {get; private set;}

//     public void SetGear(GearOperand gearOperand)
//     {
//         Operand = gearOperand;
//         _gearBlueprint.sprite = gearOperand.HandingGear.Icon;
        
//         KeyValuePair<string, string> dataset = GearDescriptionLoader.GetGearTextDescription(gearOperand.Type, gearOperand.GearID);

//         _gearName.text = dataset.Key;
//         _gearDescription.text = dataset.Value;

//         _gearFreshnessLabel.text = GearDescriptionLoader.GetFreshnessName(gearOperand.GearLevel);

//         // _gearLevelLabel.text = $"уровень {gearOperand.GearLevel + 1}";
//         // _damageBoostLabel.text = $"+{(gearOperand.GearLevel) * 10}% урона";

//         // _firstParameterLabel.text = $"<color=#FFFF55>{gearOperand.HandingGear.FirstStat}</color> {GearDescriptionLoader.GetGearVisibleStats(gearOperand.Type, 0)}";
//         // _secondParameterLabel.text = $"<color=#FFFF55>{gearOperand.HandingGear.SecondStat}</color> {GearDescriptionLoader.GetGearVisibleStats(gearOperand.Type, 1)}";

//         SetRarity(gearOperand.MRarity);
//     }

//     private void SetRarity(GearRarity rarity)
//     {
//         RarityPreset colorPreset = SceneStatics.UICore.GetComponent<InventoryGearLoader>().GetRarityPreset(rarity);

//         _gearRarityLabel.text = SceneLocalizator.GetLocalizedString("Rarity", (int)rarity, 0);

//         for (int i = 0; i < _affectedByColorImages.Length; i++)
//         {
//             SetColor(_affectedByColorImages[i], colorPreset.MainColor);
//         }

//         for (int i = 0; i < _affectedByColorTexts.Length; i++)
//         {
//             SetColor(_affectedByColorTexts[i], colorPreset.MainColor);
//         }

//         SetColor(_gearBlueprint, colorPreset.GearFirstColor);
//         SetColor(_gearBlueprintGradient, colorPreset.GearSecondColor);
//         // SetColor(_firstParameterLabel, colorPreset.GearFirstColor);
//         // SetColor(_secondParameterLabel, colorPreset.GearSecondColor);
//     }

//     private void SetColor(Image image, Color col)
//     {
//         image.color = new Color(col.r, col.g, col.b, image.color.a);
//     }

//     private void SetColor(Text text, Color col)
//     {
//         text.color = new Color(col.r, col.g, col.b, text.color.a);
//     }

// }
