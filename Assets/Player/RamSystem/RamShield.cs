using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamShield : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _shieldSpriteRenderer;
    [SerializeField] private Sprite _ramShieldSprite;
    [SerializeField] private Sprite _parryShieldSprite;
    [SerializeField] private Color _RamColor;
    [SerializeField] private Color _ParryColor;

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

    private float GetBoostByEnemyCount() => _ramBoostByEnemyCount * Spawner.EnemyCount;

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
            timer = _ramDuration * 2f;
            modColor = _ParryColor;
        }
        else
        {
            _shieldSpriteRenderer.sprite = _ramShieldSprite;
            timer = _ramDuration;
            modColor = _RamColor;
        }

        timer *=  1f + GetBoostByEnemyCount();

        if (timer < _lastRamTime)
            timer = _lastRamTime;
        _lastRamTime = timer;

        while (timer > 0)
        {

            modColor.a = Mathf.Lerp(1f, 0, 1f - (timer / _ramDuration));
            _shieldSpriteRenderer.color = modColor;

            timer -= Time.deltaTime;
            yield return null;
        }

        modColor.a = 0f;
        _shieldSpriteRenderer.color = modColor;

        Toggle(false);
        _lastRamTime = 0f;
        _active = false;
    }

    private void Toggle(bool tog, float frtBoost = 0)
    {
        PlayerShipData.TryToggleInvulnerability(tog);
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
