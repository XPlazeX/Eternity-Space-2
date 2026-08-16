using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamShield : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _shieldSpriteRenderer;
    [SerializeField] private Sprite _ramShieldSprite;
    [SerializeField] private Sprite _parryShieldSprite;
    [SerializeField] private Sprite _drillShieldSprite;
    [SerializeField] private Color _RamColor;
    [SerializeField] private Color _ParryColor;
    [Space()]
    [SerializeField] private Color _DrillColor;
    [SerializeField] private ParticleSystem _drillPS;
    [SerializeField] private GameObject _drillObject;

    public bool DrillMode {get; set;} = false;
    public bool UseConstantEnemyCount {get; set;} = false;
    public int ConstantEnemyCount {get; set;} = 0;

    private IEnumerator shielding;
    private bool _active = false;
    private float _lastBonus;
    private float _lastRamTime = 0f;
    private float _ramFirerateBoost;
    private float _parryAdditiveFirerateBoost;
    private float _ramDuration;
    private float _ramBoostByEnemyCount;

    private void Start() 
    {
        _ramFirerateBoost = ShipStats.GetValue("RamFirerateBoost");
        _parryAdditiveFirerateBoost = ShipStats.GetValue("ParryAdditiveFirerateBoost");
        _ramDuration = ShipStats.GetValue("RamBoostDuration");
        _ramBoostByEnemyCount = ShipStats.GetValue("RamBoostByEnemy");

        _drillPS.Stop();
        _drillObject.SetActive(false);
    }

    public void EnableShield(bool byParry = false)
    {
        if (_active)
        {
            StopCoroutine(shielding);
            Toggle(false);
        }

        shielding = Shielding(byParry);
        StartCoroutine(shielding);
    }
    
    private void OnDisable() {
        Toggle(false);
    }

    private float GetBoostByEnemyCount() => _ramBoostByEnemyCount * (UseConstantEnemyCount ? ConstantEnemyCount : Spawner.EnemyCount);

    private IEnumerator Shielding(bool byParry = false)
    {
        print("Shielding");
        _active = true;

        float timer = 0;
        Color modColor = new Color();

        if (byParry)
                Toggle(true, _ramFirerateBoost + _parryAdditiveFirerateBoost);
            else
                Toggle(true, _ramFirerateBoost);

        if (byParry)
        {
            _shieldSpriteRenderer.sprite = _parryShieldSprite;
            timer = _ramDuration * 1.5f;
            modColor = _ParryColor;
        }
        else
        {
            _shieldSpriteRenderer.sprite = DrillMode ? _drillShieldSprite : _ramShieldSprite;
            timer = _ramDuration;
            modColor = DrillMode ? _DrillColor : _RamColor;
        }

        timer *=  1f + GetBoostByEnemyCount();

        if (timer < _lastRamTime)
            timer = _lastRamTime;
        _lastRamTime = timer;

        if (DrillMode)
        {
            _drillPS.Play();
            _drillObject.SetActive(true);
        }

        while (timer > 0)
        {

            modColor.a = Mathf.Lerp(1f, 0, 1f - (timer / _ramDuration));
            _shieldSpriteRenderer.color = modColor;

            timer -= ESTime.worldDeltaTime;
            yield return null;
        }

        if (DrillMode)
        {
            _drillPS.Stop();
            _drillObject.SetActive(false);
        }
        modColor.a = 0f;
        _shieldSpriteRenderer.color = modColor;

        Toggle(false);
        _lastRamTime = 0f;
        _active = false;
    }

    private void Toggle(bool tog, float frtBoost = 0)
    {
        // PlayerShipData.TryToggleInvulnerability(tog);
        if (tog)
        {
            ShipStats.IncreaseStat("MainWeaponFirerateMultiplier", frtBoost);
            _lastBonus = frtBoost;
        }
        else
        {
            ShipStats.IncreaseStat("MainWeaponFirerateMultiplier", -_lastBonus);
            _lastBonus = 0;
        }   
    }
}
