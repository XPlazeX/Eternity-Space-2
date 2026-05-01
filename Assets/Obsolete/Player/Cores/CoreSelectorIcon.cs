using UnityEngine;
using UnityEngine.UI;

public class CoreSelectorIcon : MonoBehaviour
{
    [SerializeField] private Sprite _lockedSprite;
    [SerializeField] private UnlockRequire _unlockRequire;

    private Sprite _enabledSprite;
    private Image _image;

    private void OnEnable() {
        if (_image == null)
            _image = GetComponent<Image>();
        if (_enabledSprite == null)
            _enabledSprite = _image.sprite;

        _image.sprite = Unlocks.HasUnlock(_unlockRequire) ? _enabledSprite : _lockedSprite;
    }
}
