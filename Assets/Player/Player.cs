using UnityEngine;

public class Player : MonoBehaviour
{
    public delegate void objectChange();
    public static event objectChange PlayerChanged;
    public static event objectChange StartPlayerReturn;

    public static GameObject PlayerObject {get; private set;}
    public static Transform PlayerTransform {get; private set;}
    public static bool Alive => PlayerShipData.Active;

    public static bool CanAttack {get; set;} = true;

    private static GameObject _firstPlayerExample;

    public static void Initialize(GameObject player) // надо переработать
    {
        PlayerObject = Instantiate(player, Vector3.zero, Quaternion.identity);
        PlayerTransform = PlayerObject.transform;

        _firstPlayerExample = Instantiate(PlayerObject);
        _firstPlayerExample.SetActive(false);
    }

    public static void UpdatePlayer()
    {
        PlayerChanged?.Invoke();
    }

    public static void ReplacePlayer(GameObject playerBody)
    {
        Vector3 pos = PlayerTransform.position;
        Destroy(PlayerObject);

        GameObject newPlayer = Instantiate(playerBody, pos, Quaternion.identity);
        newPlayer.SetActive(true);
        PlayerObject = newPlayer;

        PlayerTransform = newPlayer.transform;
        PlayerController.ReplacePlayer(PlayerTransform);
        PlayerChanged?.Invoke();
    }

    public static void ReturnFirstPlayer()
    {
        ReplacePlayer(_firstPlayerExample);
        StartPlayerReturn?.Invoke();
    }
}
