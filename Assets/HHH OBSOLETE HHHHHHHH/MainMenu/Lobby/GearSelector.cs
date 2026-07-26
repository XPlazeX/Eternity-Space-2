// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using GearCompatibility;

// public class GearSelector : MonoBehaviour
// {
//     [SerializeField] private CompatType _type;
//     [SerializeField] private int _defaultID;
//     [SerializeField] private GearObject[] _gearObjects;
//     [SerializeField] private RectTransform _placeFrame;
//     [SerializeField] private Image _targetIcon;

//     private TextLoader _textLoader;

//     private void Start() {
//         switch (_type)
//         {
//             case (CompatType.Ship):
//                 _textLoader = new TextLoader("Localization", "Ships");
//                 break;
//             case (CompatType.Weapon):
//                 _textLoader = new TextLoader("Localization", "Weapons");
//                 break;

//             default:
//                 break;
//         }
//     }

//     public void SetGear(int id)
//     {

//         _placeFrame.anchoredPosition = _gearObjects[id].Rect.anchoredPosition;
//         _targetIcon.sprite = _gearObjects[id].Icon;
//     }

//     [System.Serializable]
//     private class GearObject
//     {
//         [SerializeField] private RectTransform _rect;
//         [SerializeField] private Sprite _icon;

//         public RectTransform Rect => _rect;
//         public Sprite Icon => _icon;
//     }
// }
