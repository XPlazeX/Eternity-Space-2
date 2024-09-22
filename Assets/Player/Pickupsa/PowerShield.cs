using UnityEngine;

public class PowerShield : MonoBehaviour
{
    const int overdrive_shield_explosion_id = 2;

    public delegate void shieldAction();
    public static event shieldAction ShieldCreated;

    private void OnEnable() 
    {
        ShieldCreated?.Invoke();

        ShieldCreated += TryOverdrive;
    }

    private void OnDisable() {
        ShieldCreated -= TryOverdrive;
    }

    public void TryOverdrive()
    {
        OverdriveShield();
    }

    public void OverdriveShield()
    {
        ParringObject exp = ParryingHandler.GetParringObject(overdrive_shield_explosion_id);
        exp.transform.position = transform.position;

        Destroy(this.gameObject);
    }
}
