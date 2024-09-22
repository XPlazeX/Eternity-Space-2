using UnityEngine;

public class DisableMainWeapon : MonoBehaviour
{
    void Start()
    {
        Player.PlayerObject.GetComponent<WeaponRoot>().enabled = false;
    }
}
