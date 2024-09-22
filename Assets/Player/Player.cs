using UnityEngine;
using GearCompatibility;

public class Player : MonoBehaviour
{
    public delegate void objectChange();
    public static event objectChange PlayerChanged;
    public static event objectChange StartPlayerReturn;

    public static GameObject PlayerObject {get; private set;}
    public static Transform PlayerTransform {get; private set;}
    public static Transform TargetPlayerTransform {get; private set;}
    public static bool Alive => PlayerShipData.Active;

    public static bool CanAttack {get; set;} = true;
    public static ShipClass Class {get; private set;}

    private static GameObject _firstPlayerExample;
    private static Vector3 _oldPlayerPosition;
    private static Vector3 _playerMoveDirection;
    private static float _foresightAddition;

    public static void Initialize(GameObject player, ShipClass shipClass) // надо переработать
    {
        PlayerObject = Instantiate(player, Vector3.zero, Quaternion.identity);
        PlayerTransform = PlayerObject.transform;
        TargetPlayerTransform = PlayerTransform;

        Class = shipClass;

        _firstPlayerExample = Instantiate(PlayerObject);
        _firstPlayerExample.SetActive(false);
    }

    private void OnEnable() {
        ShipStats.StatChanged += ObserveStat;
        _foresightAddition = ShipStats.GetValue("ForesightAddition");
    }

    private void OnDisable() {
        ShipStats.StatChanged -= ObserveStat;

        //Application.targetFrameRate
    }

    private void FixedUpdate() 
    {
        if (TargetPlayerTransform == null)
            return;

        _playerMoveDirection = Vector3.Lerp(_playerMoveDirection, (TargetPlayerTransform.position - _oldPlayerPosition), 4f * Time.deltaTime);
        _oldPlayerPosition = TargetPlayerTransform.position;
    }

    private void ObserveStat(string name, float val)
    {
        if (name == "ForesightAddition")
        {
            _foresightAddition = ShipStats.GetValue("ForesightAddition");
        }

    }

    public static Vector3 GetPlayerPosition(float foresight = 0f)
    {
        if (TargetPlayerTransform == null)
            return Vector3.zero;

        return TargetPlayerTransform.position + _playerMoveDirection * (foresight + _foresightAddition);
    }

    public static void UpdatePlayer()
    {
        PlayerChanged?.Invoke();
    }

    public static void SetTargetPlayer(Transform targetTransform)
    {
        TargetPlayerTransform = targetTransform;
    }

    public static void ResetTargetPlayer()
    {
        TargetPlayerTransform = PlayerTransform;
    }

    public static void ReplacePlayer(GameObject playerBody)
    {
        Vector3 pos = PlayerTransform.position;
        Destroy(PlayerObject);

        GameObject newPlayer = Instantiate(playerBody, pos, Quaternion.identity);
        newPlayer.SetActive(true);
        PlayerObject = newPlayer;

        PlayerTransform = newPlayer.transform;
        TargetPlayerTransform = PlayerTransform;
        PlayerController.ReplacePlayer(PlayerTransform);
        PlayerChanged?.Invoke();
    }

    public static void ReturnFirstPlayer()
    {
        ReplacePlayer(_firstPlayerExample);
        StartPlayerReturn?.Invoke();
    }
}
