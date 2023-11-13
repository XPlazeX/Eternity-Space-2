// using System.Collections;
// using UnityEngine;
// using UnityEngine.UI;
// using ModuleWork;

// public class GearBarterHandler : MonoBehaviour
// {
//     [SerializeField] private bool _browsevable = true;
//     [SerializeField] private bool _isSelling = false;
//     [SerializeField] private Module4Barter[] _gearSlots;
//     [SerializeField] private Module4Barter _transmittedSlot;
//     [Header("Gear choice settings")]
//     [SerializeField] private float _additiveLuckMultiplier = 1f;
//     [SerializeField] private int _defaultGearChoicePower = 3;
//     [SerializeField][Range(0, 4)] private int _gearLevel;
//     [Header("Sell settings")]
//     [SerializeField] private Text _sellLabel;
//     [SerializeField] private int[] PriceList = {0,1,2,3,4};

//     private Module4Barter _selectedSlot;
//     private GearOperand _oldModuleBuffer;
//     private InventoryGearLoader _inventoryGearLoader;
//     private GearHighlighter _gearHighlighter;

//     private int _offerPrice;
//     private float _priceMultiplier;
//     private bool _offer = false;

//     private int ChoicePower {get; set;}

//     private void Start() 
//     {
//         _priceMultiplier = ShipStats.GetValue("PriceMultiplier");

//         ChoicePower = Mathf.Clamp(_defaultGearChoicePower + ShipStats.GetIntValue("ChoiceBoost"), 1, _gearSlots.Length);
//         print($"Current level : {GameSessionInfoHandler.GetSessionSave().CurrentLevel}");
//         print($"choice power : {ChoicePower}");

//         _inventoryGearLoader = SceneStatics.UICore.GetComponent<InventoryGearLoader>();
//         _gearHighlighter = SceneStatics.UICore.GetComponent<GearHighlighter>();

//         _gearHighlighter.Deselect();
//         _inventoryGearLoader.Obsolescence();

//         GameSessionInfoHandler.LevelDischarge += CashOutOffer;

//         GetChoice();
//         //Initialize();
//     }

//     private void OnDestroy() {
//         GameSessionInfoHandler.LevelDischarge -= CashOutOffer;
//     }

//     private void GetChoice()
//     {
//         // CharacterModules characterModules = SceneStatics.CharacterCore.GetComponent<CharacterModules>();

//         // Random.InitState(GameSessionInfoHandler.GetSeed());

//         // for (int i = 0; i < _gearSlots.Length; i++)
//         // {
//         //     if (i < ChoicePower)
//         //     {
//         //         GearOperand operand = characterModules.GetRandomGear(_gearLevel, _additiveLuckMultiplier);

//         //         _gearSlots[i].SetOperand(operand);
//         //         _gearSlots[i].SetPrice(Mathf.CeilToInt(_priceMultiplier * PriceList[(int)operand.MRarity]));
//         //     }
//         //     else
//         //         _gearSlots[i].ClearOperand();
//         // }
//     }

//     public void TryOffer(Module4Barter slot)
//     {
//         if (_offer)
//         {
//             return;
//         }
//         if (_isSelling && PlayerShipData.HitPoints < slot.Price)
//         {
//             return;
//         }

//         _selectedSlot = slot;

//         GearOperand replacedOperand = null;

//         switch ((int)slot.HandingGearOperand.Type)
//         {
//             case 1:
//                 replacedOperand = InventoryData.HandingAttackPattern;
//                 break;
//             case 2:
//                 replacedOperand = InventoryData.HandingDevice;
//                 break;
//             case 3:
//                 replacedOperand = InventoryData.HandingUltratech;
//                 break;
//             default:
//                 throw new System.Exception("Неверный тип модуля.");
//         }
//         _inventoryGearLoader.SetGear(slot.HandingGearOperand);
//         // _gearHighlighter.Select((int)slot.ModuleOperand.Type - 1);

//         _oldModuleBuffer = _selectedSlot.HandingGearOperand;
//         _selectedSlot.ClearOperand();
//         _transmittedSlot.SetOperand(replacedOperand);

//         _offer = true;

//         if (_isSelling)
//         {
//             SetSellPrice(slot);
//         }
//         else
//         {
//             SetSellPrice(replacedOperand);
//         }
//     }

//     public void CancelOffer()
//     {
//         if (!_offer || !_browsevable)
//         {
//             return;
//         }

//         _selectedSlot.SetOperand(_oldModuleBuffer);
        
//         _inventoryGearLoader.SetGear(_transmittedSlot.HandingGearOperand);

//         _transmittedSlot.ClearOperand();

//         _offer = false;
//         _offerPrice = 0;
//         SetLabelPrice(0);
//     }

//     private void SetSellPrice(Module4Barter slot)
//     {
//         int startPrice = Mathf.CeilToInt(slot.Price);

//         SetLabelPrice(startPrice);
//         _offerPrice = startPrice;
//     }

//     private void SetSellPrice(GearOperand operand)
//     {
//         int startPrice = Mathf.CeilToInt(PriceList[(int)operand.MRarity] * _priceMultiplier);

//         SetLabelPrice(startPrice);
//         _offerPrice = startPrice;
//     }

//     private void SetLabelPrice(int price)
//     {
//         string[] text = _sellLabel.text.Split('№');
//         _sellLabel.text = text[0] + "№ " + price.ToString() + "</color>";
//     }

//     private void CashOutOffer()
//     {
//         if (_offerPrice == 0)
//             return;
            
//         if (_isSelling)
//         {
//             PlayerShipData.RegenerateHP(-_offerPrice);
//         }
//         else
//         {
//             PlayerShipData.RegenerateHP(_offerPrice);
//         }
//         print("regenerating?");
//     }

// }
