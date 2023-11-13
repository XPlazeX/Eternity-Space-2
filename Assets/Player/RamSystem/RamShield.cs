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
    private float _ramFirerateBoost;
    private float _parryAdditiveFirerateBoost;
    private float _ramDuration;

    private void Start() {
        _ramFirerateBoost = ShipStats.GetValue("RamFirerateBoost");
        _parryAdditiveFirerateBoost = ShipStats.GetValue("ParryAdditiveFirerateBoost");
        _ramDuration = ShipStats.GetValue("RamBoostDuration");
    }

    public void EnableShield(bool byParry = false)
    {
        if (_active)
        {
            Toggle(false);
            StopCoroutine(shielding);
        }
        else
        {
            shielding = Shielding(byParry);

            if (byParry)
                Toggle(true, _ramFirerateBoost + _parryAdditiveFirerateBoost);
            else
                Toggle(true, _ramFirerateBoost);
                
            StartCoroutine(shielding);
        }
    }
    private void OnDisable() {
        Toggle(false);
    }

    private IEnumerator Shielding(bool byParry = false)
    {
        print("Shielding");
        float timer = 0;
        Color modColor = new Color();

        if (byParry)
        {
            _shieldSpriteRenderer.sprite = _parryShieldSprite;
            // Toggle(true, _ramFirerateBoost + _parryAdditiveFirerateBoost);
            timer = _ramDuration * 2f;
            modColor = _ParryColor;
        }
        else
        {
            _shieldSpriteRenderer.sprite = _ramShieldSprite;
            // Toggle(true, _ramFirerateBoost);
            timer = _ramDuration;
            modColor = _RamColor;
        }

        while (timer > 0)
        {

            modColor.a = Mathf.Lerp(1f, 0, 1f - (timer / _ramDuration));
            _shieldSpriteRenderer.color = modColor;

            timer -= Time.deltaTime;
            yield return null;
        }


        Toggle(false);
    }

    private void Toggle(bool tog, float frtBoost = 0)
    {
        _active = tog;
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
