using System.Collections;
using UnityEngine;

public class DeviceHandler : MonoBehaviour
{
    private AttackHandler deviceAttack;
    private IEnumerator _chargingCoroutine;
    private IEnumerator _dischargingCoroutine;

    private float _chargingFloat;

    private float _useChargeBoost = 0.15f;
    private float _dischargingSpeed = 0.025f;
    private DeviceUI _deviceUI;

    public float ChargeTime {get; private set;} = 4f;

    public bool Workable {get; private set;} = false;

    public void Initialize() 
    {
        FindNewDeviceUI();
        if (!Workable)
            return;

        Player.StartPlayerReturn += FindNewDeviceUI;

        _chargingCoroutine = Charging();
        _dischargingCoroutine = Discharging();

        _useChargeBoost = ShipStats.GetValue("DeviceReloadBoostAfterUse");
        _dischargingSpeed *= ShipStats.GetValue("DeviceResetSpeedMultiplier");
        ChargeTime *= ShipStats.GetValue("DeviceChargeTimeBoostMultiplier");
        ChargeTime += ShipStats.GetValue("DeviceChargeTimeBoostFlat");

        if (_dischargingSpeed < 0f)
            _dischargingSpeed = 0f;
        
        _useChargeBoost = Mathf.Clamp(_useChargeBoost, 0f, 0.8f);

        PlayerController.BeginDrag += OnBeginDrag;
        PlayerController.EndDrag += OnEndDrag;
    }

    private void OnDisable() {
        if (!Workable)
            return;
        PlayerController.BeginDrag -= OnBeginDrag;
        PlayerController.EndDrag -= OnEndDrag;
    }

    public void SetDevice(Device device)
    {
        if (!Workable)
            return;
        deviceAttack = device.Fire;
        ChargeTime = device.FireReload;
    }

    private void OnBeginDrag()
    {
        StopCoroutine(_dischargingCoroutine);
        StartCoroutine(_chargingCoroutine);
        //print("begin");
    }

    private void OnEndDrag()
    {
        StopCoroutine(_chargingCoroutine);
        // if (_prepared)
        // {
        //     Use();
        //     return;
        // }
        StartCoroutine(_dischargingCoroutine);
        //print("end");
    }

    private IEnumerator Charging()
    {
        while (true)
        {
            _chargingFloat += Time.deltaTime;

            CheckCharge();
            yield return null;
        }
    }

    private IEnumerator Discharging()
    {
        while (true)
        {
            //_chargingFloat -= Time.deltaTime * 4f;
            _chargingFloat = Mathf.Lerp(_chargingFloat, 0, _dischargingSpeed);

            CheckCharge();
            yield return null;
        }
    }

    private void CheckCharge()
    {
        if (_deviceUI == null)
            return;

        float part = _chargingFloat / ChargeTime;

        if (part >= 1)
            Use();

        _deviceUI.Fill(part);
    }

    public void Use()
    {
        
        deviceAttack?.Invoke();

        _chargingFloat = ChargeTime * _useChargeBoost;
        CheckCharge();
    }

    private void FindNewDeviceUI()
    {
        _deviceUI = GameObject.FindWithTag("DeviceCharge").GetComponent<DeviceUI>();
        if (!Workable)
        {
            _deviceUI.Hide();
        }
        _chargingFloat = 0;
    }
}
