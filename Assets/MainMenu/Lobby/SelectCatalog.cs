using System;
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
        Universal = 5,
        Unique = 6
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
        [SerializeField] private AbilityCompat[] _abilities;

        public ShipClass Type => _class;
        public AbilityCompat[] Abilities => _abilities;
    }

    [System.Serializable]
    public class CoreSelector : ObjectSelector
    {
        [SerializeField] private UnlockRequire _unlockRequirement;

        public UnlockRequire UnlockRequire => _unlockRequirement;
    }

    [System.Serializable]
    public struct AbilityCompat
    {
        public UnlockRequire unlockRequirement;
        public int localizationRow;
        public Sprite icon;
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
    [SerializeField] private CoreSelector[] _coreSelectors;
    [Header("Defaults")]
    [SerializeField] private AbilitySelectHelper _abilitySelectHelper;
    [SerializeField] private CoreSelectHelper _coreSelectHelper;
    [SerializeField] private CoreList _coreList;
    [SerializeField] private int _defaultWeaponID = 0;
    [Header("Additive")]
    [SerializeField] private RectTransform _placeFrame;
    [SerializeField] private Color[] _classColors;
    [SerializeField] private Image[] _targetShipIcons;
    [SerializeField] private Text[] _abilityIndicators;
    [Space()]
    [SerializeField] private Image[] _targetWeaponIcons;
    [SerializeField] private Image[] _targetCoreIcons;
    [SerializeField] private Text _nameLabel;
    [SerializeField] private Text _weaponLetalityLabel;
    [SerializeField] private Text _weaponDescription;
    [SerializeField] private GameObject[] _panels;

    public static ShipClass ActiveClass {get; private set;} = ShipClass.Scout;
    public static int ActiveShipID {get; private set;} = 0;
    private int _shipID = -1;
    private int _weaponID = 0;
    private int _coreID = 0;
    private int _abilityID = 0;
    private int _helpAbilityID = 0;

    private void Start() 
    {
        SetShip(GlobalSaveHandler.GetSave().LastSelectedShip);
        SetAbilityVariation(GlobalSaveHandler.GetSave().LastSelectedAbility);

        int lastWeapon = GlobalSaveHandler.GetSave().LastSelectedWeapon;
        if (lastWeapon > _weaponsSelectors.Length)
            lastWeapon = _defaultWeaponID;
        
        SetWeapon(lastWeapon, false);

        SetCore(GlobalSaveHandler.GetSave().LastSelectedCore, false);
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
                SetWeapon(_weaponID, true);
                break;
            case 2:
                SetCore(_coreID, true);
                break;
            default:
                break;
        }
    }

    public void SetShip(int id)
    {
        id = Mathf.Clamp(id, 0, _shipSelectors.Length);
        _placeFrame.anchoredPosition = _shipSelectors[id].Rect.anchoredPosition;
        _nameLabel.text = SceneLocalizator.GetLocalizedString("Ships", id, 0);//new TextLoader("Ships", _shipID, 0).FirstCell;

        if (id == _shipID)
            return;

        _shipID = id;
        SetClass(_shipSelectors[id].Type);
        ActiveShipID = int.Parse(_shipSelectors[_shipID].SettingValue);

        for (int i = 0; i < _targetShipIcons.Length; i++)
        {
            _targetShipIcons[i].sprite = _shipSelectors[id].Icon;
        }
        //print(_shipID);

        SetAbilityVariation(0);
    }

    public void SelectNextAbility()
    {
        int nextId = _helpAbilityID + 1;
        if (nextId >= _shipSelectors[_shipID].Abilities.Length)
            nextId = 0;
        
        SetAbilityVariation(nextId);
    }

    public void SetAbilityVariation(int id)
    {
        //print($"Set ability: {id}, shipID: {_shipID}");
        bool unlocked = Unlocks.HasUnlock(_shipSelectors[_shipID].Abilities[id].unlockRequirement);

        _abilitySelectHelper.SetContainment(_shipSelectors[_shipID].Abilities[id].icon, _classColors[(int)_shipSelectors[_shipID].Type], _shipSelectors[_shipID].Abilities[id].localizationRow, unlocked);
        _helpAbilityID = id;

        if (unlocked)
        {
            _abilityID = id;
            for (int i = 0; i < _abilityIndicators.Length; i++)
            {
                _abilityIndicators[i].color = (i == id) ? _classColors[(int)_shipSelectors[_shipID].Type] : Color.clear;
            }
        }
    }

    public void SetWeapon(int id, bool setFrame)
    {
        if (id < 0 || id >= _weaponsSelectors.Length)
        {
            id = _defaultWeaponID;
        }

        _weaponID = id;

        if (setFrame)
        {
            _placeFrame.anchoredPosition = _weaponsSelectors[id].Rect.anchoredPosition;
            _nameLabel.text = SceneLocalizator.GetLocalizedString("Weapons", id, 0);
        }
        
        for (int i = 0; i < _targetWeaponIcons.Length; i++)
        {
            _targetWeaponIcons[i].sprite = _weaponsSelectors[id].Icon;
        }
        _weaponLetalityLabel.text = SceneLocalizator.GetLocalizedString("Weapons", id, 1);
        _weaponDescription.text = SceneLocalizator.GetLocalizedString("Weapons", id, 2);
    }

    public void ButtonSetWeapon(int id)
    {
        SetWeapon(id, true);
    }

    public void SetCore(int id, bool setFrame)
    {
        bool unlocked = Unlocks.HasUnlock(_coreSelectors[id].UnlockRequire);
        if (setFrame)
        {
            _placeFrame.anchoredPosition = _coreSelectors[id].Rect.anchoredPosition;
            _nameLabel.text = SceneLocalizator.GetLocalizedString("Cores", id, 0);//new TextLoader("Cores", id, 0).FirstCell;
        }

        if (!unlocked)
        {
            _coreSelectHelper.SetContainment(id, false);
            return;
        }

        if (id < 0 || id >= _coreSelectors.Length)
        {
            id = 0;
        }

        _coreID = id;

        for (int i = 0; i < _targetCoreIcons.Length; i++)
        {
            _targetCoreIcons[i].sprite = _coreSelectors[id].Icon;
        }

        _coreSelectHelper.SetContainment(id, true);
    }

    public void ButtonSetCore(int id)
    {
        if (NowUniqueShip())
            return;
        SetCore(id, true);
    }

    public void SetClass(ShipClass type)
    {
        if (ActiveClass == ShipClass.Unique && type != ShipClass.Unique)
        {
            SetCore(0, false);
        }

        ActiveClass = type;
        print(ActiveClass);

        if (ActiveClass == ShipClass.Unique)
        {
            SetUniqueShip(_shipID);
            return;
        }

        if (!_weaponsSelectors[_weaponID].Compat(ActiveClass))
        {
            SetWeapon(_defaultWeaponID, false);
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

    public void HideCompatibility()
    {
        for (int i = 0; i < _weaponsSelectors.Length; i++)
        {
            _weaponsSelectors[i].Hide();
        }
    }

    private void OnDisable() {
        GlobalSave gsave = GlobalSaveHandler.GetSave();
        gsave.LastSelectedShip = _shipID;
        gsave.LastSelectedWeapon = _weaponID;
        gsave.LastSelectedAbility = _abilityID;
        gsave.LastSelectedCore = _coreID;
        GlobalSaveHandler.RewriteSave(gsave);

        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
        save.WeaponModel = _weaponsSelectors[_weaponID].SettingValue;
        save.ShipModel = int.Parse(_shipSelectors[_shipID].SettingValue);
        save.AbilityID = _abilityID;

        int coreModel = int.Parse(_coreSelectors[_coreID].SettingValue);
        save.CoreModel = coreModel;
        save.MaxMegawatts = _coreList.GetCore(coreModel).MaxMegawatts;
        save.Megawatts = _coreList.GetCore(coreModel).StartMegawatts;

        GameSessionInfoHandler.RewriteSessionSave(save);
    }

    private void SetUniqueShip(int id)
    {
        if (id != 12)
            return;

        SetWeapon(13, false);
        SetCore(6, false);
        HideCompatibility();
    }

    private bool NowUniqueShip() => _shipID == 12;
}
