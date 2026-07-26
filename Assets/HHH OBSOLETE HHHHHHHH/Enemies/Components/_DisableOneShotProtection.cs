using UnityEngine;

[RequireComponent(typeof(DamageBody))]
public class _DisableOneShotProtection : MonoBehaviour
{
    void Start()
    {
        GetComponent<DamageBody>().OneShotProtection = false;
    }
}
