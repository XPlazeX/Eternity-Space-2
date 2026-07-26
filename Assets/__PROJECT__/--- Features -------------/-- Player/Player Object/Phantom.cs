using UnityEngine;

public class Phantom : MonoBehaviour
{
    [SerializeField] private float _foresight = 0f;
    [SerializeField] private float _moveSpeed;

    private void Update() {
        transform.position = Player.GetPlayerPosition(_foresight);//Vector3.Lerp(transform.position, Player.GetPlayerPosition(_foresight), _moveSpeed * ESTime.worldDeltaTime);
    }
}
