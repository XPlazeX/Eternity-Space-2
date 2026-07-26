using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponService : MonoBehaviour
{
    const int _maxWeaponLevel = 5;
    const string fix_data_collection_name = "FixWeaponUpgrades";

    [SerializeField] private Button _btn;
    [SerializeField] private WeaponLamp[] _lamps;
    [SerializeField] private Sprite _emptyLampSprite;
    [SerializeField] private Sprite _readyLampSprite;
    [SerializeField] private BuyButton _buyBtn;
    [SerializeField] private int[] _priceList = new int[6];
    [Space()]
    [SerializeField] private SoundObject _soundInstall;
    [SerializeField] private ParticleSystem _weldingPS;
    [SerializeField] private MainMenuCamera _mmCamera;
    [SerializeField] private Gradient _lampGradient;
    [SerializeField] private float _waitTime;
    [SerializeField] private float _animationTime;
    [SerializeField] private Button _repairButton;

    private bool _fixUpgrades;

    private void Start() 
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        _fixUpgrades = GameSessionInfoHandler.ExistDataCollection(fix_data_collection_name);

        CheckLevel();
    }

    public void Upgrade(bool playAnimation = false)
    {
        if (_fixUpgrades)
            return;
        
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        if (save.WeaponLevel == _maxWeaponLevel)
            return;

        save.WeaponLevel ++;
        GameSessionInfoHandler.RewriteSessionSave(save);

        if (playAnimation)
        {
            StartCoroutine(UpgradingAnimation(save.WeaponLevel - 1));
        } else
        {
            CheckLevel();
        }
    }

    public void FixUpgrades()
    {
        _fixUpgrades = true;
        GameSessionInfoHandler.AddDataCollection(fix_data_collection_name, new System.Collections.Generic.List<int>());
        CheckLevel();
    }

    public void CheckLevel()
    {
        int level = GameSessionInfoHandler.GetSessionSave().WeaponLevel;

        _buyBtn.ScalePrice(_fixUpgrades ? _priceList[_priceList.Length - 1] : _priceList[level]);

        // if (level >= _maxWeaponLevel || _fixUpgrades)
        //     _btn.interactable = false;
        _btn.interactable = (level < _maxWeaponLevel && !_fixUpgrades);

        for (int i = 0; i < _lamps.Length; i++)
        {
            if (i < level)
            {
                _lamps[i].SetMainSprite(_readyLampSprite);
            } else {
                _lamps[i].SetMainSprite(_emptyLampSprite);
            }
        }
    }

    private IEnumerator UpgradingAnimation(int lampID)
    {
        WeaponLamp lamp = _lamps[lampID];
        lamp.SetFillingColor(_lampGradient.Evaluate(0f));

        MenuNavigation menu = GameObject.FindWithTag("MenuNavigation").GetComponent<MenuNavigation>();
        menu.ToggleNavigation(false);

        float timer = _animationTime;

        _btn.interactable = false;

        SoundPlayer.PlayUISound(_soundInstall);
        _repairButton.interactable = false;

        yield return new WaitForSecondsRealtime(_waitTime);

        _mmCamera.CodeRed();
        _weldingPS.Play();

        while (timer > 0f)
        {
            lamp.SetFillingColor(_lampGradient.Evaluate(1f - (timer / _animationTime)));

            timer -= ESTime.unscaledDeltaTime;

            yield return null;
        }

        _mmCamera.ExtraNoise();
        _weldingPS.Stop();

        lamp.SetFillingColor(_lampGradient.Evaluate(0f));
        lamp.SetMainSprite(_readyLampSprite);
        CheckLevel();

        menu.ToggleNavigation(true);

        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        _repairButton.interactable = (save.HealthPoints < save.MaxHealth && !ModulasSaveHandler.GetSave().BlockHeal);
    }

    [System.Serializable]
    private struct WeaponLamp
    {
        public Image underLamp;
        public Image fillingLamp;

        public void SetMainSprite(Sprite sprite)
        {
            underLamp.sprite = sprite;
        }

        public void SetFillingColor(Color col)
        {
            fillingLamp.color = col;
        }
    }
}
