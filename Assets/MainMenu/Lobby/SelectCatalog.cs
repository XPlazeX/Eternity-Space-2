using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GearCompatibility;

namespace GearCompatibility
{
    public enum ShipClass
    {
        Scout = 0,
        Stormtrooper = 1,
        Fighter = 2,
        Bomber = 3,
        Scientific = 4,
        Universal = 5
    }

    public enum CompatType
    {
        Ship = 0,
        Weapon = 1,
        SuperWeapon = 2,
        Device = 3
    }

    [System.Serializable]
    public class ObjectSelector
    {
        [SerializeField] protected string _settingValue;
        [SerializeField] private Sprite _icon;
        [SerializeField] private RectTransform _rect;

        public string SettingValue => _settingValue;
        public Sprite Icon => _icon;
        public RectTransform Rect => _rect;

        public void Hide()
        {
            _rect.GetComponent<CanvasGroup>().blocksRaycasts = false;
            _rect.GetComponent<CanvasGroup>().alpha = 0.2f;
        }

        public void Show()
        {
            _rect.GetComponent<CanvasGroup>().blocksRaycasts = true;
            _rect.GetComponent<CanvasGroup>().alpha = 1f;
        }
    }

    [System.Serializable]
    public class ShipSelector : ObjectSelector
    {
        [SerializeField] private ShipClass _class;

        public ShipClass Type => _class;
    }

    [System.Serializable]
    public class GearSelector : ObjectSelector
    {
        [SerializeField] private ShipClass[] _classes;
        
        public bool Compat(ShipClass type) => (Array.Exists(_classes, match => match == type)) || type == ShipClass.Universal || (Array.Exists(_classes, match => match == ShipClass.Universal));
    }

    [System.Serializable]
    public class CompatObject
    {
        [SerializeField] private ShipClass _class;
        [SerializeField] private CanvasGroup[] _canvasGroupsElement;

        public ShipClass Type => _class;

        public void Show()
        {
            for (int i = 0; i < _canvasGroupsElement.Length; i++)
            {
                _canvasGroupsElement[i].blocksRaycasts = true;
                _canvasGroupsElement[i].alpha = 1f;
            }
        }

        public void Hide()
        {
            for (int i = 0; i < _canvasGroupsElement.Length; i++)
            {
                _canvasGroupsElement[i].blocksRaycasts = false;
                _canvasGroupsElement[i].alpha = 0.2f;
            }
        }
    }
}

public class SelectCatalog : MonoBehaviour
{
    //[SerializeField] private CompatObject[] _compatObjects;
    [Header("Selectors")]
    [SerializeField] private ShipSelector[] _shipSelectors;
    [SerializeField] private GearSelector[] _weaponsSelectors;
    [Header("Defaults")]
    [SerializeField] private int _defaultWeaponID = 0;
    [Header("Additive")]
    [SerializeField] private RectTransform _placeFrame;
    //[SerializeField] private RectTransform _weaponPlaceFrame;
    [SerializeField] private Image[] _targetShipIcons;
    [SerializeField] private Image[] _targetWeaponIcons;
    [SerializeField] private Text _nameLabel;
    [SerializeField] private GameObject[] _panels;

    public static ShipClass ActiveClass {get; private set;} = ShipClass.Scout;
    private int _shipID = 0;
    private int _weaponID = 0;

    private void Start() {
        SetShip(GlobalSaveHandler.GetSave().LastSelectedShip);
        SetWeapon(GlobalSaveHandler.GetSave().LastSelectedWeapon);
    }

    public void OpenPanel(int id)
    {
        for (int i = 0; i < _panels.Length; i++)
        {
            _panels[i].SetActive(false);
        }

        _panels[id].SetActive(true);
        switch (id)
        {
            case 0:
                SetShip(_shipID);
                break;
            case 1:
                SetWeapon(_weaponID);
                break;
            default:
                break;
        }
    }

    public void SetShip(int id)
    {
        SetClass(_shipSelectors[id].Type);
        _shipID = id;

        _placeFrame.anchoredPosition = _shipSelectors[id].Rect.anchoredPosition;
        for (int i = 0; i < _targetShipIcons.Length; i++)
        {
            _targetShipIcons[i].sprite = _shipSelectors[id].Icon;
        }
        //print(_shipID);
        _nameLabel.text = new TextLoader("Ships", _shipID, 0).FirstCell;
    }

    public void SetWeapon(int id)
    {
        _weaponID = id;
        _placeFrame.anchoredPosition = _weaponsSelectors[id].Rect.anchoredPosition;
        for (int i = 0; i < _targetWeaponIcons.Length; i++)
        {
            _targetWeaponIcons[i].sprite = _weaponsSelectors[id].Icon;
        }

        _nameLabel.text = new TextLoader("Weapons", _weaponID, 0).FirstCell;
    }

    public void SetClass(ShipClass type)
    {
        ActiveClass = type;
        print(ActiveClass);

        if (!_weaponsSelectors[_weaponID].Compat(ActiveClass))
        {
            SetWeapon(_defaultWeaponID);
        }

        ReloadCompatibility();
    }

    public void ReloadCompatibility()
    {
        for (int i = 0; i < _weaponsSelectors.Length; i++)
        {
            if (_weaponsSelectors[i].Compat(ActiveClass))
            {
                _weaponsSelectors[i].Show();
            } else {
                _weaponsSelectors[i].Hide();
            }
        }
    }

    private void OnDisable() {
        GlobalSave gsave = GlobalSaveHandler.GetSave();
        gsave.LastSelectedShip = _shipID;
        gsave.LastSelectedWeapon = _weaponID;
        GlobalSaveHandler.RewriteSave(gsave);

        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
        save.WeaponModel = _weaponsSelectors[_weaponID].SettingValue;
        save.ShipModel = int.Parse(_shipSelectors[_shipID].SettingValue);
        GameSessionInfoHandler.RewriteSessionSave(save);
    }
}
