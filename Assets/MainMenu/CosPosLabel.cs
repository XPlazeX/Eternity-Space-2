using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CosPosLabel : MonoBehaviour
{
    [SerializeField] private bool _isPositronium = false;
    private Text _label;

    void Start()
    {
        _label = GetComponent<Text>();
        
        CheckValue();
        Bank.BankUpdated += CheckValue;
        //PlayerShipData.ChangeHealth += ChangeValue;
    }

    private void OnDisable() {
        Bank.BankUpdated -= CheckValue;
    }


    private void CheckValue()
    {
        if (_label == null)
            return;

        if (_isPositronium)
            _label.text = GlobalSaveHandler.GetSave().Positronium.ToString();
        else
            _label.text = GlobalSaveHandler.GetSave().Cosmilite.ToString();

    }
}
