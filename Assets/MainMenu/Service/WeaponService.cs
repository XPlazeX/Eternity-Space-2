using UnityEngine;
using UnityEngine.UI;

public class WeaponService : MonoBehaviour
{
    const int _maxWeaponLevel = 5;

    [SerializeField] private Button _btn;
    [SerializeField] private Image[] _lamps;
    [SerializeField] private Sprite _enabledLampSprite;
    [SerializeField] private Sprite _disabledLampSprite;
    [SerializeField] private BuyButton _buyBtn;
    [SerializeField] private int[] _priceList = new int[6];

    private void Start() {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        CheckLevel();
    }

    public void Upgrade()
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        if (save.WeaponLevel == _maxWeaponLevel)
            return;

        save.WeaponLevel ++;
        GameSessionInfoHandler.RewriteSessionSave(save);

        CheckLevel();
    }

    private void CheckLevel()
    {
        int level = GameSessionInfoHandler.GetSessionSave().WeaponLevel;

        _buyBtn.ScalePrice(_priceList[level]);

        if (level >= _maxWeaponLevel)
            _btn.interactable = false;

        for (int i = 0; i < _lamps.Length; i++)
        {
            if (i < level)
            {
                _lamps[i].sprite = _enabledLampSprite;
            } else {
                _lamps[i].sprite = _disabledLampSprite;
            }
        }
    }
}
