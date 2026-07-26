using UnityEngine;
using UnityEngine.UI;

public class LabBlueprint : MonoBehaviour
{
    private const string blueprints_localization_filename = "Blueprints";

    [SerializeField] private Image _blueprintCanvas;
    [SerializeField] private GameObject _blueprintMask;
    [SerializeField] private Text _head;
    [SerializeField] private Text _blueprintIDLabel;
    [SerializeField] private Text _cosPricaeLabel;
    [SerializeField] private Text _posPriceLabel;
    [SerializeField] private Text _description;
    [SerializeField] private Text _requirements;
    [Space()]
    [SerializeField] private GameObject _post;
    [SerializeField] private Button _unlockBtn;
    [SerializeField] private Text _unlockBtnText;

    public void Block()
    {
        _post.SetActive(false);
        _unlockBtn.interactable = false;
    }

    public void SetData(Blueprint blueprint)
    {
        _blueprintCanvas.sprite = blueprint.sprite;
        _cosPricaeLabel.text = blueprint.cosPrice.ToString();
        _posPriceLabel.text = blueprint.posPrice.ToString();
        _blueprintIDLabel.text = blueprint.handingID.ToString();

        _head.text = SceneLocalizator.GetLocalizedString(blueprints_localization_filename, blueprint.localizationID, 0);
        _description.text = SceneLocalizator.GetLocalizedString(blueprints_localization_filename, blueprint.localizationID, 1);

        if (Unlocks.HasUnlocks(blueprint.requirementsID) && !Unlocks.HasUnlock(blueprint.handingID))
        {
            _blueprintMask.SetActive(false);
            _post.SetActive(false);
            _unlockBtn.interactable = true;

            if (Bank.EnoughtCash(BankSystem.Currency.Cosmilite, blueprint.cosPrice) && Bank.EnoughtCash(BankSystem.Currency.Positronium, blueprint.posPrice))
                _unlockBtnText.text = SceneLocalizator.GetLocalizedString("Lab", 0, 0);
            else
                _unlockBtnText.text = SceneLocalizator.GetLocalizedString("Lab", 0, 2);

        } else if (!Unlocks.HasUnlocks(blueprint.requirementsID)) 
        {
            _blueprintMask.SetActive(true);
            _post.SetActive(false);
            _unlockBtn.interactable = false;
            _description.text = "???";
            _cosPricaeLabel.text = "???";
            _posPriceLabel.text = "???";
            //_unlockBtnText.text = SceneLocalizator.GetLocalizedString("Lab", 0, 1);
        }
        else 
        {
            _blueprintMask.SetActive(false);
            _post.SetActive(true);
            _unlockBtn.interactable = false;
        }

        if (blueprint.inDevelop) 
        {
            _requirements.text = SceneLocalizator.GetLocalizedString("Lab", 1, 0);
            _unlockBtn.interactable = false;
        } else {
            _requirements.text = SceneLocalizator.GetLocalizedString(blueprints_localization_filename, blueprint.localizationID, 2);
        }
    }
}
