using UnityEngine;

public class RadiantChargeBank : MonoBehaviour
{
    [SerializeField] private float maxCharge = 10f;
    [SerializeField] private float chargePerKill = 1f; // сюда можн опотом сделать ориентировку по весу

    private static RadiantChargeBank instance;

    private static float _charge = 0f;

    public static float Charge01 => instance == null ? 0f : _charge / instance.maxCharge; 

    void Start()
    {
        _charge = 0f;
        instance = this;
    }

    public static void RadiantBulletKill()
    {
        _charge = Mathf.Clamp(_charge + instance.chargePerKill, 0, instance.maxCharge);
    }

    public static bool EnoughtCharge(float ch)
    {
        return _charge >= ch;
    }

    public static bool ConsumeCharge(float ch)
    {
        if (!EnoughtCharge(ch)) return false;

        _charge -= ch;
        return true;
    }
}
