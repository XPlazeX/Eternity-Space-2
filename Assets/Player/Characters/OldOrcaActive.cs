using UnityEngine;
using UnityEngine.UI;

public class OldOrcaActive : MonoBehaviour
{
    [SerializeField] private Button _placingButton;
    [SerializeField] private Vector2 _placingPosition;

    private void Start() {
        SceneStatics.UICore.GetComponent<UIPlacer>().PlaceInteractiveUI(_placingButton.GetComponent<RectTransform>(), _placingPosition);
    }
}
