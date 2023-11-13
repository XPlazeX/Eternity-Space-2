// using UnityEngine;
// using UnityEngine.UI;
// using ModuleWork;
// using System.Collections;
// using System.Collections.Generic;

// public class ModuleChoice : MonoBehaviour
// {
//     [SerializeField] private RectTransform _moduleSlotExample;
//     [SerializeField] private Transform _mark;
//     [SerializeField] private float _maxWidth = 600f;
//     [SerializeField] private bool _perkChoice = false;
//     [SerializeField] private int _defaultChoicePower = 3;
//     [Header("Animation choice")]
//     [SerializeField] private float _animationTime = 1f;
//     [SerializeField] private float _yOffsetStep;

//     private List<ModuleSlotData> _animSlots = new List<ModuleSlotData>();

//     public int ChoicePower {get; set;}

//     private static int myID = 0;
//     private int _uniqID = -1;
//     private bool _destroyAfterChoice = false;

//     private void Start() {
//         GameSessionInfoHandler.LevelDischarge += CleanData;
//         ChoicePower = Mathf.Clamp(_defaultChoicePower + ShipStats.GetIntValue("ChoiceBoost"), 1, 8);
//         print($"module choice power : {ChoicePower}");
        
//         List<int> choiceData = GameSessionInfoHandler.GetDataCollection("ModuleChoices");

//         myID ++;
//         print($"my id is {myID - 1} at collection {choiceData}");
//         if (!choiceData.Contains(myID-1))
//         {
//             _uniqID = myID;
//             print("not contains");
//         }
//         else
//         {
//             _destroyAfterChoice = true;
//             print("contains");
//         }
//         GetChoice();
//     }

//     private void OnDestroy() {
//         GameSessionInfoHandler.LevelDischarge -= CleanData;
//     }

//     public void GetChoice()
//     {
//         // float stepPosition = _maxWidth / (ChoicePower - 1);
//         // float curPos = -(_maxWidth / 2f);

//         // CharacterModules characterModules = SceneStatics.CharacterCore.GetComponent<CharacterModules>();

//         // for (int i = 0; i < ChoicePower; i++)
//         // {
//         //     RectTransform moduleRect = Instantiate(_moduleSlotExample);

//         //     moduleRect.transform.SetParent(_mark.transform);
//         //     moduleRect.transform.localScale = Vector3.one;

//         //     moduleRect.anchoredPosition = new Vector2(curPos, 0f);

//         //     curPos += stepPosition;

//         //     moduleRect.GetComponent<ModuleChoiceSender>().Initialize(this);

//         //     PassiveModuleOperand pmOperand = null;

//         //     if (_perkChoice)
//         //         pmOperand = characterModules.GetRandomPassiveModule(PassiveModuleType.Perk);

//         //     else
//         //         pmOperand = characterModules.GetRandomPassiveModule(PassiveModuleType.Perk);

//         //     print("Здесь нужно исправить Choice");

//         //     moduleRect.GetComponent<ModuleSlotData>().SetData(pmOperand);
//         //     moduleRect.GetComponent<ModuleSlotData>().SetTypeIcon();

//         //     _animSlots.Add(moduleRect.GetComponent<ModuleSlotData>());
//         // }

//         // if (_destroyAfterChoice)
//         //     gameObject.SetActive(false);
//     }

//     public void SelectModule(ModuleSlotData slotData)
//     {
//         GetComponent<CanvasGroup>().interactable = false;

//         SceneStatics.UICore.GetComponent<InventoryGearLoader>().AddModule(slotData.GetBindedModule(), slotData.GetBindedModuleID());

//         for (int i = 0; i < _animSlots.Count; i++)
//         {
//             _animSlots[i].SetNormalIcon();
//         }

//         InventoryData.SaveModule(slotData.GetBindedModuleID(), (slotData.GetBindedModule().Type == PassiveModuleType.Perk));
//         GameSessionInfoHandler.AddValueToCollection("ModuleChoices", _uniqID - 1);

//         StartCoroutine(ChoiceAnimation(slotData.GetComponent<RectTransform>()));
//         //gameObject.SetActive(false);
//     }

//     private IEnumerator ChoiceAnimation(RectTransform targetRect)
//     {
//         float timer = _animationTime;

//         CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

//         while (timer > 0)
//         {
//             targetRect.anchoredPosition = new Vector2(targetRect.anchoredPosition.x, targetRect.anchoredPosition.y + (_yOffsetStep * Time.unscaledDeltaTime));
            
//             canvasGroup.alpha = timer / _animationTime;

//             timer -= Time.unscaledDeltaTime;

//             yield return null;
//         }

//         gameObject.SetActive(false);
//     }

//     private void CleanData()
//     {
//         myID = 0;
//         GameSessionInfoHandler.AddDataCollection("ModuleChoices", new List<int>());
//     }
// }
