using UnityEngine;

public class PlayerShieldGetter : MonoBehaviour
{
    [SerializeField] private int _gettingValue;

    void Start()
    {
        //print($"Get shield : {_gettingValue}");
        GetComponent<PlayerDamageBody>().GetShield(_gettingValue);
    }
}
