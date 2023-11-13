using UnityEngine;

public class MMShip : MonoBehaviour
{
    [SerializeField] private Sprite[] _shipSpritesByID;

    private void Start() {
        GetComponent<SpriteRenderer>().sprite = _shipSpritesByID[GameSessionInfoHandler.GetSessionSave().ShipModel];
    }
}
