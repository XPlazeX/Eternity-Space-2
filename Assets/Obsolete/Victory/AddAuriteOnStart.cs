using UnityEngine;

public class AddAuriteOnStart : MonoBehaviour
{
    [SerializeField] private int _auriteVal;

    private void Start() {
        SceneStatics.CharacterCore.GetComponent<VictoryHandler>().AddAurite(_auriteVal);
    }
}
