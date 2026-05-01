using UnityEngine;

public class PlayerBarrelsSetter : MonoBehaviour
{
    [SerializeField] private Transform[] _newBarrels;

    private void Start() 
    {
        WeaponRoot playerWR = Player.PlayerObject.GetComponent<WeaponRoot>();

        for (int i = 0; i < _newBarrels.Length; i++)
        {
            playerWR.ReplaceBarrel(i, _newBarrels[i]);
        }
    }
}
