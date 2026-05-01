using UnityEngine;
using UnityEngine.UI;

public class AbilitySelectHelper : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Text _head;
    [SerializeField] private Text _description;
    [SerializeField] private Sprite _lockedSprite;
    [SerializeField] private Color _lockedColor;

    public void SetContainment(Sprite icon, Color classColor, int localizationRow, bool unlocked)
    {
        _icon.sprite = unlocked ? icon : _lockedSprite;

        _head.text = SceneLocalizator.GetLocalizedString("Abilities", localizationRow, unlocked ? 0 : 2);
        _description.text = SceneLocalizator.GetLocalizedString("Abilities", localizationRow, unlocked ? 1 : 3);

        _head.color = unlocked ? classColor : _lockedColor;
        _description.color = unlocked ? Color.white : _lockedColor;
    }
}
