using UnityEngine;

public class Gate : MonoBehaviour
{
    // [SerializeField] private int _levelIndex;
    // [SerializeField] private bool _copyCurrentLocation = false;

    // private void Start() {
    //     if (_copyCurrentLocation)
    //         _levelIndex = GameSessionInfoHandler.GetSessionSave().LocationID;
    // }

    // private void OnTriggerEnter2D(Collider2D other) {
    //     if (other.gameObject.CompareTag("Bus"))
    //     {
    //         SceneStatics.SceneCore.GetComponent<GateHandler>().PrepareTransition(_levelIndex);
    //     }
    // }

    // private void OnTriggerExit2D(Collider2D other) {
    //     if (other.gameObject.CompareTag("Bus"))
    //     {
    //         SceneStatics.SceneCore.GetComponent<GateHandler>().StopTransition();
    //     }
    // }
}
