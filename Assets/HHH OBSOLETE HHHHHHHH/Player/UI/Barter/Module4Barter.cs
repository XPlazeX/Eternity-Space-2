// using System.Collections;
// using UnityEngine;
// using UnityEngine.UI;
// using ModuleWork;

// public class Module4Barter : MonoBehaviour
// {
//     [SerializeField] private Image _gradient;
//     [SerializeField] private Sprite _defaultSprite;
//     [SerializeField] private Color _defaultColor;
//     [SerializeField] private Text _price;

//     public GearOperand HandingGearOperand {get; private set;}
//     public int Price {get; private set;}

//     private Image _blueprint;

//     private void Awake() {
//         _blueprint = GetComponent<Image>();
//     }

//     public void SetOperand(GearOperand operand)
//     {
//         HandingGearOperand = operand;
//         _blueprint.sprite = HandingGearOperand.HandingGear.Icon;

//         RarityPreset colorPreset = SceneStatics.UICore.GetComponent<InventoryGearLoader>().GetRarityPreset(operand.MRarity);
//         Color col = colorPreset.GearFirstColor;
//         Color secCol = colorPreset.GearSecondColor;

//         _blueprint.color = new Color(col.r, col.g, col.b, 1f);
//         _gradient.color = new Color(secCol.r, secCol.g, secCol.b, 1f);
//     }

//     public void ClearOperand()
//     {
//         HandingGearOperand = null;
//         _blueprint.sprite = _defaultSprite;

//         _blueprint.color = _defaultColor;
//         _gradient.color = Color.clear;
//     }

//     public void SetPrice(int value)
//     {
//         if (_price != null)
//             _price.text = $"{value} #";

//         Price = value;
//     }

//     public void TryOffer()
//     {
//         if (HandingGearOperand!= null)
//         GameObject.FindWithTag("Barter").GetComponent<GearBarterHandler>().TryOffer(this);
//     }

// }
