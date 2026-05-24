using UnityEngine;

public class SimpleCoreV1 : MonoBehaviour
{
    private static MainWeaponHandler _mwh;

    void Start()
    {
        _mwh = FindAnyObjectByType<MainWeaponHandler>();
    }

    public static void RadiantPowerup()
    {
        _mwh.RadiantUpgrade(out bool overcharged);
    }
}
