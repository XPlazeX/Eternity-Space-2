﻿using UnityEngine;
using UnityEngine.UI;
using ModuleWork;

public class ModuleService : MonoBehaviour
{
    [SerializeField] private Button _btn;
    [SerializeField] private Image _icon;
    [SerializeField] private Text _name;
    [SerializeField] private Text _description;
    [SerializeField] private BuyButton _buyBtn;
    [SerializeField] private float _heightStep;
    [Space()]
    [SerializeField][Range(0, 11)] private int _contagionLevel;
    [SerializeField][Range(0, 1f)] private float _sale = 0f;
    [Space()]
    [SerializeField] private bool _useConstantLevelEvent = false;
    [SerializeField] private LevelEvent _constantLevelEvent;

    public float Height => _heightStep;

    private LevelEvent _activeEvent;
    private int _choiceID;
    
    public void Load(LevelEvent levelEvent, int choiceID)
    {
        if (_useConstantLevelEvent)
        {
            levelEvent = _constantLevelEvent;
        }

        _activeEvent = levelEvent;
        _choiceID = choiceID;

        ModuleOperand mo = levelEvent.handingModuleOperand;
        _name.text = SceneLocalizator.GetLocalizedString("PassiveModules", mo.LocalizationID, 0);
        _description.text = SceneLocalizator.GetLocalizedString("PassiveModules", mo.LocalizationID, 1);
        _icon.sprite = mo.Icon;

        _buyBtn.ScalePrice(Mathf.RoundToInt(levelEvent.auritePrice * (1f - _sale)));

        CheckAsquisition();
    }

    public void Asquire()
    {
        ModulasSave modulasSave = ModulasSaveHandler.GetSave();

        modulasSave.AddEvent(_activeEvent);
        modulasSave.ActiveEventDatas[_choiceID].purchased = true;

        ModulasSaveHandler.RewriteSave(modulasSave);

        _activeEvent.handingModule.Asquiring();

        if (_contagionLevel > 0)
        {
            ContagionHandler.AddContagion(_contagionLevel);
        }

        CheckAsquisition();
    }

    public void CheckAsquisition()
    {
        if (ModulasSaveHandler.GetSave().ActiveEventDatas[_choiceID].purchased)
        {
            _buyBtn.SetPriceText(SceneLocalizator.GetLocalizedString("MissionMenu", 0, 0));
            _btn.interactable = false;

            if (_activeEvent.stackType != ModuleStackType.Pack)
                return;

            PackModuleOperand mo = _activeEvent.handingModuleOperand as PackModuleOperand;
            SceneStatics.SceneCore.GetComponent<MenuPackRenderer>().RenderPack(mo.PackID);

        } else {
            _btn.interactable = true;
        }
    }
}
