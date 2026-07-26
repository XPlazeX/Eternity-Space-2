using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UltratechUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Button _ultratechButton;
    [SerializeField] private Image _fillingImage;
    [SerializeField] private Text _leftLabel;

    public void SetUTIcon(Sprite sprite)
    {
        _icon.sprite = sprite;
    }

    public void FillImage(float part)
    {
        _fillingImage.fillAmount = part;
    }

    public void ToggleInteractability(bool tog)
    {
        _ultratechButton.interactable = tog;
    }

    public void UpdateTextInfo(int leftRams = 0)
    {
        if (leftRams < 0)
            _leftLabel.text = "ACTIVE";

        else if (leftRams == 0)
            _leftLabel.text = "READY";

        else
            _leftLabel.text = "left : " + leftRams.ToString();
    }

    public void Disable()
    {
        _ultratechButton.interactable = false;
        _fillingImage.fillAmount = 0f;
        _leftLabel.text = "SAFETY MODE";
    }
}
